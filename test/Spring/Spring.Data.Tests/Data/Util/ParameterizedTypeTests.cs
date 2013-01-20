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
using NUnit.Framework;

namespace Spring.Data.Util
{
    /// <summary>
    /// Unit tests for <see cref="ClassTypeInformation"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [TestFixture]
    public class ParameterizedTypeTests
    {
        private Type one;

        [SetUp]
	    public void SetUp()
        {
            one = typeof(Localized<string>);
        }

        [Test]
	    public void considersTypeInformationsWithDifferingParentsNotEqual()
        {
		    TypeDiscoverer stringParent = new TypeDiscoverer(typeof(string), null);
		    TypeDiscoverer objectParent = new TypeDiscoverer(typeof(object), null);

		    ParameterizedTypeInformation first = new ParameterizedTypeInformation(one, stringParent);
		    ParameterizedTypeInformation second = new ParameterizedTypeInformation(one, objectParent);

		    Assert.That(first, Is.Not.EqualTo(second));
	    }

	    [Test]
	    public void considersTypeInformationsWithSameParentsNotEqual()
        {
		    TypeDiscoverer stringParent = new TypeDiscoverer(typeof(string), null);

		    ParameterizedTypeInformation first = new ParameterizedTypeInformation(one, stringParent);
		    ParameterizedTypeInformation second = new ParameterizedTypeInformation(one, stringParent);

		    Assert.That(first, Is.EqualTo(second));
	    }

	    [Test]
	    public void ResolvesDictonaryValueTypeCorrectly()
	    {
	        ITypeInformation type = ClassTypeInformation.From<Foo>();
		    ITypeInformation propertyType = type.GetProperty("param");

		    Assert.That(propertyType.GetProperty("value").Type, Is.EqualTo(typeof(string)));
            Assert.That(propertyType.DictionaryValueType.Type, Is.EqualTo(typeof(string)));

		    propertyType = type.GetProperty("param2");

		    Assert.That(propertyType.GetProperty("value").Type, Is.EqualTo(typeof(string)));
            Assert.That(propertyType.DictionaryValueType.Type, Is.EqualTo(typeof(DateTime)));
	    }

	    class Localized<T> : Dictionary<DateTime, T>
        {
		    T value;
	    }

	    class Localized2<T> : Dictionary<T, DateTime>
        {
		    T value;
	    }

	    class Foo 
        {
		    Localized<string> param;
		    Localized2<string> param2;
	    }

    }
}
