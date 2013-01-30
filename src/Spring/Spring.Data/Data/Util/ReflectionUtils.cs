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

namespace Spring.Data.Util
{
    /// <summary>
    /// Simple utility class for working with the reflection API and handling
    /// reflection exceptions.
    /// </summary>
    /// <author>Juergen Hoeller</author>
    /// <author>Rob Harrop</author>
    /// <author>Rod Johnson</author>
    /// <author>Costin Leau</author>
    /// <author>Sam Brannen</author>
    /// <author>Chris Beams</author>
    /// <author>Thomas Trageser</author>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Creates a new class instance with a generic and an inner type
        /// </summary>
        /// <param name="generic">The Type of the class to create</param>
        /// <param name="innerType">the generic type of the class</param>
        /// <param name="args">arguments that are required to create an instance of class</param>
        /// <returns></returns>
        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            var specificType = generic.MakeGenericType(new [] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        /// <summary>
        /// Invoke the given callback on all properties in the target class, going up the
        /// class hierarchy to get all declared properties.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fc"></param>
        /// <param name="ff"></param>
	    public static void DoWithProperties(Type type, IPropertyCallback fc, IPropertyFilter ff)
        {
		    // Keep backing up the inheritance hierarchy.
		    do {
			    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			    foreach(var property in properties) 
                {
				    // Skip static and final properties.
				    if (ff != null && !ff.Matches(property)) 
                    {
					    continue;
				    }
					fc.DoWith(property);
			    }
			    type = type.BaseType;
		    }
		    while (type != null && type == typeof(object));
	    }
    }

    /// <summary>
    /// Callback interface invoked on each property in the hierarchy.
    /// </summary>
	public interface IPropertyCallback 
    {
        /// <summary>
        /// Perform an operation using the given property.
        /// </summary>
        /// <param name="propertyInfo"></param>
		void DoWith(PropertyInfo propertyInfo);
	}

	/// <summary>
	///  Callback optionally used to filter properties to be operated on by a property callback.
	/// </summary>
	public interface IPropertyFilter 
    {
        /// <summary>
        /// Determine whether the given property matches.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        bool Matches(PropertyInfo propertyInfo);
	}

}
