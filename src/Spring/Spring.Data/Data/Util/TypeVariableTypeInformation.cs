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
using Spring.Util;

namespace Spring.Data.Util
{
    /// <summary>
    /// Special <see cref="TypeDiscoverer"/> to determine the actual type for a generic parameter.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class TypeVariableTypeInformation : ParentTypeAwareTypeInformation
    {
	    private readonly Type _variable;
	    private readonly Type _owningType;

        /// <summary>
        /// Creates a bew {@link TypeVariableTypeInformation} for the given {@link TypeVariable} owning {@link Type} 
        /// and parent {@link TypeDiscoverer}.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="owningType"></param>
        /// <param name="parent"></param>
        /// <param name="typeVariableDictionary"></param>
        public TypeVariableTypeInformation(Type variable, Type owningType, TypeDiscoverer parent,
                                           IDictionary<Type, Type> typeVariableDictionary)
            : base(variable, parent, typeVariableDictionary)
        {
            AssertUtils.ArgumentNotNull(variable, "variable");

            _variable = variable;
            _owningType = owningType;
        }

	    public override Type Type
        {
	        get
	        {
	            int index = GetIndex(_variable);

	            if (_owningType.IsGenericType && index != -1)
	            {
	                Type genericArgument = _owningType.GetGenericArguments()[index];
	                return ResolveType(genericArgument);
	            }

	            return ResolveType(_variable);
	        }
        }

        /// <summary>
        /// Returns the index of the type parameter binding the given <see cref="Type"/>.
        /// </summary>
	    private int GetIndex(Type variable)
        {
		    Type rawType = ResolveType(_owningType);
		    Type[] genericArguments = rawType.GetGenericArguments();

		    for (int i = 0; i < genericArguments.Length; i++)
            {
			    if (variable == genericArguments[i])
                {
				    return i;
			    }
		    }

		    return -1;
	    }

	    public override bool Equals(object obj)
        {
		    if (!base.Equals(obj))
			    return false;

		    var that = (TypeVariableTypeInformation) obj;
	        return ObjectUtils.NullSafeEquals(_owningType, that._owningType) &&
	               ObjectUtils.NullSafeEquals(_variable, _variable);
        }

	    public override int GetHashCode()
        {
		    int result = base.GetHashCode();
		    result += 31 * ObjectUtils.NullSafeHashCode(_owningType);
            result += 31 * ObjectUtils.NullSafeHashCode(_variable);
		    return result;
	    }

    }
}
