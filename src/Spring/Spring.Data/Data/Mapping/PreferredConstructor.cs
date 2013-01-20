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
using System.Linq;
using System.Reflection;
using Spring.Data.Annotation;
using Spring.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Value object to encapsulate the constructor to be used when mapping persistent data to objects.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Jon Brisbin</author>
    /// <author>Thomas Trageser</author>
    public class PreferredConstructor
    {
	    private readonly ConstructorInfo _constructorInfo;
        private readonly IList<Parameter> _parameters;

        /// <summary>
        /// Creates a new <see cref="PreferredConstructor"/> from the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">must not be null</param>
        /// <param name="parameters">list of parameters from </param>
	    public PreferredConstructor(ConstructorInfo constructor, params Parameter[] parameters)
        {
		    AssertUtils.ArgumentNotNull(constructor, "constructor must not be null");

		    _constructorInfo = constructor;
            _parameters = parameters.ToList();
	    }

        /// <summary>
        /// Returns the underlying <see cref="ConstructorInfo"/>.
        /// </summary>
        public ConstructorInfo Constructor
        {
            get { return _constructorInfo; }
        }

        /// <summary>
        /// Returns the <see cref="Parameter"/>s of the constructor.
        /// </summary>
	    public IList<Parameter> GetParameters()
        {
            return _parameters;
        }

        /// <summary>
        /// Returns whether the constructor has <see cref="Parameter"/>s.
        /// </summary>
        /// <returns>
        /// <code>true</code> if constructor has parameters, 
        /// <code>false</code> if it is an emptyless constructor
        /// </returns>
        public bool HasParameters
        {
            get { return (_parameters.Count > 0); }
        }

        /// <summary>
        /// Returns whether the constructor does not have any arguments.
        /// </summary>
        /// <returns>
        /// <code>true</code> if it is the default constructor,
        /// <code>false</code> if constructor has parameters
        /// </returns>
        public bool IsNoArgConstructor
        {
            get { return _parameters.Count == 0; }
        }

        /// <summary>
        /// Returns whether the constructor was explicitly selected (by <see cref="PersistenceConstructorAttribute"/>).
        /// </summary>
        /// <returns></returns>
        public bool IsExplicitlyAnnotated
        {
            get
            {
                var annotated =
                    Attribute.GetCustomAttribute(_constructorInfo, typeof (PersistenceConstructorAttribute)) as
                    PersistenceConstructorAttribute;
                return annotated != null;
            }
        }

        /// <summary>
        /// Returns whether the given <see cref="IPersistentProperty"/> is referenced in a constructor argument of the
        /// <see cref="IPersistentEntity"/> backing this {@link MappedConstructor}. 
        /// </summary>
        /// <param name="property">must not be <code>null</code></param>
	    public bool IsConstructorParameter(IPersistentProperty property)
        {
		    AssertUtils.ArgumentNotNull(property, "property must not be null");

            return _parameters.Any(parameter => parameter.Maps(property));
        }

        /// <summary>
        /// Returns whether the given <see cref="Parameter"/> is one referring to an enclosing class.
        /// That is in case the class this <see cref="PreferredConstructor"/> belongs to is a member class
        /// actually. If that's the case the compiler creates a first constructor argument of the enclosing class type.
        /// </summary>
        /// <param name="parameter">must not be <code>null</code></param>
        /// <returns>
        /// </returns>
	    public bool IsEnclosingClassParameter(Parameter parameter)
        {
		    AssertUtils.ArgumentNotNull(parameter, "parameter must not be null");

		    if (_parameters.Count == 0) 
			    return false;

		    return _parameters[0].Equals(parameter) && parameter.IsEnclosingClassParameter;
	    }
    }
}
