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
using NUnit.Framework;
using Spring.Collections.Generic;
using Spring.Data.Mapping.Context;
using Spring.Data.Mapping.Model;
using Spring.Data.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Integration tests for Mapping metadata.
    /// </summary>
    /// <author>Jon Brisbin</author>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [TestFixture]
    [Category(TestCategory.Integration)]
    public class MappingMetadataTests
    {
	    SampleMappingContext ctx;

	    [SetUp]
	    public void SetUp()
        {
		    ctx = new SampleMappingContext();
	    }

	    [Test]
	    public void TestPocoWithId()
	    {
	        ctx.InitialEntitySet = new HashedSet<Type>
	                                   {
	                                       typeof (PersonWithId)
	                                   };
		    ctx.Initialize();

		    IPersistentEntity person = ctx.GetPersistentEntity<PersonWithId>();

		    Assert.That(person.IdProperty, Is.Not.Null);
            Assert.That(person.IdProperty.Type, Is.EqualTo(typeof(string)));
            Assert.That(person.IdProperty.Name, Is.EqualTo("Id"));
	    }

	    [Test]
	    public void TestAssociations() 
        {
		    ctx.InitialEntitySet = new HashedSet<Type> { typeof(PersonWithChildren) };
		    ctx.Initialize();

		    IPersistentEntity person = ctx.GetPersistentEntity<PersonWithChildren>();
            person.DoWithAssociations(new AssociationHandler());
	    }

        public class AssociationHandler : IAssociationHandler
        {
			public void DoWithAssociation(Association association)
            {
			    Assert.That(association.Inverse.ComponentType, Is.EqualTo(typeof(Child)));
            }            
        }

	    public interface ISampleProperty : IPersistentProperty 
        {
	    }

        public class SampleMappingContext : AbstractMappingContext
        {
            public override IMutablePersistentEntity CreatePersistentEntity(ITypeInformation typeInformation)
            {
                return new BasicPersistentEntity(typeInformation);
            }

            public override IPersistentProperty CreatePersistentProperty(PropertyInfo propertyInfo,
                                                                         IPersistentEntity owner,
                                                                         SimpleTypeHolder simpleTypeHolder)
            {
                return new SamplePropertyImpl(propertyInfo, owner, simpleTypeHolder);
            }
        }

        public class SamplePropertyImpl : AnnotationBasedPersistentProperty, ISampleProperty
        {
            public SamplePropertyImpl(PropertyInfo propertyInfo, IPersistentEntity owner, SimpleTypeHolder simpleTypeHolder)
                : base(propertyInfo, owner, simpleTypeHolder)
            {
            }

            public override Association CreateAssociation()
            {
                return new Association(this, null);
            }
        }
    }
}
