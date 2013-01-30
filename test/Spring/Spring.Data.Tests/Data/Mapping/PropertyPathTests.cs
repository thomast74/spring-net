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
		    PropertyPath reference = PropertyPath.From<Foo>("UserName");

		    Assert.That(reference.HasNext, Is.False);
		    Assert.That(reference.DotPath, Is.EqualTo("UserName"));
		    Assert.That(reference.OwningType, Is.EqualTo(ClassTypeInformation.From<Foo>()));
	    }

        [Test]
	    public void ParsesPathPropertyCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("UserName");

		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
		    Assert.That(reference.DotPath, Is.EqualTo("User.Name"));
	    }

        [Test]
	    public void PrefersLongerMatches()
        {
		    PropertyPath reference = PropertyPath.From<Sample>("UserName");

		    Assert.That(reference.HasNext, Is.False);
		    Assert.That(reference.DotPath, Is.EqualTo("UserName"));
	    }

        [Test]
	    public void TestName() 
        {
		    PropertyPath reference = PropertyPath.From<Sample2>("UserName");

		    Assert.That(reference.Segment, Is.EqualTo("User"));
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
	    }

        [Test]
	    public void PrefersExplicitPaths()
        {
		    PropertyPath reference = PropertyPath.From<Sample>("User_Name");

		    Assert.That(reference.Segment, Is.EqualTo("User"));
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesGenericsCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("UsersName");

		    Assert.That(reference.Segment, Is.EqualTo("Users"));
		    Assert.That(reference.IsCollection, Is.True);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesDictionaryCorrectly() 
        {
		    PropertyPath reference = PropertyPath.From<Bar>("UserDictionaryName");

		    Assert.That(reference.Segment, Is.EqualTo("UserDictionary"));
		    Assert.That(reference.IsCollection, Is.False);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesArrayCorrectly()
        {
		    PropertyPath reference = PropertyPath.From<Bar>("UserArrayName");

		    Assert.That(reference.Segment, Is.EqualTo("UserArray"));
		    Assert.That(reference.IsCollection, Is.True);
		    Assert.That(reference.HasNext, Is.True);
		    Assert.That(reference.Next, Is.EqualTo(new PropertyPath("Name", typeof(FooBar))));
	    }

        [Test]
	    public void HandlesInvalidCollectionCompountTypeProperl()
        {
		    try 
            {
			    PropertyPath.From<Bar>("UsersMame");
			    Assert.Fail("Expected PropertyReferenceException!");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.PropertyName, Is.EqualTo("Mame"));
			    Assert.That(e.BaseProperty, Is.EqualTo(PropertyPath.From<Bar>("Users")));
		    }
	    }

        [Test]
	    public void HandlesInvalidDictionaryValueTypeProperly()
        {
		    try 
            {
			    PropertyPath.From<Bar>("UserDictionaryMame");
			    Assert.Fail("Expected PropertyReferenceException!");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.PropertyName, Is.EqualTo("Mame"));
			    Assert.That(e.BaseProperty, Is.EqualTo(PropertyPath.From<Bar>("UserDictionary")));
		    }
	    }

        [Test]
	    public void FindsNested()
        {
		    PropertyPath from = PropertyPath.From<Sample>("Bar.User.Name");

		    Assert.That(from, Is.Not.Null);
		    Assert.That(from.LeafProperty, Is.EqualTo(PropertyPath.From<FooBar>("Name")));
	    }

	    /**
	     * @see DATACMNS-45
	     */
        [Test]
	    public void HandlesEmptyUnderscoresCorrectly()
        {
		    PropertyPath propertyPath = PropertyPath.From<Sample2>("Foo");

		    Assert.That(propertyPath.Segment, Is.EqualTo("Foo"));
		    Assert.That(propertyPath.Type, Is.EqualTo(typeof(Foo)));

		    propertyPath = PropertyPath.From<Sample2>("Foo_Email");

		    Assert.That(propertyPath.DotPath, Is.EqualTo("Foo.Email"));
	    }

        [Test]
	    public void SupportsDotNotationAsWell()
        {
		    PropertyPath propertyPath = PropertyPath.From<Sample>("Bar.UserDictionary.Name");

		    Assert.That(propertyPath, Is.Not.Null);
		    Assert.That(propertyPath.Segment, Is.EqualTo("Bar"));
		    Assert.That(propertyPath.LeafProperty, Is.EqualTo(PropertyPath.From<FooBar>("Name")));
	    }

        [Test]
	    public void ReturnsCorrectIteratorForSingleElement()
        {
		    PropertyPath propertyPath = PropertyPath.From<Foo>("UserName");
		    var enumerator = propertyPath.GetEnumerator();

		    Assert.That(enumerator.MoveNext(), Is.True);
		    Assert.That(enumerator.Current, Is.EqualTo(propertyPath));
		    Assert.That(enumerator.MoveNext(), Is.False);
	    }

        [Test]
	    public void ReturnsCorrectIteratorForMultipleElement()
        {
		    PropertyPath propertyPath = PropertyPath.From<Bar>("User.Name");
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
			    PropertyPath.From<Foo>("Id");
                Assert.Fail("Expected PropertyReferenceException");
		    }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property Id"));
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
			    PropertyPath.From<Sample2>("Foo_Id");
                Assert.Fail("Expected PropertyReferenceException");
            }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property Id"));
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
			    PropertyPath.From<Sample2>("Foo_Id");
                Assert.Fail("Expected PropertyReferenceException");
            }
            catch (PropertyReferenceException e) 
            {
			    Assert.That(e.Message, Contains.Substring("property Id"));
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
			    PropertyPath.From<Foo>("Bar");
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
		    PropertyPath left = PropertyPath.From<Bar>("User.Name");
		    PropertyPath right = PropertyPath.From<Bar>("User.Name");
		    PropertyPath shortPath = PropertyPath.From<Bar>("User");

		    Assert.That(left, Is.EqualTo(right));
		    Assert.That(right, Is.EqualTo(left));
		    Assert.That(left, Is.Not.EqualTo(shortPath));
		    Assert.That(shortPath, Is.Not.EqualTo(left));
		    Assert.That(left, Is.Not.EqualTo(new object()));
	    }

        [Test]
	    public void HashCodeTests() 
        {
		    PropertyPath left = PropertyPath.From<Bar>("User.Name");
		    PropertyPath right = PropertyPath.From<Bar>("User.Name");
		    PropertyPath shortPath = PropertyPath.From<Bar>("User");

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
		    PropertyPath path = PropertyPath.From<Sample2>("Foo.UUID");

		    Assert.That(path, Is.Not.Null);
		    Assert.That(path.Segment, Is.EqualTo("Foo"));
		    Assert.That(path.HasNext, Is.True);
		    Assert.That(path.Next.Segment, Is.EqualTo("UUID"));
	    }





	    private class Foo
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string UUID { get; set; }
	    }

	    private class Bar
        {
            public FooBar User { get; set; }
            public Set<FooBar> Users { get; set; }
            public Dictionary<string, FooBar> UserDictionary { get; set; }
            public FooBar[] UserArray { get; set; }
	    }

	    private class FooBar 
        {
            public string Name { get; set; }
	    }

	    private class Sample
        {
            public string UserName { get; set; }
            public FooBar User { get; set; }
            public Bar Bar { get; set; }
	    }

	    private class Sample2 
        {
            public string UserNameWhatever { get; set; }
            public FooBar User { get; set; }
            public Foo Foo { get; set; }
	    }

    }
}
