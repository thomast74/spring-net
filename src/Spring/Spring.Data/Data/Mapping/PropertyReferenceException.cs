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
using System.Runtime.Serialization;
using Spring.Data.Util;
using Spring.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Exception being thrown when creating <see cref="PropertyPath"/> instances.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [Serializable]
    public class PropertyReferenceException : ApplicationException
    {
        private const string ErrorTemplate = "No property {0} found for type {1}";

        private readonly string _propertyName;
	    private readonly ITypeInformation _type;
	    private readonly PropertyPath _baseProperty;

        /// <summary>
        /// Creates a new <see cref="PropertyReferenceException"/>.
        /// Should not be used.
        /// </summary>
        public PropertyReferenceException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="PropertyReferenceException"/>.
        /// Should not be used.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        public PropertyReferenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PropertyReferenceException"/>.
        /// Should not be used.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="inner">The inner exception.</param>
        public PropertyReferenceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PropertyReferenceException"/>.
        /// </summary>
        /// <param name="propertyName">the name of the property not found on the given type.</param>
        /// <param name="type">the type the property could not be found on.</param>
        /// <param name="baseProperty">the base {@link PropertyPath}.</param>
	    public PropertyReferenceException(String propertyName, ITypeInformation type, PropertyPath baseProperty)
        {
		    AssertUtils.ArgumentHasText(propertyName, "propertyName must not be null and empty");
		    AssertUtils.ArgumentNotNull(type, "type");

		    _propertyName = propertyName;
		    _type = type;
		    _baseProperty = baseProperty;
	    }

        /// <summary>
        /// Creates a new <see cref="PropertyReferenceException"/>.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected PropertyReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
        /// with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object 
        /// data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        /// about the source or destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("propertyName", _propertyName);
            info.AddValue("type", _type);
            info.AddValue("baseProperty", _baseProperty);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the name of the property not found.
        /// </summary>
        /// <returns>
        /// will not be <code>null</code> or empty.
        /// </returns>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Returns the type the property could not be found on.
        /// </summary>
        /// <returns>
        /// the Type
        /// </returns>
        public ITypeInformation Type
        {
            get { return _type; }
        }
        
        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string Message
        {
            get
            {
                if (_propertyName == null && _type == null)
                    return base.Message;

                return string.Format(ErrorTemplate, _propertyName, _type.Type.Name);
            }
        }

        /// <summary>
        /// Returns the <see cref="PropertyPath"/> which could be resolved so far.
        /// </summary>
        public PropertyPath BaseProperty
        {
            get { return _baseProperty; }
        }

    }
}
