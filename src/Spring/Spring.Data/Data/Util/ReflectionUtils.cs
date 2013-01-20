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
        /// Invoke the given callback on all fields in the target class, going up the
        /// class hierarchy to get all declared fields.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fc"></param>
        /// <param name="ff"></param>
	    public static void DoWithFields(Type type, IFieldCallback fc, IFieldFilter ff)
        {
		    // Keep backing up the inheritance hierarchy.
		    do {
			    FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			    foreach(FieldInfo field in fields) 
                {
				    // Skip static and final fields.
				    if (ff != null && !ff.Matches(field)) 
                    {
					    continue;
				    }
					fc.DoWith(field);
			    }
			    type = type.BaseType;
		    }
		    while (type != null && type == typeof(object));
	    }
    }

    /// <summary>
    /// Callback interface invoked on each field in the hierarchy.
    /// </summary>
	public interface IFieldCallback 
    {
        /// <summary>
        /// Perform an operation using the given field.
        /// </summary>
        /// <param name="fieldInfo"></param>
		void DoWith(FieldInfo fieldInfo);
	}

	/// <summary>
	///  Callback optionally used to filter fields to be operated on by a field callback.
	/// </summary>
	public interface IFieldFilter 
    {
        /// <summary>
        /// Determine whether the given field matches.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
		bool Matches(FieldInfo fieldInfo);

        /// <summary>
        /// Determine whether the given field matches.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        bool Matches(PropertyInfo propertyInfo);
	}

}
