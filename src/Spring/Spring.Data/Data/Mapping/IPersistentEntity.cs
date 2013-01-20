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
using Spring.Data.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Represents a persistent entity.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Graeme Rocher</author>
    /// <author>Jon Brisbin</author>
    /// <author>Patryk Wasik</author>
    /// <author>Thomas Trageser</author>
    public interface IPersistentEntity
    {
        /// <summary>
        /// The entity name including any package prefix.
        /// </summary>
        /// <returns>must never return <code>null</code></returns>
	    string Name { get; }

        /// <summary>
        /// Returns the <see cref="PreferredConstructor"/> to be used to instantiate objects of 
        /// this <see cref="IPersistentEntity"/>.
        /// </summary>
        /// <returns>
        /// <code>null</code> in case no suitable constructor for automatic construction can be found. This usually
	    /// indicates that the instantiation of the object of tthat persistent entity is done through either a customer
	    /// {@link EntityInstantiator} or handled by custom conversion mechanisms entirely.
        /// </returns>
	    PreferredConstructor PersistenceConstructor { get; }

        /// <summary>
        /// Returns whether the given <see cref="IPersistentProperty"/> is referred to by a constructor argument of the
        /// <see cref="IPersistentProperty"/>. 
        /// </summary>
        /// <param name="property"></param>
        /// <returns>
        /// true if the given <see cref="IPersistentProperty"/> is referred to by a constructor
        /// argument or <code>false</code>
        /// if not or <code>null</code>.
        /// </returns>
        bool IsConstructorArgument(IPersistentProperty property);

        /// <summary>
        /// Returns whether the given <see cref="IPersistentProperty"/> is the id property of the entity.
        /// </summary>
        /// <param name="property"></param>
	    bool IsIdProperty(IPersistentProperty property);

        /// <summary>
        /// Returns whether the given <see cref="IPersistentProperty"/> is the version property of the entity.
        /// </summary>
        /// <param name="property"></param>
        bool IsVersionProperty(IPersistentProperty property);

        /// <summary>
        /// Returns the id property of the <see cref="IPersistentEntity"/>.
        /// Can be <code>null</code> in case this is an entity completely handled by a custom conversion.
        /// </summary>
        /// <returns>the id property of the <see cref="IPersistentEntity"/>.</returns>
	    IPersistentProperty IdProperty { get; }

        /// <summary>
        /// Returns the version property of the <see cref="IPersistentEntity"/>.
        /// Can be <code>null</code> in case no version property is available on the entity.
        /// </summary>
        /// <returns>the version property of the <see cref="IPersistentEntity"/>.</returns>
        IPersistentProperty VersionProperty { get; }

        /// <summary>
        /// Obtains a <see cref="IPersistentProperty"/> instance by name.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>the <see cref="IPersistentProperty"/> or <code>null</code> if it doesn't exist.</returns>
        IPersistentProperty GetPersistentProperty(string name);

        /// <summary>
        /// Returns whether the <see cref="IPersistentEntity"/> has an id property. 
        /// If this call returns <code>true</code>,
        /// <see cref="IdProperty"/> will return a non-<code>null</code> value. 
        /// </summary>
        /// <returns></returns>
	    bool HasIdProperty { get; }

        /// <summary>
        /// Returns whether the <see cref="IPersistentEntity"/> has a version property. 
        /// If this call returns <code>true</code>,
        /// <see cref="VersionProperty"/> will return a non-<code>null</code> value.
        /// </summary>
        /// <returns></returns>
	    bool HasVersionProperty { get; }

        /// <summary>
        /// Returns the resolved Java type of this entity.
        /// </summary>
        /// <returns>The underlying object type class for this entity</returns>
	    Type Type { get; }

        /// <summary>
        /// Returns the alias to be used when storing type information. Might be <code>null</code> 
        /// to indicate that there was no alias defined through the mapping metadata.
        /// </summary>
        /// <returns></returns>
	    object TypeAlias { get; }

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/> backing this <see cref="IPersistentEntity"/>.
        /// </summary>
        ITypeInformation TypeInformation { get; }

        /// <summary>
        /// Applies the given <see cref="IPropertyHandler"/> to all 
        /// <see cref="IPersistentProperty"/>s contained in this <see cref="IPersistentEntity"/>.
        /// </summary>
        /// <param name="handler">must not be <code>null</code></param>
	    void DoWithProperties(IPropertyHandler handler);

        /// <summary>
        /// Applies the given {@link AssociationHandler} to all {@link Association} contained in this 
        /// <see cref="IPersistentEntity"/>.
        /// </summary>
        /// <param name="handler">must not be <code>null</code></param>
	    void DoWithAssociations(IAssociationHandler handler);
    }
}
