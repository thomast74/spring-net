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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Spring.Data.Util
{
    /// <summary>
    /// <see cref="ITypeInformation"/> for a plain class.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class ClassTypeInformation : TypeDiscoverer
    {
        private static readonly IDictionary<Type, ITypeInformation> Cache =
            new ConcurrentDictionary<Type, ITypeInformation>();

        /// <summary>
        /// Creates <see cref="ClassTypeInformation"/> for the given type.
        /// </summary>
	    public ClassTypeInformation(Type type)
            : base(type, null)
        {
        }

        /// <summary>
        /// Creates <see cref="ClassTypeInformation"/> for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeVariableMap"></param>
        public ClassTypeInformation(Type type, IDictionary<Type, Type> typeVariableMap)
            : base(type, typeVariableMap)
        {
        }

        /// <summary>
        /// Simple factory method to easily create new instances of <see cref="ClassTypeInformation"/>.
        /// </summary>
        public static ITypeInformation From<TType>()
        {
            return From(typeof(TType));
        }

        public static ITypeInformation From(Type type)
        {
            if (Cache.ContainsKey(type))
            {
                var cachedTypeInfo = Cache[type];
                if (cachedTypeInfo != null)
                {
                    return cachedTypeInfo;
                }
            }

            var result = new ClassTypeInformation(type);
            Cache.Add(type, result);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="ITypeInformation"/> from the given method's return type.
        /// </summary>
        public static ITypeInformation FromReturnTypeOf(MethodInfo method)
        {
            var result = new ClassTypeInformation(method.DeclaringType);
            return result.CreateInfo(method.ReturnType);
        }

    }
}
