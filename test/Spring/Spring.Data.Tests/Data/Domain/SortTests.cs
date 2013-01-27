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
using NUnit.Framework;

namespace Spring.Data.Domain
{
    /// <summary>
    /// Unit test for <see cref="Sort"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [TestFixture]
    public class SortTests
    {
	    [Test]
	    public void AppliesDefaultForOrder()
	    {
	        var sortEnumerator = new Sort("foo").GetEnumerator();
	        sortEnumerator.MoveNext();

	        Assert.That(Sort.DefaultDirection, Is.EqualTo(sortEnumerator.Current.Direction));

	        sortEnumerator = new Sort(Sort.Direction.Asc, "foo").GetEnumerator();
	        sortEnumerator.MoveNext();

	        Assert.That(Sort.DefaultDirection, Is.EqualTo(sortEnumerator.Current.Direction));
	    }

	    [Test]
	    public void PreventsNullProperties()
        {
            Assert.That(delegate { new Sort(Sort.Direction.Asc, (String[]) null); }, Throws.ArgumentException);
	    }

	    [Test]
	    public void PreventsNullProperty()
	    {
	        Assert.That(delegate { new Sort(Sort.Direction.Asc, (String) null); },
	                    Throws.TypeOf<ArgumentNullException>());
	    }

	    [Test]
	    public void PreventsEmptyProperty()
        {
		    Assert.That(delegate { new Sort(Sort.Direction.Asc, ""); }, Throws.TypeOf<ArgumentNullException>());
	    }

	    [Test]
	    public void PreventsNoProperties()
        {
            Assert.That(delegate { new Sort(Sort.Direction.Asc); }, Throws.ArgumentException);		    
	    }

	    [Test]
	    public void AllowsCombiningSorts()
        {
		    Sort sort = new Sort("foo").And(new Sort("bar"));
            Assert.That(sort.Count(), Is.EqualTo(2));
            Assert.That(sort, Has.Member(new Sort.Order("foo")));
	        Assert.That(sort, Has.Member(new Sort.Order("bar")));
	    }

	    [Test]
	    public void HandlesAdditionalNullSort()
        {
		    Sort sort = new Sort("foo").And(null);
            Assert.That(sort.Count(), Is.EqualTo(1));
            Assert.That(sort, Has.Member(new Sort.Order("foo")));
        }

    }
}
