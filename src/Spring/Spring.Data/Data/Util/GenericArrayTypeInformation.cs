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

namespace Spring.Data.Util
{
    /// <summary>
    /// Special <see cref="TypeDiscoverer"/> handling <see cref="Type.IsArray"/>s.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class GenericArrayTypeInformation : ParentTypeAwareTypeInformation
    {
        /// <summary>
        /// Creates a new <see cref="GenericArrayTypeInformation"/> for the given <see cref="GenericArrayTypeInformation"/>
        /// and <see cref="TypeDiscoverer"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
	    public GenericArrayTypeInformation(Type type, TypeDiscoverer parent)
            : base(type, parent, parent.TypeVariableDictionary)
        {
	    }

        /// <summary>
        /// Returns the component type for <see cref="IEnumerable{T}"/>s or the key type for
        /// <see cref="Dictionary{TKey,TValue}"/>s.
        /// </summary>
        public override ITypeInformation ComponentType
        {
	        get
	        {
                return CreateInfo(Type.GetElementType());
	        }
        }
    }
}
