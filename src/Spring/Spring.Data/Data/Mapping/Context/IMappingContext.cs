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
using Spring.Data.Mapping.Model;
using Spring.Data.Util;

namespace Spring.Data.Mapping.Context
{
    /// <summary>
    /// This interface defines the overall context including all known PersistentEntity instances and methods to obtain
    /// instances on demand. it is used internally to establish associations between entities and also at runtime to obtain
    /// entities by name.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Jon Brisbin</author>
    /// <author>Graeme Rocher</author>
    /// <author>Thomas Trageser</author>
    public interface IMappingContext
    {
        /// <summary>
        /// Returns all <see cref="IMutablePersistentEntity"/>s held in the context.
        /// </summary>
	    ICollection<IMutablePersistentEntity> PersistentEntities { get; }

        /// <summary>
        /// Returns a <see cref="IMutablePersistentEntity"/> for the given <see cref="Type"/>.
        /// Will return <code>null</code> for types that are considered simple ones.
        /// </summary>
        /// <typeparam name="T">Type to get presistent entity from</typeparam>
        IMutablePersistentEntity GetPersistentEntity<T>();

        /// <summary>
        /// Returns a <see cref="IMutablePersistentEntity"/> for the given <see cref="Type"/>.
        /// Will return <code>null</code> for types that are considered simple ones.
        /// </summary>
        /// <param name="type">must not be <code>null</code></param>
        IMutablePersistentEntity GetPersistentEntity(Type type);

        /// <summary>
        /// Returns a <see cref="IPersistentEntity"/> for the given <see cref="ITypeInformation"/>. 
        /// Will return <code>null</code> for types that are considered simple ones.
        /// </summary>
        /// <param name="type">must not be <code>null</code></param>
        IMutablePersistentEntity GetPersistentEntity(ITypeInformation type);

        /// <summary>
        /// Returns the <see cref="IMutablePersistentEntity"/> mapped by the given <see cref="IMutablePersistentEntity"/>.
        /// </summary>
        /// <returns>
        /// the <see cref="IMutablePersistentEntity"/> mapped by the given <see cref="IMutablePersistentEntity"/> or 
        /// <code>null</code> if no <see cref="IMutablePersistentEntity"/> exists for it or the 
        /// <see cref="IMutablePersistentEntity"/> does not refer to an entity (the type of the property is 
	    /// considered simple see <see cref="SimpleTypeHolder.IsSimpleType"/>).
        /// </returns>
        IMutablePersistentEntity GetPersistentEntity(IPersistentProperty persistentProperty);

        /// <summary>
        /// Returns all <see cref="IPersistentProperty"/>s for the given path expression based on 
        /// the given <see cref="PropertyPath"/>.
        /// </summary>
        IPersistentPropertyPath<T> GetPersistentPropertyPath<T>(PropertyPath propertyPath)
            where T : IPersistentProperty;
    }
}
