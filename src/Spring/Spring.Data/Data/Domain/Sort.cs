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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spring.Util;

namespace Spring.Data.Domain
{
    /// <summary>
    /// Sort option for queries. You have to provide at least a list of properties to sort for that must not include
    /// <code>null</code> or empty strings. The direction defaults to <see cref="DefaultDirection"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public class Sort : IEnumerable<Sort.Order>
    {
	    public static readonly Direction DefaultDirection = Direction.Asc;
	    private readonly List<Order> _orders;

        /// <summary>
        /// Creates a new {@link Sort} instance using the given {@link Order}s.
        /// </summary>
        /// <param name="orders">must not be <code>null</code></param>
	    public Sort(params Order[] orders)
            : this(orders.ToList())
        {
	    }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="orders">must not be <code>null</code> and contain at least one item</param>
	    public Sort(List<Order> orders)
        {
            AssertUtils.ArgumentNotNull(orders, "orders");
            AssertUtils.IsTrue(orders.Count > 0);

		    _orders = orders;
	    }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance. Order defaults to <see cref="Direction.Asc"/>.
        /// </summary>
        /// <param name="properties">must not be <code>null</code> or contain <code>null</code> or empty strings</param>
	    public Sort(params string[] properties) 
            : this(DefaultDirection, properties)
        {
	    }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="direction">defaults to <see cref="DefaultDirection"/> (for <code>null</code> cases, too)</param>
        /// <param name="properties">must not be <code>null</code> or contain <code>null</code> or empty strings</param>
	    public Sort(Direction direction, params string[] properties)
            : this(direction, properties == null ? new List<string>() : properties.ToList())
        {
	    }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="properties"></param>
	    public Sort(Direction direction, List<String> properties)
        {
            AssertUtils.ArgumentNotNull(properties, "properties");
            AssertUtils.IsTrue(properties.Count > 0);

		    _orders = new List<Order>(properties.Count);

		    foreach (var property in properties)
            {
			    _orders.Add(new Order(direction, property));
		    }
	    }

        /// <summary>
        /// Returns a new {@link Sort} consisting of the {@link Order}s of the current {@link Sort} combined 
        /// with the given ones.
        /// </summary>
        /// <param name="sort"></param>
	    public Sort And(Sort sort)
        {
		    if (sort == null)
			    return this;

		    var these = new List<Order>(_orders);
            these.AddRange(sort);

            return new Sort(these);
	    }

        /// <summary>
        /// Returns the order registered for the given property.
        /// </summary>
        /// <param name="property"></param>
	    public Order GetOrderFor(string property)
        {
            return this.FirstOrDefault(order => order.Property.Equals(property));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _orders.GetEnumerator();
        }

        public IEnumerator<Order> GetEnumerator()
        {
            return _orders.GetEnumerator();
        }

	    public override bool Equals(object obj) 
        {
		    if (this == obj) return true;
		    if (!(obj is Sort)) return false;

		    var that = (Sort) obj;

		    return _orders.Equals(that._orders);
	    }

	    public override int GetHashCode()
        {
		    int result = 17;
		    result = 31 * result + _orders.GetHashCode();
		    return result;
	    }

	    public override string ToString()
        {
		    return StringUtils.CollectionToCommaDelimitedString(_orders);
	    }

        /// <summary>
        /// Enumeration for sort directions.
        /// </summary>
        /// <author>Oliver Gierke</author>
        /// <author>Thomas Trageser</author>
	    public enum Direction
	    {
	        Asc,
	        Desc
        }

        /// <summary>
        /// PropertyPath implements the pairing of an <see cref="Direction"/> and a property. It is used to
        /// provide input for <see cref="Sort"/>.
        /// </summary>
        /// <author>Oliver Gierke</author>
        /// <author>Thomas Trageser</author>
        [Serializable]
	    public class Order
        {
		    private readonly Direction _direction;
		    private readonly string _property;

            /// <summary>
            /// Creates a new <see cref="Order"/> instance. if order is <code>null</code> then order defaults to
            /// <see cref="Sort.DefaultDirection"/>.
            /// </summary>
            /// <param name="direction">
            /// can be <code>null</code>, will default to <see cref="Sort.DefaultDirection"/>
            /// </param>
            /// <param name="property">must not be <code>null</code> or empty.</param>
		    public Order(Direction direction, String property)
            {
                AssertUtils.ArgumentHasText(property, "property");

			    _direction = direction;
			    _property = property;
		    }

            /// <summary>
            /// Creates a new <see cref="Order"/> instance. Takes a single property. Direction defaults to
            /// <see cref="Sort.DefaultDirection"/>.
            /// </summary>
		    public Order(string property) 
                : this(DefaultDirection, property)
            {
		    }

            [Obsolete]
		    public static List<Order> Create(Direction direction, IEnumerable<string> properties)
            {
                return properties.Select(property => new Order(direction, property)).ToList();
            }

            /// <summary>
            /// Returns the order the property shall be sorted for.
            /// </summary>
		    public Direction Direction
            {
                get { return _direction; }
            }

            /// <summary>
            /// Returns the property to order for.
            /// </summary>
		    public string Property
            {
                get { return _property; }
            }

            /// <summary>
            /// Returns whether sorting for this property shall be ascending.
            /// </summary>
		    public bool IsAscending
            {
                get { return _direction.Equals(Direction.Asc); }
            }

            /// <summary>
            /// Returns a new <see cref="Order"/> with the given <see cref="Order"/>.
            /// </summary>
            /// <param name="order"></param>
		    public Order With(Direction order)
            {
			    return new Order(order, _property);
		    }

            /// <summary>
            /// Returns a new <see cref="Sort"/> instance for the given properties.
            /// </summary>
            /// <param name="properties"></param>
		    public Sort WithProperties(params string[] properties)
            {
			    return new Sort(_direction, properties);
		    }

		    public override int GetHashCode()
            {
			    int result = 17;
			    result = 31 * result + _direction.GetHashCode();
			    result = 31 * result + _property.GetHashCode();
			    return result;
		    }

		    public override bool Equals(object obj) 
            {
			    if (this == obj) return true;
			    if (!(obj is Order)) return false;

			    var that = (Order) obj;

			    return _direction.Equals(that._direction) && _property.Equals(that._property);
		    }

		    public override string ToString()
            {
			    return string.Format("{0}: {1}", _property, _direction);
		    }
	    }
    
    }
}
