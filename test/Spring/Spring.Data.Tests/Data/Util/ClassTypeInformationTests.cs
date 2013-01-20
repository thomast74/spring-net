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
using System.Reflection;
using NUnit.Framework;
using Spring.Data.Mapping;

namespace Spring.Data.Util
{
    /// <summary>
    /// Unit tests for <see cref="ClassTypeInformation"/>.
    /// </summary>
    [TestFixture]
    [Category(TestCategory.Unit)]
    public class ClassTypeInformationTests
    {
        [Test]
	    public void DiscoversTypeForSimpleGenericField()
        {
		    ITypeInformation discoverer = ClassTypeInformation.From<ConcreteType>();

            Assert.That(discoverer.Type, Is.EqualTo(typeof(ConcreteType)));
		    
            ITypeInformation content = discoverer.GetProperty("content");
		    
            Assert.That(content.Type, Is.EqualTo(typeof(string)));
            Assert.That(content.ComponentType, Is.Null);
            Assert.That(content.DictionaryValueType, Is.Null);            
	    }

        [Test]
	    public void DiscoversTypeForNestedGenericField()
        {
		    ITypeInformation discoverer = ClassTypeInformation.From<ConcreteWrapper>();

            Assert.That(discoverer.Type, Is.EqualTo(typeof(ConcreteWrapper)));

		    ITypeInformation wrapper = discoverer.GetProperty("wrapped");

            Assert.That(wrapper.Type, Is.EqualTo(typeof(GenericType<string,object>)));

		    ITypeInformation content = wrapper.GetProperty("content");

            Assert.That(content.Type, Is.EqualTo(typeof(string)));
            Assert.That(discoverer.GetProperty("wrapped").GetProperty("content").Type, Is.EqualTo(typeof(string)));
            Assert.That(discoverer.GetProperty("wrapped.content").Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void DiscoversBoundType()
        {
		    ITypeInformation information = ClassTypeInformation.From<GenericTypeWithBound<Person>>();

            Assert.That(information.GetProperty("person").Type, Is.EqualTo(typeof(Person)));
	    }

        [Test]
	    public void DiscoversBoundTypeForSpecialization() 
        {
		    ITypeInformation information = ClassTypeInformation.From<SpecialGenericTypeWithBound>();

            Assert.That(information.GetProperty("person").Type, Is.EqualTo(typeof(SpecialPerson)));
	    }

        [Test]
	    public void DiscoversBoundTypeForNested()
        {
            ITypeInformation information =
                ClassTypeInformation.From<AnotherGenericType<Person, GenericTypeWithBound<Person>>>();

            Assert.That(information.GetProperty("nested").Type, Is.EqualTo(typeof(GenericTypeWithBound<Person>)));
            Assert.That(information.GetProperty("nested.person").Type, Is.EqualTo(typeof(Person)));
	    }

        [Test]
	    public void DiscoversArraysAndCollections()
        {
		    ITypeInformation information = ClassTypeInformation.From<StringCollectionContainer>();
		    ITypeInformation property = information.GetProperty("array");

            Assert.That(property.ComponentType.Type, Is.EqualTo(typeof(string)));

		    Type type = property.Type;
            Assert.That(type, Is.EqualTo(typeof(string[])));
            Assert.That(type.IsArray, Is.True);

		    property = information.GetProperty("foo");
            Assert.That(property.Type, Is.EqualTo(typeof(ICollection<string>[])));
            Assert.That(property.ComponentType.Type, Is.EqualTo(typeof(ICollection<string>)));
            Assert.That(property.ComponentType.ComponentType.Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void DiscoversMapValueType()
        {
		    ITypeInformation information = ClassTypeInformation.From<StringDictionaryContainer>();
		    ITypeInformation genericDictionary = information.GetProperty("genericDictionary");

            Assert.That(genericDictionary.Type, Is.EqualTo(typeof(Dictionary<string,string>)));
            Assert.That(genericDictionary.DictionaryValueType.Type, Is.EqualTo(typeof(string)));

		    ITypeInformation map = information.GetProperty("dictionary");

            Assert.That(map.Type, Is.EqualTo(typeof(Dictionary<string, DateTime>)));
            Assert.That(map.DictionaryValueType.Type, Is.EqualTo(typeof(DateTime)));
	    }

        [Test]
	    public void TypeInfoDoesNotEqualForGenericTypesWithDifferentParent()
        {
		    ITypeInformation first = ClassTypeInformation.From<ConcreteWrapper>();
		    ITypeInformation second = ClassTypeInformation.From<AnotherConcreteWrapper>();

            Assert.That(first.GetProperty("wrapped"), Is.Not.EqualTo(second.GetProperty("wrapped")));
	    }

        [Test]
	    public void HandlesPropertyFieldMismatchCorrectly()
        {
		    ITypeInformation from = ClassTypeInformation.From<PropertyGetter>();

		    ITypeInformation property = from.GetProperty("_name");

            Assert.That(property, Is.Not.Null);
            Assert.That(property.Type, Is.EqualTo(typeof(string)));

		    property = from.GetProperty("Name");
            Assert.That(property, Is.Null);
	    }

        [Test]
	    public void ReturnsSameInstanceForCachedClass()
        {
		    ITypeInformation info = ClassTypeInformation.From<PropertyGetter>();

            Assert.That(ClassTypeInformation.From<PropertyGetter>(), Is.SameAs(info));
	    }

        [Test]
	    public void ResolvesWildCardTypeCorrectly()
        {
		    ITypeInformation information = ClassTypeInformation.From<ClassWithWildCardBound>();
		    ITypeInformation property = information.GetProperty("wildcard");

            Assert.That(property.IsCollectionLike, Is.True);
            Assert.That(property.ComponentType.Type, Is.EqualTo(typeof(string)));

		    property = information.GetProperty("complexWildcard");

		    Assert.That(property.IsCollectionLike, Is.True);

		    ITypeInformation component = property.ComponentType;

            Assert.That(component.IsCollectionLike, Is.True);
            Assert.That(component.ComponentType.Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void ResolvesTypeParametersCorrectly()
        {
		    ITypeInformation information = ClassTypeInformation.From<ConcreteType>();
		    ITypeInformation superTypeInformation = information.GetSuperTypeInformation(typeof(GenericType<string,object>));
		    IList<ITypeInformation> parameters = superTypeInformation.TypeArguments;

            Assert.That(parameters.Count, Is.EqualTo(2));
            Assert.That(parameters[0].Type, Is.EqualTo(typeof(string)));
            Assert.That(parameters[1].Type, Is.EqualTo(typeof(object)));
	    }

        [Test]
	    public void ResolvesNestedInheritedTypeParameters()
        {
		    ITypeInformation information = ClassTypeInformation.From<SecondExtension>();
		    ITypeInformation superTypeInformation = information.GetSuperTypeInformation(typeof(Base<string>));
		    IList<ITypeInformation> parameters = superTypeInformation.TypeArguments;

            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters[0].Type, Is.EqualTo(typeof(string)));
	    }

        [Test]
	    public void DiscoversMethodParameterTypesCorrectly()
        {
		    ITypeInformation information = ClassTypeInformation.From<SecondExtension>();
		    MethodInfo method = typeof(SecondExtension).GetMethod("Foo");
		    IList<ITypeInformation> informations = information.GetParameterTypes(method);
		    ITypeInformation returnTypeInformation = information.GetReturnType(method);

            Assert.That(informations.Count, Is.EqualTo(1));
            Assert.That(informations[0].Type, Is.EqualTo(typeof(Base<GenericWrapper<long>>)));
            Assert.That(informations[0], Is.EqualTo(returnTypeInformation));
	    }

        [Test]
	    public void DiscoversImplementationBindingCorrectlyForString()
        {
		    ITypeInformation information = ClassTypeInformation.From<ITypedClient>();
		    MethodInfo method = typeof(ITypedClient).GetMethod("StringMethod");
		    ITypeInformation parameterType = information.GetParameterTypes(method)[0];
		    ITypeInformation stringInfo = ClassTypeInformation.From<StringImplementation>();
		    
            Assert.That(parameterType.IsAssignableFrom(stringInfo), Is.True);
            Assert.That(stringInfo.GetSuperTypeInformation(typeof (IGenericInterface<string>)),
                        Is.TypeOf(parameterType.GetType()));
            Assert.That(parameterType.IsAssignableFrom(ClassTypeInformation.From<LongImplementation>()), Is.False);
            Assert.That(
                parameterType.IsAssignableFrom(
                    ClassTypeInformation.From<StringImplementation>()
                                               .GetSuperTypeInformation(typeof (IGenericInterface<string>))), Is.True);
	    }

        [Test]
	    public void DiscoversImplementationBindingCorrectlyForLong()
        {
		    ITypeInformation information = ClassTypeInformation.From<ITypedClient>();
		    MethodInfo method = typeof(ITypedClient).GetMethod("LongMethod");
		    ITypeInformation parameterType = information.GetParameterTypes(method)[0];

            Assert.That(parameterType.IsAssignableFrom(ClassTypeInformation.From<StringImplementation>()), Is.False);
            Assert.That(parameterType.IsAssignableFrom(ClassTypeInformation.From<LongImplementation>()), Is.True);
            Assert.That(
                parameterType.IsAssignableFrom(
                    ClassTypeInformation.From<StringImplementation>()
                                               .GetSuperTypeInformation(typeof(IGenericInterface<string>))), Is.False);
        }

        [Test]
	    public void ReturnsComponentTypeForMultiDimensionalArrayCorrectly() 
        {
		    ITypeInformation information = ClassTypeInformation.From<string[][]>();

            Assert.That(information.Type, Is.EqualTo(typeof(string[][])));
            Assert.That(information.ComponentType.Type, Is.EqualTo(typeof(string[])));
            Assert.That(information.ActualType.ActualType.Type, Is.EqualTo(typeof(string)));
	    }



	    class StringDictionaryContainer : DictionaryContainer<string>
        {
	    }

	    class DictionaryContainer<T>
        {
		    Dictionary<string, T> genericDictionary;
		    Dictionary<string, DateTime> dictionary;
	    }

	    class StringCollectionContainer : CollectionContainer<string>
        {
	    }

	    class CollectionContainer<T>    
        {
		    protected T[] array;
            protected ICollection<T>[] foo;
            protected ISet<string> set;
	    }

	    class GenericTypeWithBound<T> where T : Person
        {
		    T person;
	    }

	    class AnotherGenericType<T, TS> where T : Person
                                        where TS : GenericTypeWithBound<T>
        {
		    TS nested;
	    }

	    class SpecialGenericTypeWithBound : GenericTypeWithBound<SpecialPerson>
        {

	    }

	    abstract class SpecialPerson : Person 
        {
	        private SpecialPerson(int ssn, string firstName, string lastName)
	            : base(ssn, firstName, lastName)
	        {
	        }
        }

	    class GenericType<T, TS>
        {
		    long index;
		    T content;
	    }

	    class ConcreteType : GenericType<string, object>
        {
	    }

	    class GenericWrapper<TS> 
        {
		    GenericType<TS, object> wrapped;
	    }

	    class ConcreteWrapper : GenericWrapper<string>
        {
	    }

	    class AnotherConcreteWrapper : GenericWrapper<long> 
        {
	    }

	    class PropertyGetter 
        {
		    private string _name;

		    public char[] Name
            {
		        get { return _name.ToCharArray(); }
            }
	    }

	    class ClassWithWildCardBound 
        {
            IList<string> wildcard;
		    IList<IEnumerable<string>> complexWildcard;
	    }

	    class Base<T>
        {
	    }

	    class FirstExtension<T> : Base<string> 
        {
		    public Base<GenericWrapper<T>> Foo(Base<GenericWrapper<T>> param)
            {
			    return null;
		    }
	    }

	    class SecondExtension : FirstExtension<long> 
        {
	    }

	    interface IGenericInterface<T>
        {
	    }

	    interface ITypedClient
        {
		    void StringMethod(IGenericInterface<string> param);
		    void LongMethod(IGenericInterface<long> param);
	    }

	    class StringImplementation : IGenericInterface<string>
        {
	    }

	    class LongImplementation : IGenericInterface<long>
        {
	    }

    }
}
