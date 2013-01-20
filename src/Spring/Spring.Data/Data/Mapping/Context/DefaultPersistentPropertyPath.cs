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
using Spring.Util;

namespace Spring.Data.Mapping.Context
{
    /// <summary>
    /// Abstraction of a path of <see cref="IPersistentProperty"/>s.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class DefaultPersistentPropertyPath<TProperty> : IPersistentPropertyPath<TProperty>
        where TProperty : IPersistentProperty
    {
        private readonly IList<TProperty> _properties;


        /// <summary>
        /// Creates a new <see cref="DefaultPersistentPropertyPath{TProperty}"/> for the given
        /// <see cref="IPersistentProperty"/>s.
        /// </summary>
        /// <param name="properties">must not be <code>null</code></param>
	    public DefaultPersistentPropertyPath(IList<TProperty> properties)
        {
		    AssertUtils.ArgumentNotNull(properties, "properties");
		    AssertUtils.IsTrue(properties.Count > 0);

		    _properties = properties;
	    }

        public string ToDotPath()
        {
            return ToPath(null, null);
        }

        public string ToDotPath(Converter<TProperty, string> converter) 
        {
		    return ToPath(null, converter);
	    }

	    public string ToPath(string delimiter) 
        {
		    return ToPath(delimiter, null);
	    }

	    public string ToPath(string delimiter, Converter<TProperty, string> converter) 
        {
		    var converterToUse = converter ?? Convert;
		    var delimiterToUse = delimiter ?? ".";

		    IList<string> result = new List<string>();

		    foreach(TProperty property in _properties) 
            {
			    result.Add(converterToUse.Invoke(property));
		    }

		    return StringUtils.CollectionToDelimitedString(result, delimiterToUse);
	    }

        private static string Convert(TProperty source)
        {
            return source.Name;
        }

        public TProperty LeafProperty
        {
            get
            {
                if (_properties.Count == 0)
                    return default(TProperty);

                return _properties.Last();
            }
        }

	    public TProperty BaseProperty
        {
	        get { return _properties.FirstOrDefault(); }
        }

	    public bool IsBasePathOf(IPersistentPropertyPath<TProperty> path) 
        {
		    if (path == null)
			    return false;

		    IEnumerator<TProperty> iterator = path.GetEnumerator();

		    foreach (TProperty property in this) 
            {
			    if (!iterator.MoveNext()) 
				    return false;

			    TProperty reference = iterator.Current;
			    if (!property.Equals(reference)) 
				    return false;
		    }

		    return true;
	    }

	    public IPersistentPropertyPath<TProperty> GetExtensionForBaseOf(IPersistentPropertyPath<TProperty> baseProperty) 
        {
		    if (!baseProperty.IsBasePathOf(this)) 
			    return this;

		    IList<TProperty> properties = new List<TProperty>();
		    IEnumerator<TProperty> iterator = GetEnumerator();

		    for (int i = 0; i < baseProperty.Length; i++) 
            {
			    iterator.MoveNext();
		    }

		    while (iterator.MoveNext()) {
			    properties.Add(iterator.Current);
		    }

		    return new DefaultPersistentPropertyPath<TProperty>(properties);
	    }

	    public IPersistentPropertyPath<TProperty> ParentPath
        {
	        get
	        {
	            int size = _properties.Count;
	            if (size <= 1)
	            {
	                return this;
	            }
	            return new DefaultPersistentPropertyPath<TProperty>(_properties.Take(size-1).ToList());
	        }
        }

        public int Length
        {
            get { return _properties.Count; }
        }

	    public IEnumerator<TProperty> GetEnumerator() 
        {
		    return _properties.GetEnumerator();
	    }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

	    public override bool Equals(object obj)
        {
		    if (this == obj)
			    return true;

		    if (obj == null || GetType() != obj.GetType())
			    return false;

		    var that = (DefaultPersistentPropertyPath<TProperty>) obj;

		    return _properties.Equals(that._properties);
	    }

	    public override int GetHashCode() 
        {
		    return _properties.GetHashCode();
	    }

	    public override string ToString()
        {
		    return ToDotPath();
	    }
    }
}
