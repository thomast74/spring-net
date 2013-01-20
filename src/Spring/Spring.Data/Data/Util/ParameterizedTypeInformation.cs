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
using Spring.Collections.Generic;
using Spring.Util;

namespace Spring.Data.Util
{
    /// <summary>
    /// Base class for all types that include parameterization of some kind. Crucial as we have to 
    /// take note of the parent class we will have to resolve generic parameters against.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class ParameterizedTypeInformation : ParentTypeAwareTypeInformation
    {
        /// <summary>
        /// Creates a new {@link ParameterizedTypeInformation} for the given {@link Type} and parent {@link TypeDiscoverer}.
        /// </summary>
        /// <param name="type">must not be <code>null</code></param>
        /// <param name="parent">must not be <code>null</code></param>
        public ParameterizedTypeInformation(Type type, TypeDiscoverer parent)
            : base(type, parent, null)
        {
        }

        public override ITypeInformation DictionaryValueType
        {
            get
            {
                if (Type == typeof(IDictionary<,>))
                {
                    Type[] arguments = _type.GetGenericArguments();
                    return CreateInfo(arguments[1]);
                }

                Type rawType = Type;

                var supertypes = new HashedSet<Type>();
                if (rawType.BaseType != null)
                    supertypes.Add(rawType.BaseType);
                supertypes.AddAll(rawType.GetInterfaces());

                foreach (Type supertype in supertypes)
                {
                    Type rawSuperType = ResolveType(supertype);
                    if (typeof(IDictionary<,>).IsAssignableFrom(rawSuperType))
                    {
                        Type[] arguments = supertype.GetGenericArguments();
                        return CreateInfo(arguments[1]);
                    }
                }

                return base.DictionaryValueType;
            }
        }

	    public override IList<ITypeInformation> TypeArguments
        {
	        get
	        {
	            IList<ITypeInformation> result = new List<ITypeInformation>();

	            foreach(Type argument in _type.GetGenericArguments())
	            {
	                result.Add(CreateInfo(argument));
	            }

	            return result;
	        }
        }

	    public override bool IsAssignableFrom(ITypeInformation target)
        {
		    if (Equals(target))
			    return true;

		    Type rawType = Type;
		    Type rawTargetType = target.Type;

		    if (!rawType.IsAssignableFrom(rawTargetType))
			    return false;

	        ITypeInformation otherTypeInformation = rawType == rawTargetType
	                                                    ? target
	                                                    : target.GetSuperTypeInformation(rawType);

		    IList<ITypeInformation> myParameters = TypeArguments;
		    IList<ITypeInformation> typeParameters = otherTypeInformation.TypeArguments;

		    if (myParameters.Count != typeParameters.Count)
			    return false;

		    for (int i = 0; i < myParameters.Count; i++)
            {
			    if (!myParameters[i].IsAssignableFrom(typeParameters[i]))
				    return false;
		    }

		    return true;
	    }

	    public override ITypeInformation ComponentType
        {
	        get { return CreateInfo(_type.GetGenericArguments()[0]); }
        }

	    public override bool Equals(object obj)
        {
		    if (obj == this)
            {
			    return true;
		    }

		    if (!(obj is ParameterizedTypeInformation))
            {
			    return false;
		    }

		    var that = (ParameterizedTypeInformation) obj;

		    if (IsResolvedCompletely() && that.IsResolvedCompletely())
            {
			    return Type == that.Type;
		    }

		    return base.Equals(obj);
	    }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result += 31 * ObjectUtils.NullSafeHashCode(_type);
            return result;
        }

	    private bool IsResolvedCompletely()
        {
		    Type[] types = _type.GetGenericArguments();

		    if (types.Length == 0)
			    return false;

		    foreach(var type in types)
            {
			    var info = CreateInfo(type);

			    if (info is ParameterizedTypeInformation)
                {
				    if (!((ParameterizedTypeInformation) info).IsResolvedCompletely()) 
					    return false;
			    }

			    if (!(info is ClassTypeInformation))
				    return false;
		    }

		    return true;
	    }
    }
}
