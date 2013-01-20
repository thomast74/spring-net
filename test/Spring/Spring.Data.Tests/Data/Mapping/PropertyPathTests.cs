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

using NUnit.Framework;
using System.Collections.Generic;
using Spring.Collections.Generic;
using Spring.Data.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Unit tests for <see cref="PropertyPath"/>.
    /// </summary>
    [TestFixture]
    [Category(TestCategory.Unit)]
    public class PropertyPathTests
    {
        [Test]
	    public void ParsesSimplePropertyCorrectly() 
        {
		    PropertyPath reference = PropertyPath.From<Foo>("userName");

		    Assert.That(reference.HasNext, Is.False);
		    Assert.That(reference.DotPath, Is.EqualTo("userName"));
		    Assert.That(reference.OwningType, Is.EqualTo(ClassTypeInformation.From<Foo>()));
	    }

        [Test]
	    public void ParsesPathPropertyCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("userName");

		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
		    Assert.That(reference.DotPath, Is.EqualTo("user.name"));
	    }

        [Test]
	    public void PrefersLongerMatches()
        {
		    PropertyPath reference = PropertyPath.From<Sample>("userName");

		    Assert.That(reference.HasNext, Is.False);
		    Assert.That(reference.DotPath, Is.EqualTo("userName"));
	    }

        [Test]
	    public void TestName() 
        {
		    PropertyPath reference = PropertyPath.From<Sample2>("userName");

		    Assert.That(reference.Segment, Is.EqualTo("user"));
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
	    }

        [Test]
	    public void PrefersExplicitPaths()
        {
		    PropertyPath reference = PropertyPath.From<Sample>("user_name");

		    Assert.That(reference.Segment, Is.EqualTo("user"));
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesGenericsCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("usersName");

		    Assert.That(reference.Segment, Is.EqualTo("users"));
		    Assert.That(reference.IsCollection, Is.True);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesDictionaryCorrectly() 
        {
		    PropertyPath reference = PropertyPath.From<Bar>("userDictionaryName");

		    Assert.That(reference.Segment, Is.EqualTo("userDictionary"));
		    Assert.That(reference.IsCollection, Is.False);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesArrayCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("userArrayName");

		    Assert.That(reference.Segment, Is.EqualTo("userArray"));
		    Assert.That(reference.IsCollection, Is.True);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesInvalidCollectionCompountTypeProperl()
        {
		    try 
            {
			    PropertyPath.From<Bar>("usersMame");
			    Assert.Fail("Expected PropertyReferenceException!");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.PropertyName, Is.EqualTo("mame"));
			    Assert.That(e.BaseProperty, Is.EqualTo(PropertyPath.From<Bar>("users")));
		    }
	    }

        [Test]
	    public void HandlesInvalidDictionaryValueTypeProperly()
        {
		    try 
            {
			    PropertyPath.From<Bar>("userDictionaryMame");
			    Assert.Fail("Expected PropertyReferenceException!");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.PropertyName, Is.EqualTo("mame"));
			    Assert.That(e.BaseProperty, Is.EqualTo(PropertyPath.From<Bar>("userDictionary")));
		    }
	    }

        [Test]
	    public void FindsNested()
        {
		    PropertyPath from = PropertyPath.From<Sample>("bar.user.name");

		    Assert.That(from, Is.Not.Null);
		    Assert.That(from.LeafProperty, Is.EqualTo(PropertyPath.From<FooBar>("name")));
	    }

	    /**
	     * @see DATACMNS-45
	     */
        [Test]
	    public void HandlesEmptyUnderscoresCorrectly()
        {
		    PropertyPath propertyPath = PropertyPath.From<Sample2>("_foo");

		    Assert.That(propertyPath.Segment, Is.EqualTo("_foo"));
		    Assert.That(propertyPath.Type, Is.EqualTo(typeof(Foo)));

		    propertyPath = PropertyPath.From<Sample2>("_foo__email");

		    Assert.That(propertyPath.DotPath, Is.EqualTo("_foo._email"));
	    }

        [Test]
	    public void SupportsDotNotationAsWell()
        {
		    PropertyPath propertyPath = PropertyPath.From<Sample>("bar.userDictionary.name");

		    Assert.That(propertyPath, Is.Not.Null);
		    Assert.That(propertyPath.Segment, Is.EqualTo("bar"));
		    Assert.That(propertyPath.LeafProperty, Is.EqualTo(PropertyPath.From<FooBar>("name")));
	    }

        [Test]
	    public void ReturnsCorrectIteratorForSingleElement()
        {
		    PropertyPath propertyPath = PropertyPath.From<Foo>("userName");
		    var enumerator = propertyPath.GetEnumerator();

		    Assert.That(enumerator.MoveNext(), Is.True);
		    Assert.That(enumerator.Current, Is.EqualTo(propertyPath));
		    Assert.That(enumerator.MoveNext(), Is.False);
	    }

        [Test]
	    public void ReturnsCorrectIteratorForMultipleElement()
        {
		    PropertyPath propertyPath = PropertyPath.From<Bar>("user.name");
		    var enumerator = propertyPath.GetEnumerator();

		    Assert.That(enumerator.MoveNext(), Is.True);
            var current = enumerator.Current;
		    Assert.That(current, Is.EqualTo(propertyPath));
		    Assert.That(enumerator.MoveNext(), Is.True);
		    Assert.That(enumerator.Current, Is.EqualTo(propertyPath.Next));
		    Assert.That(enumerator.MoveNext(), Is.False);
	    }

	    /**
	     * @see DATACMNS-139
	     */
        [Test]
	    public void RejectsInvalidPropertyWithLeadingUnderscore() 
        {
		    try 
            {
			    PropertyPath.From<Foo>("_id");
                Assert.Fail("Expected PropertyReferenceException");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property _id"));
		    }
	    }

	    /**
	     * @see DATACMNS-139
	     */
        [Test]
	    public void RejectsNestedInvalidPropertyWithLeadingUnderscore() 
        {
		    try 
            {
			    PropertyPath.From<Sample2>("_foo_id");
                Assert.Fail("Expected PropertyReferenceException");
            }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property id"));
		    }
	    }

	    /**
	     * @see DATACMNS-139
	     */
        [Test]
	    public void RejectsNestedInvalidPropertyExplictlySplitWithLeadingUnderscore()
        {
		    try 
            {
			    PropertyPath.From<Sample2>("_foo__id");
                Assert.Fail("Expected PropertyReferenceException");
            }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property _id"));
		    }
	    }

	    /**
	     * @see DATACMNS 158
	     */
        [Test]
	    public void RejectsInvalidPathsContainingDigits()
        {
            Assert.That(delegate { PropertyPath.From<Foo>("PropertyThatWillFail4Sure"); },
                        Throws.TypeOf<PropertyReferenceException>());
        }

        [Test]
	    public void RejectsInvalidProperty()
        {
		    try 
            {
			    PropertyPath.From<Foo>("bar");
                Assert.Fail("Expected PropertyReferenceException");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.BaseProperty, Is.Null);
		    }
	    }

        [Test]
	    public void SamePathsEqual() 
        {
		    PropertyPath left = PropertyPath.From<Bar>("user.name");
		    PropertyPath right = PropertyPath.From<Bar>("user.name");
		    PropertyPath shortPath = PropertyPath.From<Bar>("user");

		    Assert.That(left, Is.EqualTo(right));
		    Assert.That(right, Is.EqualTo(left));
		    Assert.That(left, Is.Not.EqualTo(shortPath));
		    Assert.That(shortPath, Is.Not.EqualTo(left));
		    Assert.That(left, Is.Not.EqualTo(new object()));
	    }

        [Test]
	    public void HashCodeTests() 
        {
		    PropertyPath left = PropertyPath.From<Bar>("user.name");
		    PropertyPath right = PropertyPath.From<Bar>("user.name");
		    PropertyPath shortPath = PropertyPath.From<Bar>("user");

		    Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
		    Assert.That(left.GetHashCode(), Is.Not.EqualTo(shortPath.GetHashCode()));
	    }

	    /**
	     * @see DATACMNS-257
	     */
        [Test]
	    public void FindsAllUppercaseProperty() 
        {
		    PropertyPath path = PropertyPath.From<Foo>("UUID");

		    Assert.That(path, Is.Not.Null);
		    Assert.That(path.Segment, Is.EqualTo("UUID"));
	    }

	    /**
	     * @see DATACMNS-257
	     */
        [Test]
	    public void FindsNestedAllUppercaseProperty()
        {
		    PropertyPath path = PropertyPath.From<Sample2>("_foo.UUID");

		    Assert.That(path, Is.Not.Null);
		    Assert.That(path.Segment, Is.EqualTo("_foo"));
		    Assert.That(path.HasNext, Is.True);
		    Assert.That(path.Next.Segment, Is.EqualTo("UUID"));
	    }





	    private class Foo
        {
		    string userName;
		    string _email;
		    string UUID;
	    }

	    private class Bar
        {
		    private FooBar user;
		    private Set<FooBar> users;
		    private Dictionary<string, FooBar> userDictionary;
		    private FooBar[] userArray;
	    }

	    private class FooBar 
        {
		    private string name;
	    }

	    private class Sample
        {
		    private string userName;
		    private FooBar user;
		    private Bar bar;
	    }

	    private class Sample2 
        {
		    private string userNameWhatever;
		    private FooBar user;
		    private Foo _foo;
	    }

    }
}
