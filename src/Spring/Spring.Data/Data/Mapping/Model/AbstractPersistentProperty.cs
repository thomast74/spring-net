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
using System.Collections.Generic;
using System.Reflection;
using Spring.Data.Util;
using Spring.Util;
using Spring.Data.Annotation;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Simple impementation of <see cref="IPersistentProperty"/>.
    /// </summary>
    /// <author>Jon Brisbin</author>
    /// <author>Oliver Gierke</author>
    public abstract class AbstractPersistentProperty : IPersistentProperty
    {
	    protected readonly string _name;
	    protected readonly ITypeInformation _information;
	    protected readonly Type _rawType;
	    protected readonly FieldInfo _fieldInfo;
	    protected readonly Association _association;
	    protected readonly IPersistentEntity _owner;

	    private readonly SimpleTypeHolder _simpleTypeHolder;


	    public AbstractPersistentProperty(FieldInfo fieldInfo, IPersistentEntity owner, SimpleTypeHolder simpleTypeHolder)
        {
		    AssertUtils.ArgumentNotNull(fieldInfo, "fieldInfo must not be null");
		    AssertUtils.ArgumentNotNull(owner, "owner must not be null");
		    AssertUtils.ArgumentNotNull(simpleTypeHolder, "simpleTypeHolder must not be null");

	        _name = fieldInfo.Name;
	        _rawType = fieldInfo.FieldType;
		    _information = owner.TypeInformation.GetProperty(_name);
		    _fieldInfo = fieldInfo;
		    _association = IsAssociation ? CreateAssociation() : null;
		    _owner = owner;
		    _simpleTypeHolder = simpleTypeHolder;
	    }

        public abstract bool IsIdProperty { get; }
        public abstract bool IsVersionProperty { get; }
	    public abstract Association CreateAssociation();

        public IPersistentEntity Owner
        {
            get { return _owner; }
        }

	    public string Name 
        {
	        get { return _name; }
        }

	    public Type Type
        {
	        get { return _information.Type; }
        }

	    public Type RawType
        {
	        get { return _rawType; }
        }

	    public ITypeInformation TypeInformation 
        {
	        get { return _information; }
        }

        public IEnumerable<ITypeInformation> PersistentEntityTypes
        {
            get
            {
                IList<ITypeInformation> result = new List<ITypeInformation>();

                if (IsEntity)
                {
                    result.Add(TypeInformation);
                }

                if (TypeInformation.IsCollectionLike || IsDictionary)
                {
                    ITypeInformation nestedType = GetTypeInformationIfNotSimpleType(TypeInformation.ActualType);
                    if (nestedType != null)
                    {
                        result.Add(nestedType);
                    }
                }

                return result;
            }
        }

        private ITypeInformation GetTypeInformationIfNotSimpleType(ITypeInformation typeInformation) 
        {
            return typeInformation == null || _simpleTypeHolder.IsSimpleType(typeInformation.Type) ? null : typeInformation;
	    }

	    public FieldInfo FieldInfo
        {
	        get { return _fieldInfo; }
        }

	    public virtual string SpelExpression
        {
	        get { return null; }
        }

        public virtual bool IsTransient
        {
            get { return false; }
        }

	    public bool ShallBePersisted
        {
	        get { return !IsTransient; }
        }

        public virtual bool IsAssociation
        {
            get
            {
                return Attribute.GetCustomAttribute(_fieldInfo, typeof (ReferenceAttribute)) != null;
            }
        }

        public virtual Association Association
        {
            get { return _association; }
        }

        public bool IsCollectionLike
        {
            get { return _information.IsCollectionLike; }
        }

        public bool IsDictionary
        {
            get { return _information.IsDictionary; }
        }

	    public bool IsArray
        {
	        get { return Type.IsArray; }
        }

        public bool IsEntity
        {
            get
            {
                ITypeInformation actualType = _information.ActualType;
                bool isComplexType = actualType != null && !_simpleTypeHolder.IsSimpleType(actualType.Type);
                return isComplexType && !IsTransient && !IsCollectionLike && !IsDictionary;
            }
        }

        public Type ComponentType
        {
            get
            {
                if (!IsDictionary && !IsCollectionLike)
                {
                    return null;
                }

                ITypeInformation componentType = _information.ComponentType;
                return componentType == null ? null : componentType.Type;
            }
        }

        public Type DictionaryValueType
        {
            get { return IsDictionary ? _information.DictionaryValueType.Type : null; }
        }

	    public override bool Equals(object obj) 
        {
		    if (this == obj)
			    return true;

		    if (!(obj is AbstractPersistentProperty)) 
			    return false;

		    var that = (AbstractPersistentProperty) obj;

	        if (_fieldInfo != null && that._fieldInfo == null) return false;

	        return _fieldInfo.Equals(that._fieldInfo);
        }

	    public override int GetHashCode()
	    {	        
	        return _fieldInfo.GetHashCode();
	    }
    }
}
