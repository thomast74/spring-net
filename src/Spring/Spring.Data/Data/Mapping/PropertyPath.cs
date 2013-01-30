#region License

/*
 * Copyright © 2002-2013 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Spring.Data.Util;
using Spring.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Abstraction of a <see cref="PropertyPath"/> of a domain class.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class PropertyPath
    {
        private const string Delimiters = "_\\.";
        private const string AllUppercase = "^[A-Z0-9._$]+$";
        private static readonly Regex Splitter = new Regex("(?:[%s]?([%s]*?[^%s]+))".Replace("%s", Delimiters));

	    private readonly ITypeInformation _owningType;
	    private readonly string _name;
	    private readonly ITypeInformation _type;
	    private readonly bool _isCollection;
        private readonly PropertyPathEnumerator _enumerator;
	    private PropertyPath _next;

        /// <summary>
        /// Creates a leaf <see cref="PropertyPath"/> (no nested ones) with the given name inside the given owning type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owningType"></param>
	    public PropertyPath(string name, Type owningType) 
            : this(name, ClassTypeInformation.From(owningType), null)
        {
	    }

        /// <summary>
        /// Creates a leaf <see cref="PropertyPath"/> (no nested ones with the given name and owning type.
        /// </summary>
        /// <param name="name">must not be <code>null</code> or empty.</param>
        /// <param name="owningType">must not be <code>null</code> or empty.</param>
        /// <param name="baseProperty">the <see cref="PropertyPath"/> previously found</param>
	    public PropertyPath(string name, ITypeInformation owningType, PropertyPath baseProperty)
        {
		    AssertUtils.ArgumentHasText(name, "name");
		    AssertUtils.ArgumentNotNull(owningType, "owningType");

            var propertyName = name;
		    var type = owningType.GetProperty(propertyName);

		    if (type == null)
            {
			    throw new PropertyReferenceException(propertyName, owningType, baseProperty);
		    }

		    _owningType = owningType;
		    _isCollection = type.IsCollectionLike;
		    _type = type.ActualType;
		    _name = propertyName;
            _enumerator = PropertyPathEnumerator.Create(this);
	    }

        /// <summary>
        /// Returns the owning type of the <see cref="PropertyPath"/>.
        /// </summary>
        /// <returns>
        /// the owningType will never be <code>null</code>.
        /// </returns>
        public ITypeInformation OwningType
        {
            get { return _owningType; }
        }

        /// <summary>
        /// Returns the name of the <see cref="PropertyPath"/>.
        /// </summary>
        /// <returns>
        /// the name will never be <code>null</code>.
        /// </returns>
        public string Segment
        {
            get { return _name; }
        }

        /// <summary>
        /// Returns the leaf property of the <see cref="PropertyPath"/>.
        /// </summary>
        /// <returns>
        /// will never be <code>null</code>.
        /// </returns>
        public PropertyPath LeafProperty
        {
            get
            {
                PropertyPath result = this;

                while (result.HasNext)
                {
                    result = result.Next;
                }

                return result;
            }
        }

        /// <summary>
	    /// Returns the type of the property will return the plain resolved type for simple properties, 
	    /// the component type for any <see cref="IEnumerable{T}"/> or the value type of a 
	    /// <see cref="Dictionary{TKey,TValue}"/> if the property is one.
        /// </summary>
        public Type Type
        {
            get { return _type.Type; }
        }

        /// <summary>
        /// Returns the next nested <see cref="PropertyPath"/>.
        /// </summary>
        /// <returns>
        /// the next nested <see cref="PropertyPath"/> or <code>null</code> 
        /// if no nested <see cref="PropertyPath"/> available.
        /// </returns>
        public PropertyPath Next
        {
            get { return _next; }
        }

        /// <summary>
	    /// Returns whether there is a nested <see cref="PropertyPath"/>. 
	    /// If this returns <code>true</code> you can expect
	    /// <see cref="Next"/> to return a non-<code>null</code> value.
        /// </summary>
        public bool HasNext
        {
            get { return _next != null; }
        }

        /// <summary>
        /// Returns the <see cref="PropertyPath"/> in dot notation.
        /// </summary>
        public string DotPath
        {
            get
            {
                if (HasNext)
                {
                    return Segment + "." + Next.DotPath;
                }

                return Segment;
            }
        }

        /// <summary>
        /// Returns whether the <see cref="PropertyPath"/> is actually a collection.
        /// </summary>
        public bool IsCollection
        {
            get { return _isCollection; }
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("[Segment = {0}, Type = {1}, Next = {2}]", _name, _type, _next);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
		    if (this == obj)
			    return true;

		    if (obj == null || GetType() != obj.GetType())
			    return false;

		    var that = (PropertyPath) obj;

            return _name.Equals(that._name) && _type.Equals(that._type) && ObjectUtils.NullSafeEquals(_next, that._next);
        }


        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
		    int result = 17;

		    result += 31 * _name.GetHashCode();
		    result += 31 * _type.GetHashCode();
		    result += 31 * (_next == null ? 0 : _next.GetHashCode());

		    return result;
	    }

        /// <summary>
        /// Returns the <see cref="IEnumerator{PropertyPath}"/> for this PropertyPath
        /// </summary>
        /// <returns>
        /// </returns>
	    public IEnumerator<PropertyPath> GetEnumerator()
        {
            return _enumerator;
        }

        public class PropertyPathEnumerator : IEnumerator<PropertyPath>
        {
            private readonly PropertyPath _start;
            private PropertyPath _current;
            private bool _isFirst;

            PropertyPathEnumerator(PropertyPath propertyPath)
            {
                _start = propertyPath;
                _current = null;
                _isFirst = true;
            }

            public PropertyPath Current
            {
                get { return _current; }
            }

            public void Dispose()
            {
                _current = null;
                _isFirst = true;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                if (_isFirst)
                {
                    _current = _start;
                    _isFirst = false;
                }
                else
                    _current = _current._next;

                return _current != null;
            }

            public void Reset()
            {
                _current = _start;
                _isFirst = true;
            }

            public static PropertyPathEnumerator Create(PropertyPath propertyPath)
            {
                return new PropertyPathEnumerator(propertyPath);
            }
        }

        /// <sumamry>
	    /// Extracts the <see cref="PropertyPath"/> chain from the given source <see cref="String"/> and type.
        /// </sumamry>
        /// <param name="source"></param>
	    public static PropertyPath From<T>(string source)
        {
		    return From(source, ClassTypeInformation.From<T>());
	    }

        /// <summary>
        /// Extracts the <see cref="PropertyPath"/> chain from the given source <see cref="String"/> 
        /// and <see cref="ITypeInformation"/>.
        /// </summary>
        /// <param name="source">must not be <code>null</code></param>
        /// <param name="type"></param>
	    public static PropertyPath From(string source, ITypeInformation type)
        {
            var matcher = Splitter.Matches("_" + source);

            IList<string> iteratorSource =
                (from Match match in matcher
                 from Capture capture in match.Captures
                 select capture.Value.Substring(1))
                    .ToList();

            PropertyPath result = null;
		    PropertyPath current = null;
            var parts = iteratorSource.GetEnumerator();

		    while (parts.MoveNext())
            {
			    if (result == null)
                {
				    result = Create(parts.Current, type, null);
				    current = result;
			    }
                else
                {
				    current = Create(parts.Current, current);
			    }
		    }

		    return result;
	    }

        /// <summary>
        /// Creates a new <see cref="PropertyPath"/> as subordinary of the given <see cref="PropertyPath"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="baseProperty"></param>
	    private static PropertyPath Create(string source, PropertyPath baseProperty) 
        {
		    var propertyPath = Create(source, baseProperty._type, baseProperty);
		    baseProperty._next = propertyPath;

		    return propertyPath;
	    }

        /// <summary>
	    /// Factory method to create a new <see cref="PropertyPath"/> for the given <see cref="String"/> 
	    /// and owning type. It will inspect the given source for camel-case parts and traverse the 
	    /// <see cref="String"/> along its parts starting with the entire one and chewing off parts from 
	    /// the right side then. Whenever a valid property for the given class is found, the tail
	    /// will be traversed for subordinary properties of the just found one and so on.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="baseProperty"></param>
	    private static PropertyPath Create(string source, ITypeInformation type, PropertyPath baseProperty)
        {
		    return Create(source, type, "", baseProperty);
	    }

        /// <summary>
	    /// Tries to look up a chain of <see cref="PropertyPath"/>s by trying the givne source first. 
	    /// If that fails it will split the source apart at camel case borders (starting from the right 
	    /// side) and try to look up a <see cref="PropertyPath"/> from the calculated head and recombined 
	    /// new tail and additional tail.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="addTail"></param>
        /// <param name="baseProperty"></param>
        /// <returns>
        /// </returns>
	    private static PropertyPath Create(string source, ITypeInformation type, 
                                           string addTail, PropertyPath baseProperty)
        {
		    PropertyReferenceException exception = null;
		    PropertyPath current = null;

		    try
            {
			    current = new PropertyPath(source, type, baseProperty);

			    if (StringUtils.HasText(addTail))
                {
				    current._next = Create(addTail, current._type, current);
			    }

			    return current;
		    }
            catch (PropertyReferenceException e)
            {
			    if (current != null)
                {
				    throw e;
			    }

			    exception = e;
		    }

		    var pattern = new Regex("\\p{Lu}+\\p{Ll}*$");
		    var matcher = pattern.Match(source);

		    if (matcher.Length > 0 && matcher.Index > 0)
		    {
		        var position = matcher.Index;
			    var head = source.Substring(0, position);
			    var tail = source.Substring(position);

			    return Create(head, type, tail + addTail, baseProperty);
		    }

		    throw exception;
	    }
    }
}
