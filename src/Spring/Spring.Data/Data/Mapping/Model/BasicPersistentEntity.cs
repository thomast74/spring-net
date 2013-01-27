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
using System.Linq;
using Spring.Data.Annotation;
using SpringCollections = Spring.Collections.Generic;
using Spring.Data.Util;
using Spring.Util;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Simple value object to capture information of {@link PersistentEntity}s.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Jon Brisbin</author>
    /// <author>Patryk Wasik</author>
    /// <author>Thomas Trageser</author>
    public class BasicPersistentEntity : IMutablePersistentEntity
    {
	    private readonly PreferredConstructor _constructor;
	    private readonly ITypeInformation _information;
	    private readonly SpringCollections.ISet<IPersistentProperty> _properties;
	    private readonly SpringCollections.ISet<Association> _associations;

	    private IPersistentProperty _idProperty;
	    private IPersistentProperty _versionProperty;

        /// <summary>
        /// Creates a new {@link BasicPersistentEntity} from the given {@link TypeInformation}.
        /// </summary>
        /// <param name="information">must not be {@literal null}.</param>
	    public BasicPersistentEntity(ITypeInformation information) : this(information, null)
        {
	    }

        /// <summary>
        /// Creates a new <see cref="BasicPersistentEntity"/> for the given <see cref="ITypeInformation"/> 
        /// and <see cref="IComparer{T}"/>. The given <see cref="IComparer{T}"/> will be used to define the 
        /// order of the <see cref="IPersistentProperty"/> instances added to the entity.
        /// </summary>
        /// <param name="information">must not be <code>null</code></param>
        /// <param name="comparer"></param>
        public BasicPersistentEntity(ITypeInformation information, PersistentPropertyComparer<IPersistentProperty> comparer)
        {
		    AssertUtils.ArgumentNotNull(information, "information");

		    _information = information;
		    _constructor = new PreferredConstructorDiscoverer(information, this).Constructor;

            if (comparer == null)
                _properties = new SpringCollections.HashedSet<IPersistentProperty>();
            else
                _properties = new SpringCollections.SortedSet<IPersistentProperty>(comparer);

            if (comparer == null)
                _associations = new SpringCollections.HashedSet<Association>();
            else
                _associations = new SpringCollections.SortedSet<Association>(new AssociationComparer(comparer));
        }

	    public PreferredConstructor PersistenceConstructor
        {
	        get { return _constructor; }
        }

	    public bool IsConstructorArgument(IPersistentProperty property)
        {
		    return _constructor != null && _constructor.IsConstructorParameter(property);
	    }

	    public bool IsIdProperty(IPersistentProperty property)
        {
		    return _idProperty != null && _idProperty.Equals(property);
	    }

	    public bool IsVersionProperty(IPersistentProperty property)
        {
		    return _versionProperty != null && _versionProperty.Equals(property);
	    }

        public string Name
        {
            get { return Type.Name; }
        }

	    public IPersistentProperty IdProperty
        {
	        get { return _idProperty; }
        }

	    public IPersistentProperty VersionProperty
        {
	        get { return _versionProperty; }
        }

	    public bool HasIdProperty
        {
	        get { return _idProperty != null; }
        }

	    public bool HasVersionProperty
        {
	        get { return _versionProperty != null; }
        }

	    public void AddPersistentProperty(IPersistentProperty property) 
        {
		    AssertUtils.ArgumentNotNull(property, "property");

		    _properties.Add(property);

		    if (property.IsIdProperty)
            {
                if (_idProperty != null)
                {
                    throw new MappingException(string.Format(
                        "Attempt to add id property {0} but already have property {1} registered "
                        + "as id. Check your mapping configuration!", property.FieldInfo, _idProperty.FieldInfo));
                }
                _idProperty = property;
		    }

		    if (property.IsVersionProperty)
            {
                if (_versionProperty != null)
                {
                    throw new MappingException(string.Format(
                        "Attempt to add version property {0} but already have property {1} registered "
                        + "as version. Check your mapping configuration!", property.FieldInfo,
                        _versionProperty.FieldInfo));
                }
                _versionProperty = property;
		    }
	    }

	    public void AddAssociation(Association association)
        {
		    _associations.Add(association);
	    }

	    public IPersistentProperty GetPersistentProperty(string name)
	    {
	        return _properties.FirstOrDefault(property => property.Name == name);
	    }

        public Type Type
        {
	        get { return _information.Type; }
        }

	    public Object TypeAlias
        {
	        get
	        {
	            var alias = Attribute.GetCustomAttribute(Type, typeof (TypeAliasAttribute)) as TypeAliasAttribute;
	            return alias == null ? null : StringUtils.HasText(alias.Name) ? alias.Name : null;
	        }
        }

	    public ITypeInformation TypeInformation
        {
	        get { return _information; }
        }

	    public void DoWithProperties(IPropertyHandler handler) 
        {
		    AssertUtils.ArgumentNotNull(handler, "handler");
		    foreach(var property in _properties)
            {
			    if (!property.IsTransient && !property.IsAssociation)
                {
				    handler.DoWithPersistentProperty(property);
			    }
		    }
	    }

	    public void DoWithAssociations(IAssociationHandler handler)
        {
		    AssertUtils.ArgumentNotNull(handler, "handler");
		    foreach(var association in _associations)
            {
			    handler.DoWithAssociation(association);
		    }
	    }

	    public void Verify()
        {
	    }

	    sealed class AssociationComparer: IComparer<Association>
        {
		    private readonly IComparer<IPersistentProperty> _comparer;

		    public AssociationComparer(IComparer<IPersistentProperty> comparer)
            {
			    AssertUtils.ArgumentNotNull(comparer, "comparer");
			    _comparer = comparer;
		    }

		    public int Compare(Association left, Association right)
            {
			    return _comparer.Compare(left.Inverse, right.Inverse);
		    }
	    }
    }
}
