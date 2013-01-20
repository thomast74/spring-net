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
using Spring.Objects.Factory.Attributes;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Graeme Rocher</author>
    /// <author>Jon Brisbin</author>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IPersistentProperty
    {
        /// <summary>
        /// Returns the owner as <see cref="IPersistentEntity"/>
        /// </summary>
        /// <returns></returns>
        IPersistentEntity Owner { get; }

        /// <summary>
        /// The name of the property
        /// </summary>
	    string Name { get; }

        /// <summary>
        /// The type of the property
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// The <see cref="ITypeInformation"/> for the 
        /// </summary>
        ITypeInformation TypeInformation { get; }

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/>  if the property references a 
        /// <see cref="IPersistentEntity"/>. Will return <code>null</code> in case it refers to a 
        /// simple type. Will return <see cref="IEnumerable{T}"/>'s component type or the
        /// <see cref="IDictionary{TKey,TValue}"/>'s value type transparently.
        /// </summary>
        IEnumerable<ITypeInformation> PersistentEntityTypes { get; }

        /// <summary>
        /// Returns the associated field info if available.
        /// Might return <code>null</code> in case there is no property info assignable.
        /// </summary>
        /// <returns>the field info to access the property value if available, otherwise <code>null</code>.</returns>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// Returns the Spring Expression Language value from the <see cref="ValueAttribute"/>
        /// </summary>
	    string SpelExpression { get; }

        /// <summary>
        /// TODO
        /// </summary>
        Association Association { get; }

        /// <summary>
        /// Returns whether the type of the <see cref="IPersistentProperty"/> is actually to be regarded 
        /// as <see cref="IPersistentEntity"/>
        /// in turn.
        /// </summary>
	    bool IsEntity { get; }

        /// <summary>
	    /// Returns whether the property is a <em>potential</em> identifier property of the owning
	    /// <see cref="IPersistentEntity"/>. This method is mainly used by 
	    /// <see cref="IPersistentEntity"/> implementation to discover id property candidates on
	    /// <see cref="IPersistentProperty"/> creation you should rather call
	    /// <see cref="IPersistentEntity.IsIdProperty"/> 
	    /// to determine whether the current property is the id property of that 
	    /// <see cref="IPersistentEntity"/> under consideration.
        /// </summary>
	    bool IsIdProperty { get; }

        /// <summary>
	    /// Returns whether the current property is a <em>potential</em> version property of the owning
	    /// <see cref="IPersistentEntity"/>. This method is mainly used by 
	    /// <see cref="IPersistentEntity"/> implementation to discover version property 
	    /// candidates on <see cref="IPersistentEntity"/> creation you should rather call
	    /// <see cref="IPersistentEntity.IsVersionProperty"/> to determine whether the 
	    /// current property is the version property of that <see cref="IPersistentEntity"/> 
	    /// under consideration.
        /// </summary>
	    bool IsVersionProperty { get; }

        /// <summary>
        /// Returns whether the property is a <see cref="IEnumerable{T}"/>, <see cref="IEnumerator{T}"/> or an array.
        /// </summary>
	    bool IsCollectionLike { get; }

        /// <summary>
        /// Returns whether the property is a <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
	    bool IsDictionary { get; }

        /// <summary>
        /// Returns whether the property is an array.
        /// </summary>
	    bool IsArray { get; }

        /// <summary>
        /// Returns whether the property is transient.
        /// </summary>
	    bool IsTransient { get; }

        /// <summary>
        /// TODO
        /// </summary>
	    bool ShallBePersisted { get; }

        /// <summary>
        /// Returns whether the property is an {@link Association}.
        /// </summary>
	    bool IsAssociation { get; }

        /// <summary>
        /// Returns the component type of the type if it is a <see cref="IEnumerable{T}"/>. 
        /// Will return the type of the key if the property is a <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        /// the component type, the map's key type or <code>null</code> if neither <see cref="IEnumerable{T}"/> nor
        /// <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
	    Type ComponentType { get; }

        /// <summary>
        /// Returns the raw type as it's pulled from from the reflected property.
        /// </summary>
        /// <returns>
        /// the raw type of the property.
        /// </returns>
	    Type RawType { get; }

        /// <summary>
        /// Returns the type of the values if the property is a <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>the map's value type or <code>null</code> if no <see cref="IDictionary{TKey,TValue}"/></returns>
	    Type DictionaryValueType { get; }
    }
}
