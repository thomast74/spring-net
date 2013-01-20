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
using System.Text;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Interface to discover parameter names for methods and constructors.
    /// <p>
    /// Parameter name discovery is not always possible, but various strategies are
    /// available to try, such as looking for debug information that may have been
    /// emitted at compile time, and looking for argname annotation values optionally
    /// accompanying AspectJ annotated methods.
    /// </p>
    /// </summary>
    /// <author>Rod Johnson</author>
    /// <author>Adrian Colyer</author>
    /// <author>Thomas Trageser</author>
    public interface IParameterNameDiscoverer
    {
        /// <summary>
        /// Return parameter names for this method,
        /// or <code>null</code> if they cannot be determined.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns>
        /// an array of parameter names if the names can be resolved,
        /// or <code>null</code> if they cannot
        /// </returns>
        string[] GetParameterNames(MethodInfo methodInfo);

        /// <summary>
        /// Return parameter names for this constructor,
        /// or <code>null</code> if they cannot be determined.
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <returns>
        /// an array of parameter names if the names can be resolved,
        /// or <code>null</code> if they cannot
        /// </returns>
        string[] GetParameterNames(ConstructorInfo constructorInfo);

    }
}
