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
using System.Text;
using Spring.Util;

namespace Spring.Data.Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <author></author>
    public abstract class ParentTypeAwareTypeInformation : TypeDiscoverer
    {
	    protected readonly TypeDiscoverer _parent;

        /// <summary>
        /// Creates a new {@link ParentTypeAwareTypeInformation}.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <param name="dictionarymap"></param>
	    protected ParentTypeAwareTypeInformation(Type type, TypeDiscoverer parent, IDictionary<Type, Type> dictionarymap)
            : base(type, dictionarymap)
        {
		    _parent = parent;
	    }

        /// <summary>
        /// Considers the parent's type variable map before invoking the super class method.
        /// </summary>
	    public override IDictionary<Type, Type> TypeVariableDictionary
        {
	        get { return _parent == null ? base.TypeVariableDictionary : _parent.TypeVariableDictionary; }
        }

        /// <summary>
        /// Creates <see cref="ITypeInformation"/> for the given <see cref="Type"/>.
        /// </summary>
	    public override ITypeInformation CreateInfo(Type fieldType)
        {
		    if (_parent != null && _parent.Type == fieldType)
            {
			    return _parent;
		    }

		    return base.CreateInfo(fieldType);
	    }

        public override string ToString()
        {
            return string.Format("[Parent = {0}, Type = {1}]", _parent, _type);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
		    if (!base.Equals(obj))
            {
			    return false;
		    }

		    if (GetType() != obj.GetType())
            {
			    return false;
		    }

		    var that = (ParentTypeAwareTypeInformation) obj;
		    return _parent == null ? that._parent == null : _parent.Equals(that._parent);
	    }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
		    return base.GetHashCode() + 31 * ObjectUtils.NullSafeHashCode(_parent);
	    }

    }
}
