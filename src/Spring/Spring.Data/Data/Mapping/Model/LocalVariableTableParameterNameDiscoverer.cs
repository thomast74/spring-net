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
using System.Linq;
using System.Reflection;
using Common.Logging;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Implementation of <see cref="IParameterNameDiscoverer"/> that uses the LocalVariableTable
    /// information in the method attributes to discover parameter names. Returns
    /// <code>null</code> if the class file was compiled without debug information.
    /// </summary>
    /// <author>Adrian Colyer</author>
    /// <author>Costin Leau</author>
    /// <author>Juergen Hoeller</author>
    /// <author>Thomas Trageser</author>
    public class LocalVariableTableParameterNameDiscoverer : IParameterNameDiscoverer
    {
	    private static readonly ILog Logger = LogManager.GetLogger<LocalVariableTableParameterNameDiscoverer>();

	    // the cache uses a nested index (value is a map) to keep the top level cache relatively small in size
        private readonly ConcurrentDictionary<Type, IDictionary<MemberInfo, string[]>> _parameterNamesCache =
            new ConcurrentDictionary<Type, IDictionary<MemberInfo, string[]>>();


	    public string[] GetParameterNames(MethodInfo method)
        {
		    Type declaringType = method.DeclaringType;
		    IDictionary<MemberInfo, string[]> map;
            _parameterNamesCache.TryGetValue(declaringType, out map);
		    if (map == null) 
            {
			    // initialize cache
			    map = InspectType(declaringType);
			    _parameterNamesCache.TryAdd(declaringType, map);
		    }
		    if (map.ContainsKey(method))
            {
			    return map[method];
		    }
		    return null;
	    }

	    public string[] GetParameterNames(ConstructorInfo constructor)
	    {
	        Type declaringType = constructor.DeclaringType;
		    IDictionary<MemberInfo, string[]> map;
            _parameterNamesCache.TryGetValue(declaringType, out map);
		    if (map == null) 
            {
			    // initialize cache
			    map = InspectType(declaringType);
			    _parameterNamesCache.TryAdd(declaringType, map);
		    }
		    if (map.ContainsKey(constructor)) 
            {
			    return map[constructor];
		    }

		    return null;
	    }

        private IDictionary<MemberInfo, string[]> InspectType(Type typeToInspect)
        {
            IDictionary<MemberInfo, string[]> memberParameterMap = new Dictionary<MemberInfo, string[]>();
            Logger.Debug(m => m("Inspect Type " + typeToInspect.Name));

            foreach (
                var constructor in
                    typeToInspect.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var parameterNames = from p in constructor.GetParameters()
                                     select p.Name;

                Logger.Debug(m => m("Found constructor {0}({1})" + constructor.Name, parameterNames));
                memberParameterMap.Add(constructor, parameterNames.ToArray());
            }

            foreach (
                var method in
                    typeToInspect.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var parameterNames = from p in method.GetParameters()
                                     select p.Name;

                Logger.Debug(m => m("Found method {0}({1})" + method.Name, parameterNames));
                memberParameterMap.Add(method, parameterNames.ToArray());
            }

            return memberParameterMap;
        }

    }
}
