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

namespace Spring.Data.Domain
{
    /// <summary>
    /// Abstract interface for pagination information.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IPageable
    {
        /// <summary>
        /// Returns the page to be returned.
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Returns the number of items to be returned.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Returns the offset to be taken according to the underlying page and page size.
        /// </summary>
        int Offset { get; }

        /// <summary>
        /// Returns the sorting parameters.
        /// </summary>
        Sort Sort { get; }
    }
}
