
using System;
using System.Reflection;
using Spring.Data.Util;
using Spring.Objects.Factory.Attributes;
using Spring.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Value object to represent constructor parameters.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class Parameter
    {
        private readonly string _name;
        private readonly ITypeInformation _type;
        private readonly string _key;
        private readonly IPersistentEntity _entity;

        private bool? _enclosingClassCache;

        /// <summary>
        /// Creates a new <see cref="Parameter"/> with the given name and <see cref="ParameterInfo"/>.
        /// Will insprect the annotations for an <see cref="ValueAttribute"/> annotation to lookup a key or an SpEL
        /// expression to be evaluated. 
        /// </summary>
        /// <param name="name">the name of the parameter, can be <code>null</code></param>
        /// <param name="type">must not be <code>null</code></param>
        /// <param name="entity">can be <code>null</code>.</param>
        public Parameter(string name, ITypeInformation type, IPersistentEntity entity)
        {
            AssertUtils.ArgumentNotNull(type, "type");

            _name = name;
            _type = type;
            _key = GetValue();
            _entity = entity;
        }

        private string GetValue()
        {
            var valueAttribute = Attribute.GetCustomAttribute(_type.Type, typeof(ValueAttribute)) as ValueAttribute;
            return valueAttribute == null ? string.Empty : valueAttribute.Expression;
        }

        /// <summary>
        /// Returns the name of the parameter or <code>null</code> if none was given.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Returns the <see cref="ParameterInfo"/> of the parameter.
        /// </summary>
        public ITypeInformation Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Returns the raw type behind <see cref="ITypeInformation"/>
        /// </summary>
        public Type RawType
        {
            get
            {
                return _type.Type;
            }
        }

        /// <summary>
        /// Returns the key to be used when looking up a source data structure to populate the actual parameter value.
        /// </summary>
        public string SpelExpression
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Returns whether the constructor parameter is equipped with a SpEL expression.
        /// </summary>
        public bool HasSpelExpression
        {
            get { return !string.IsNullOrEmpty(SpelExpression); }
        }

        /// <summary>
        /// Returns whether the <see cref="Parameter"/> maps the given <see cref="IPersistentProperty"/>.
        /// </summary>
        /// <param name="property"></param>
        public bool Maps(IPersistentProperty property)
        {
            IPersistentProperty referencedProperty = _entity == null ? null : _entity.GetPersistentProperty(_name);
            return property == null ? false : property.Equals(referencedProperty);
        }

        public bool IsEnclosingClassParameter
        {
            get
            {
                if (!_enclosingClassCache.HasValue)
                {
                    Type owningType = _entity.Type;
                    _enclosingClassCache = owningType.IsNested && _type.Type.Equals(owningType);
                }
                return _enclosingClassCache.Value;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is Parameter))
                return false;

            var that = (Parameter)obj;

            bool nameEquals = _name == null ? that._name == null : _name.Equals(that._name);
            bool keyEquals = _key == null ? that._key == null : _key.Equals(that._key);
            bool typeEquals = ObjectUtils.NullSafeEquals(_type, that._type);
            bool entityEquals = _entity == null ? that._entity == null : _entity.Equals(that._entity);

            return nameEquals && keyEquals && typeEquals && entityEquals;
        }

        public override int GetHashCode()
        {
            var result = 17;

            result += 31 * ObjectUtils.NullSafeHashCode(_name);
            result += 31 * ObjectUtils.NullSafeHashCode(_key);
            result += 31 * ObjectUtils.NullSafeHashCode(_type);
            result += 31 * ObjectUtils.NullSafeHashCode(_entity);

            return result;
        }
    }
}
