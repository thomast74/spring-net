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

using Spring.Context;
using Spring.Util;

namespace Spring.Data.Mapping.Context
{
    /// <summary>
    /// Base implementation of an <see cref="ApplicationEventArgs"/> refering to a <see cref="IPersistentEntity"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Jon Brisbin</author>
    /// <author>Thomas Trageser</author>
    public class MappingContextEvent : ApplicationEventArgs
    {
	    private readonly IMappingContext _source;
	    private readonly IPersistentEntity _entity;

        /// <summary>
        /// Creates a new<see cref="MappingContextEvent"/> for the given 
        /// <see cref="IMappingContext"/> and <see cref="IPersistentEntity"/>.
        /// </summary>
        /// <param name="source">must not be <code>null</code>.</param>
        /// <param name="entity">must not be <code>null</code>.</param>
	    public MappingContextEvent(IMappingContext source, IPersistentEntity entity)
        {
		    AssertUtils.ArgumentNotNull(source, "source");
		    AssertUtils.ArgumentNotNull(entity, "entity");

		    _source = source;
		    _entity = entity;
	    }

        /// <summary>
        /// Returns the <see cref="IPersistentEntity"/> the event was created for.
        /// </summary>
	    public IPersistentEntity PersistentEntity
        {
            get { return _entity; }
        }

        /// <summary>
        /// Returns whether the <see cref="MappingContextEvent"/> was triggered by the given 
        /// <see cref="IMappingContext"/>.
        /// </summary>
        /// <param name="context">the <see cref="IMappingContext"/> that potentially created the event.</param>
	    public bool WasEmittedBy(object context)
        {
		    return _source.Equals(context);
	    }
    }
}
