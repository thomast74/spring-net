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
using System.Reflection;
using Spring.Data.Util;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Helper class to find a <see cref="PreferredConstructor"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class PreferredConstructorDiscoverer
    {
	    private readonly IParameterNameDiscoverer _nameDiscoverer = new LocalVariableTableParameterNameDiscoverer();

	    private readonly PreferredConstructor _constructor;

        /// <summary>
        /// Creates a new <see cref="PreferredConstructorDiscoverer"/> for the given type.
        /// </summary>
        /// <param name="type">must not be <code>null</code></param>
	    public PreferredConstructorDiscoverer(Type type)
            : this(ClassTypeInformation.From(type), null)
        {
	    }

        /// <summary>
        /// Creates a new <see cref="PreferredConstructorDiscoverer"/> for the given <see cref="IPersistentEntity"/>.
        /// </summary>
        /// <param name="entity">must not be <code>null</code></param>
        public PreferredConstructorDiscoverer(IPersistentEntity entity)
            : this(entity.TypeInformation, entity)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PreferredConstructorDiscoverer"/> for the given type.
        /// </summary>
        /// <param name="type">must not be <code>null</code></param>
        /// <param name="entity"></param>
        public PreferredConstructorDiscoverer(ITypeInformation type, IPersistentEntity entity)
        {
            bool noArgConstructorFound = false;
            int numberOfArgConstructors = 0;
            Type rawOwningType = type.Type;

            foreach (var constructor in rawOwningType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                PreferredConstructor preferredConstructor = BuildPreferredConstructor(constructor, type, entity);

                // Explicitly defined constructor trumps all
                if (preferredConstructor.IsExplicitlyAnnotated)
                {
                    this._constructor = preferredConstructor;
                    return;
                }

                // No-arg constructor trumps custom ones
                if (this._constructor == null || preferredConstructor.IsNoArgConstructor)
                {
                    this._constructor = preferredConstructor;
                }

                if (preferredConstructor.IsNoArgConstructor)
                {
                    noArgConstructorFound = true;
                }
                else
                {
                    numberOfArgConstructors++;
                }
            }

            if (!noArgConstructorFound && numberOfArgConstructors > 1)
            {
                _constructor = null;
            }
        }

        private PreferredConstructor BuildPreferredConstructor(ConstructorInfo constructor,
                                                               ITypeInformation typeInformation,
                                                               IPersistentEntity entity)
        {
            var parameterTypes = typeInformation.GetParameterTypes(constructor);

            if (parameterTypes.Count == 0)
            {
                return new PreferredConstructor(constructor);
            }

            var parameterNames = _nameDiscoverer.GetParameterNames(constructor);
            var parameters = new Parameter[parameterTypes.Count];

            for (var i = 0; i < parameterTypes.Count; i++)
            {
                var name = parameterNames == null ? null : parameterNames[i];
                var type = parameterTypes[i];

                parameters[i] = new Parameter(name, type, entity);
            }

            return new PreferredConstructor(constructor, parameters);
        }

        /// <summary>
        /// Returns the discovered <see cref="PreferredConstructor"/>.
        /// </summary>
        public PreferredConstructor Constructor
        {
            get { return _constructor; }
        }
    }
}
