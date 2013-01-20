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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Spring.Util;
using System.Linq;

namespace Spring.Data.Util
{
    /// <summary>
    /// Basic <see cref="TypeDiscoverer"/> that contains basic functionality to discover property types.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <auhtor>Thomas Trageser</auhtor>
    public class TypeDiscoverer : ITypeInformation
    {
        protected Type _type;
        protected IDictionary<Type, Type> _typeVariableDictionary;

        private readonly IDictionary<string, ITypeInformation> _fieldTypes =
            new ConcurrentDictionary<string, ITypeInformation>();

        /// <summary>
        /// Creates a nee <see cref="TypeDiscoverer"/> for the given type, type variable map and parent.
        /// </summary>
        public TypeDiscoverer(Type type) : this(type, null)
        {
        }

        /// <summary>
        /// Creates a nee <see cref="TypeDiscoverer"/> for the given type, type variable map and parent.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeVariableDictionary"></param>
        public TypeDiscoverer(Type type, IDictionary<Type, Type> typeVariableDictionary)
        {
            _type = type;
            _typeVariableDictionary = typeVariableDictionary ?? new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Creates {@link TypeInformation} for the given {@link Type}.
        /// </summary>
        public ITypeInformation CreateInfo<TType>()
        {
            Type fieldType = typeof(TType);
            return CreateInfo(fieldType);
        }

        /// <summary>
        /// Creates <see cref="ITypeInformation"/> for the given <see cref="Type"/>.
        /// </summary>
        public virtual ITypeInformation CreateInfo(Type fieldType)
        {
            if (fieldType == _type)
                return this;

            if (fieldType.IsArray)
                return new GenericArrayTypeInformation(fieldType, this);

            if (fieldType.IsGenericType)
                return new ParameterizedTypeInformation(fieldType, this);

            if (fieldType.IsGenericParameter)
                return new TypeVariableTypeInformation(fieldType, Type, this, null);

            if (fieldType.IsClass || fieldType.IsInterface || fieldType.IsValueType)
                return ClassTypeInformation.From(fieldType);

            throw new ArgumentException();
        }

        /// <summary>
        /// Returns the type variable dictionary. Will traverse the parents up to the root on and use it's dictionary.
        /// </summary>
        public virtual IDictionary<Type, Type> TypeVariableDictionary
        {
            get { return _typeVariableDictionary; }
        }

        /// <summary>
        /// Resolves the given type into a plain Type.
        /// </summary>
        /// <param name="type"></param>
        protected Type ResolveType(Type type)
        {
            if (_typeVariableDictionary.ContainsKey(type))
            {
                return _typeVariableDictionary[type];
            }

            return type;
        }

        public IList<ITypeInformation> GetParameterTypes(ConstructorInfo constructorInfo)
        {
            AssertUtils.ArgumentNotNull(constructorInfo, "constructorInfo must no be null");
            IList<ITypeInformation> result = new List<ITypeInformation>();

            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                result.Add(CreateInfo(parameterInfo.ParameterType));
            }

            return result;
        }

        public ITypeInformation GetProperty(string fieldname)
        {
            int separatorIndex = fieldname.IndexOf('.');

            if (separatorIndex == -1)
            {
                if (_fieldTypes.ContainsKey(fieldname))
                {
                    return _fieldTypes[fieldname];
                }

                var propertyInformation = GetPropertyInformation(fieldname);
                if (propertyInformation != null)
                {
                    _fieldTypes.Add(fieldname, propertyInformation);
                }
                return propertyInformation;
            }

            string head = fieldname.Substring(0, separatorIndex);
            ITypeInformation info = _fieldTypes.ContainsKey(head) ? _fieldTypes[head] : null;
            return info == null ? null : info.GetProperty(fieldname.Substring(separatorIndex + 1));
        }

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/> for the given atomic field. Will inspect fields first
        /// and return the type of a field if available. Otherwise it will fall back to a {@link PropertyDescriptor}.
        /// </summary>
        private ITypeInformation GetPropertyInformation(string fieldname)
        {
            Type type = Type;
            FieldInfo field;
            do
            {
                field = type.GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    return CreateInfo(field.FieldType);
                }
                type = type.BaseType;
            } while (type != typeof (object));

            return null;
        }

        public virtual Type Type
        {
            get { return ResolveType(_type); }
        }

        public ITypeInformation ActualType
        {
            get
            {
                if (Type.IsArray)
                {
                    return CreateInfo(Type.GetElementType());
                }

                if (IsDictionary)
                {
                    return DictionaryValueType;
                }

                if (IsCollectionLike)
                {
                    return ComponentType;
                }

                var arguments = TypeArguments;
                if (arguments.Count == 0)
                {
                    return this;
                }

                return arguments[arguments.Count-1];
            }
        }

        public bool IsDictionary
        {
            get
            {
                if (Type.Name.Contains("IDictionary"))
                    return true;

                var type = Type;
                while (type != null && type != typeof(object))
                {
                    if (type.GetInterface(typeof (IDictionary<,>).Name) != null)
                        return true;

                    type = type.BaseType;
                }

                return false;
            }
        }

        public virtual ITypeInformation DictionaryValueType
        {
            get
            {
                if (IsDictionary)
                {
                    return GetTypeArgument(GetDictionaryType(Type), 1);
                }

                IList<ITypeInformation> arguments = TypeArguments;
                if (arguments.Count > 1)
                {
                    return arguments[1];
                }

                return null;
            }
        }

        public bool IsCollectionLike
        {
            get
            {
                if (Type.Name.Contains("IEnumerable") || Type.IsArray)
                    return true;

                var type = Type;
                if (!type.IsGenericType || type.GetInterface(typeof(IDictionary<,>).Name) != null)
                    return false;

                if (type.GetInterface(typeof(IEnumerable<>).Name) != null)
                    return true;

                return false;
            }
        }

        public virtual ITypeInformation ComponentType
        {
            get
            {
                Type rawType = Type;

                if (rawType == typeof(string))
                    return null;

                if (rawType.IsArray)
                    return CreateInfo(rawType.GetElementType());

                if (IsDictionary)
                {
                    return GetTypeArgument(GetDictionaryType(rawType), 0);
                }

                if (IsCollectionLike)
                {
                    return GetTypeArgument(rawType, 0);                    
                }

                if (rawType.GetInterface(typeof(IEnumerable<>).Name) != null)
                {
                    return GetTypeArgument(rawType.GetInterface(typeof(IEnumerable<>).Name), 0);
                }

                return TypeArguments.Count > 0 ? TypeArguments[0] : null;
            }
        }

        private Type GetDictionaryType(Type type)
        {
            while (type != null && type != typeof (object))
            {
                if (type.Name.Contains("IDictionary"))
                    return type;
                if (type.GetInterface(typeof (IDictionary<,>).Name) != null)
                    return type.GetInterface(typeof (IDictionary<,>).Name);

                type = type.BaseType;
            }

            return null;
        }

        public ITypeInformation GetReturnType(MethodInfo methodInfo)
        {
		    AssertUtils.ArgumentNotNull(methodInfo, "methodInfo must not be null");

		    return CreateInfo(methodInfo.ReturnType);
	    }

	    public IList<ITypeInformation> GetParameterTypes(MethodInfo methodInfo)
        {
		    AssertUtils.ArgumentNotNull(methodInfo, "methodInfo must not be null");

            IList<ITypeInformation> result = new List<ITypeInformation>();

	        foreach (var parameter in methodInfo.GetParameters())
	        {
                result.Add(CreateInfo(parameter.ParameterType));
	        }

		    return result;
	    }

	    public ITypeInformation GetSuperTypeInformation(Type superType)
        {
		    var type = Type;

		    if (!superType.IsAssignableFrom(type)) 
			    return null;

		    if (type == superType)
			    return this;

            if (superType.IsInterface)
            {
                foreach (var ifType in Type.GetInterfaces())
                {
                    if (ifType == superType)
                        return CreateInfo(ifType);
                }
            }

	        if (Type.BaseType == typeof (object))
	            return null;

	        return CreateInfo(Type.BaseType).GetSuperTypeInformation(superType);
	    }


        public virtual IList<ITypeInformation> TypeArguments
        {
            get 
            {
                if (!Type.IsGenericType)
                    return new List<ITypeInformation>();

                IList<ITypeInformation> result = new List<ITypeInformation>();
                foreach (var argument in Type.GetGenericArguments())
                {
                    result.Add(CreateInfo(argument));
                }

                return result;
            }
        }

        public virtual bool IsAssignableFrom(ITypeInformation target)
        {
            var superType = target.GetSuperTypeInformation(Type);
            return superType != null ? superType.Equals(this) : false;
        }

	    private ITypeInformation GetTypeArgument(Type type, int index)
	    {
	        if (!type.IsGenericType)
	            return null;

	        var arguments = type.GetGenericArguments();
            return (arguments.Length > index) ? CreateInfo(arguments[index]) : null;
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
            return string.Format("[Type = {0}]", _type);
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
		    if (obj == this)
			    return true;

		    if (obj == null)
			    return false;

		    var that = (TypeDiscoverer)obj;

		    var typeEqual = ObjectUtils.NullSafeEquals(_type, that._type);
		    var typeVariableDictionaryEqual = _typeVariableDictionary.SequenceEqual(that._typeVariableDictionary);

		    return typeEqual && typeVariableDictionaryEqual;
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
		    result += ObjectUtils.NullSafeHashCode(_type);
		    result += ObjectUtils.NullSafeHashCode(_typeVariableDictionary);
		    return result;
	    }

    }
}
