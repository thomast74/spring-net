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
using Spring.Collections.Generic;
using Spring.Util;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Simple container to hold a set of types to be considered simple types.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class SimpleTypeHolder
    {
        private static readonly ISet<Type> Defaults = new HashedSet<Type>
                                                          {
                                                              typeof (bool),
                                                              typeof (bool[]),
                                                              typeof (long),
                                                              typeof (long[]),
                                                              typeof (short),
                                                              typeof (short[]),
                                                              typeof (int),
                                                              typeof (int[]),
                                                              typeof (byte),
                                                              typeof (byte[]),
                                                              typeof (float),
                                                              typeof (float[]),
                                                              typeof (double),
                                                              typeof (double[]),
                                                              typeof (decimal),
                                                              typeof (decimal[]),
                                                              typeof (char),
                                                              typeof (char[]),
                                                              typeof (Boolean),
                                                              typeof (Int64),
                                                              typeof (Int16),
                                                              typeof (Int32),
                                                              typeof (Byte),
                                                              typeof (Single),
                                                              typeof (Double),
                                                              typeof (Decimal),
                                                              typeof (Char),
                                                              typeof (String),
                                                              typeof (DateTime),
                                                              typeof (Enum)
                                                          };
                                                              

	    private readonly ISet<Type> _simpleTypes;

        /// <summary>
        /// Creates a new <see cref="SimpleTypeHolder"/> containing the default types.
        /// </summary>
        public SimpleTypeHolder() : this(new HashedSet<Type>(), true)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SimpleTypeHolder"/> to carry the given custom simple types. Registration of default simple types
        /// can be deactivated by passing <code>false</code> for registerDefaults.
        /// </summary>
        /// <param name="customSimpleTypes">must not be <code>null</code></param>
        /// <param name="registerDefaults"><code>true</code> to register default types defined in <see cref="Defaults"/></param>
	    public SimpleTypeHolder(ISet<Type> customSimpleTypes, bool registerDefaults)
        {
		    AssertUtils.ArgumentNotNull(customSimpleTypes, "customSimpleTypes");

		    _simpleTypes = new HashedSet<Type>(customSimpleTypes);

		    if (registerDefaults)
            {
			    _simpleTypes.AddAll(Defaults);
		    }
	    }

        /// <summary>
        /// Copy constructor to create a new <see cref="SimpleTypeHolder"/> that carries the given additional custom simple types.
        /// </summary>
        /// <param name="customSimpleTypes">must not be <code>null</code></param>
        /// <param name="source">must not be <code>null</code></param>
	    public SimpleTypeHolder(Set<Type> customSimpleTypes, SimpleTypeHolder source)
        {
		    AssertUtils.ArgumentNotNull(customSimpleTypes, "customSimpleTypes must not be null");
		    AssertUtils.ArgumentNotNull(source, "source must not be null");

		    _simpleTypes = new HashedSet<Type>(customSimpleTypes);
		    _simpleTypes.AddAll(source._simpleTypes);
	    }

        /// <summary>
        /// Returns whether the given type is considered a simple one.
        /// </summary>
        /// <param name="type">must not be null</param>
        /// <returns></returns>
	    public bool IsSimpleType(Type type) 
        {
		    AssertUtils.ArgumentNotNull(type, "type must not be null");

		    if (typeof(object) == type)
			    return true;

            foreach (Type clazz in _simpleTypes)
            {
			    if (type == clazz || clazz.IsAssignableFrom(type)) 
                {
				    return true;
			    }
		    }
		    return false;
	    }
    }
}
