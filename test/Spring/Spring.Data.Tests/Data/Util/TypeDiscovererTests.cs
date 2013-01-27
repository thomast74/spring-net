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
using NUnit.Framework;

namespace Spring.Data.Util
{
    /// <summary>
    /// Unit tests for <see cref="TypeDiscoverer"/>
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [TestFixture]
    [Category(TestCategory.Unit)]
    public class TypeDiscovererTests
    {

        IDictionary<Type, Type> _firstDictionary;
        IDictionary<Type, Type> _secondDictionary;


        [SetUp]
        public void SetUp()
        {
            _firstDictionary = new Dictionary<Type, Type> {{typeof (int), typeof (Int64)}};
            _secondDictionary = new Dictionary<Type, Type> {{typeof (int), typeof (Int16)}};
        }

        [Test]
	    public void IsNotEqualIfTypesDiffer()
        {
		    var objectTypeInfo = new TypeDiscoverer(typeof(object));
		    var stringTypeInfo = new TypeDiscoverer(typeof(string));

            Assert.That(objectTypeInfo.Equals(stringTypeInfo), Is.False);
	    }

        [Test]
	    public void IsNotEqualIfTypeVariableDictionaryDiffer()
        {
            Assert.That(_firstDictionary.Equals(_secondDictionary), Is.False);

		    var first = new TypeDiscoverer(typeof(int), _firstDictionary);
		    var second = new TypeDiscoverer(typeof(int), _secondDictionary);

            Assert.That(first.Equals(second), Is.False);
	    }

        [Test]
	    public void DealsWithTypesReferencingThemselves()
        {
            var information = new ClassTypeInformation(typeof(SelfReferencing));
		    var first = information.GetProperty("parent").DictionaryValueType;
            var second = first.GetProperty("dictionary").DictionaryValueType;

            Assert.That(first, Is.EqualTo(second));
	    }

        [Test]
	    public void DealsWithTypesReferencingThemselvesInAMap()
        {
            var information = new ClassTypeInformation(typeof(SelfReferencingDictionary));
            var dictionaryValueType = information.GetProperty("dictionary").DictionaryValueType;

            Assert.That(dictionaryValueType, Is.EqualTo(information));
	    }

        [Test]
	    public void ReturnsComponentAndValueTypesForDictionaryExtensions()
        {
            var discoverer = new TypeDiscoverer(typeof(ICustomDictionary));

            Assert.That(discoverer.ComponentType.Type, Is.EqualTo(typeof(string)));
            Assert.That(discoverer.DictionaryValueType.Type, Is.EqualTo(typeof(int)));
	    }

        [Test]
	    public void ReturnsComponentTypeForCollectionExtension()
        {
            var discoverer = new TypeDiscoverer(typeof(ICustomCollection));

            Assert.That(discoverer.ComponentType.Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void ReturnsComponentTypeForArrays() 
        {
		    var discoverer = new TypeDiscoverer(typeof(string[]));

            Assert.That(discoverer.Type, Is.EqualTo(typeof(string[])));
            Assert.That(discoverer.ComponentType.Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void DiscoveresConstructorParameterTypesCorrectly() 
        {
            var discoverer = new TypeDiscoverer(typeof(GenericConstructors));
            var constructor = typeof(GenericConstructors).GetConstructors()[0];
            var types = discoverer.GetParameterTypes(constructor);

            Assert.That(types.Count, Is.EqualTo(2));
            Assert.That(types[0].Type, Is.EqualTo(typeof(IList<string>)));
            Assert.That(types[0].ComponentType.Type, Is.EqualTo(typeof(string)));

            Assert.That(types[1].Type, Is.EqualTo(typeof(int)));
        }

        [Test]
	    public void ReturnsNullForComponentAndValueTypesForRawDictionaries() 
        {
            var discoverer = new TypeDiscoverer(typeof(Hashtable));

            Assert.That(discoverer.ComponentType, Is.Null);
            Assert.That(discoverer.DictionaryValueType, Is.Null);
	    }

        [Test]
	    public void DoesNotConsiderTypeImplementingEnumeratorACollection() 
        {
            var discoverer = new TypeDiscoverer(typeof(Person));
		    ITypeInformation reference = ClassTypeInformation.From<Address>();
		    ITypeInformation addresses = discoverer.GetProperty("addresses");

            Assert.That(addresses.IsCollectionLike, Is.False);
            Assert.That(addresses.ComponentType, Is.EqualTo(reference));

		    ITypeInformation addressEnumerable = discoverer.GetProperty("addressEnumerable");

            Assert.That(addressEnumerable.IsCollectionLike, Is.True);
            Assert.That(addressEnumerable.ComponentType, Is.EqualTo(reference));
	    }



	    class Person
        {
		    Addresses addresses;
		    IList<Address> addressEnumerable;
	    }

	    class Addresses : IEnumerable<Address>
        {
            public IEnumerator<Address> GetEnumerator()
            {
                return null;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return null;
            }
        }

	    class Address
        {
	    }

	    class SelfReferencing 
        {
		    IDictionary<string, SelfReferencingDictionary> parent;
	    }

	    class SelfReferencingDictionary
        {
		    IDictionary<string, SelfReferencingDictionary> dictionary;
	    }

	    interface ICustomDictionary : IDictionary<string, int>
        {
	    }

	    interface ICustomCollection : ICollection<string>
        {
	    }

	    public class GenericConstructors
	    {
		    public GenericConstructors(IList<string> first, int second)
		    {
		    }
	    }

    }
}
