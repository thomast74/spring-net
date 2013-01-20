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

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Callback interface to implement functionality to be applied to a 
    /// collection of <see cref="Association"/>s.
    /// </summary>
    /// <author>Jon Brisbin</author>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IAssociationHandler
    {
        /// <summary>
        /// Processes the given <see cref="Association"/>.
        /// </summary>
        /// <param name="association"></param>
        void DoWithAssociation(Association association);
    }
}
