#region License

/*
 * Copyright © 2002-2011 the original author or authors.
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

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Interface capturing mutator methods for <see cref="IPersistentEntity"/>s.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IMutablePersistentEntity : IPersistentEntity
    {
        /// <summary>
        /// Adds a <see cref="IPersistentProperty"/> to the entity.
        /// </summary>
        /// <param name="property"></param>
        void AddPersistentProperty(IPersistentProperty property);

        /// <summary>
        /// Adds an <see cref="Association"/> to the entity.
        /// </summary>
        /// <param name="association"></param>
        void AddAssociation(Association association);

        /// <summary>
        /// Callback method to trigger validation of the <see cref="IPersistentEntity"/>.
        /// As <see cref="IMutablePersistentEntity"/> is not immutable there might be some 
        /// verification steps necessary after the object has reached is final state.
        /// </summary>
        /// <exception cref="MappingException"></exception>
	    void Verify();
    }
}
