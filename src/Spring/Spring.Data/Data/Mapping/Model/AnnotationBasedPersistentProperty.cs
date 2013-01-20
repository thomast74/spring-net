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
using Spring.Data.Annotation;
using Spring.Objects.Factory.Attributes;

namespace Spring.Data.Mapping.Model
{
    /// <summary>
    /// Special <see cref="IPersistentProperty"/> that takes annotations at a property into account.
    /// </summary>
    public abstract class AnnotationBasedPersistentProperty : AbstractPersistentProperty
    {
	    private readonly ValueAttribute _value;
        private readonly AutowiredAttribute _autowired;

        /// <summary>
        /// Creates a new <see cref="AnnotationBasedPersistentProperty"/>.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="owner"></param>
        /// <param name="simpleTypeHolder"></param>
        public AnnotationBasedPersistentProperty(FieldInfo fieldInfo, IPersistentEntity owner,
                                                 SimpleTypeHolder simpleTypeHolder)
            : base(fieldInfo, owner, simpleTypeHolder)
        {
            _value = Attribute.GetCustomAttribute(_fieldInfo, typeof (ValueAttribute)) as ValueAttribute;
            _autowired = Attribute.GetCustomAttribute(_fieldInfo, typeof (AutowiredAttribute)) as AutowiredAttribute;
        }

        /// <summary>
        /// Inspects a potentially available <see cref="ValueAttribute"/> annotation at the property and returns the 
        /// <see cref="String"/> value of is.
        /// </summary>
	    public override string SpelExpression
        {
            get { return _value == null ? null : _value.Expression; }
        }

        /// <summary>
        /// Considers plain transient fields, fields annotated with <see cref="TransientAttribute"/>,
        /// <see cref="ValueAttribute"/> or <see cref="AutowiredAttribute"/> as transient.
        /// </summary>
        public override bool IsTransient
        {
            get
            {
                var transient = Attribute.GetCustomAttribute(_fieldInfo, typeof(TransientAttribute));

                bool isTransient = base.IsTransient || transient != null;
                return isTransient || _value != null || _autowired != null;
            }
        }

        public override bool IsIdProperty
        {
            get { return Attribute.GetCustomAttribute(_fieldInfo, typeof (IdAttribute)) != null; }
        }

        public override bool IsVersionProperty
        {
            get { return Attribute.GetCustomAttribute(_fieldInfo, typeof (VersionAttribute)) != null; }
        }

        /// <summary>
        ///  Considers the property an <see cref="Association"/> if it is annotated with <see cref="ReferenceAttribute"/>.
        /// </summary>
        /// <returns></returns>
        public override bool IsAssociation
        {
            get
            {
                if (IsTransient)
                {
                    return false;
                }
                if (Attribute.GetCustomAttribute(_fieldInfo, typeof(ReferenceAttribute)) != null)
                {
                    return true;
                }

                return false;
            }
        }

    }
}
