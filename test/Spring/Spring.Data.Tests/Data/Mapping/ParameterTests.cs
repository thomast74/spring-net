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

using System.Reflection;
using NUnit.Framework;
using Spring.Data.Util;
using Rhino.Mocks;
using Spring.Util;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Unit tests for <see cref="Parameter"/>.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    [TestFixture]
    [Category(TestCategory.Unit)]
    public class ParameterTests
    {
        private IPersistentProperty _entityProperty;
        private IPersistentProperty _stringEntityProperty;

        private IPersistentEntity entity;
        private IPersistentEntity stringEntity;

        private ITypeInformation type;

        [SetUp]
        public void SetUp()
        {
            type = ClassTypeInformation.From<object>();

            entity = MockRepository.GenerateStub<IPersistentEntity>();
            stringEntity = MockRepository.GenerateStub<IPersistentEntity>();

            _entityProperty = new PersistentPropertyImpl(entity);
            _stringEntityProperty = new PersistentPropertyImpl(stringEntity);
        }

	    [Test]
	    public void TwoParametersWithIdenticalSetupEqual() 
        {
		    var left = new Parameter("name", type, entity);
            var right = new Parameter("name", type, entity);

		    Assert.That(left, Is.EqualTo(right));
		    Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
	    }

	    [Test]
	    public void TwoParametersWithIdenticalSetupAndNullNameEqual() 
        {
            var left = new Parameter(null, type, entity);
            var right = new Parameter(null, type, entity);

		    Assert.That(left, Is.EqualTo(right));
		    Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
	    }

	    [Test]
	    public void TwoParametersWithIdenticalAndNullEntitySetupEqual() 
        {
            var left = new Parameter("name", type, null);
            var right = new Parameter("name", type, null);

		    Assert.That(left, Is.EqualTo(right));
		    Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
	    }

	    [Test]
	    public void TwoParametersWithDifferentNameAreNotEqual() 
        {
            var left = new Parameter("first", type, entity);
            var right = new Parameter("second", type, entity);

		    Assert.That(left, Is.Not.EqualTo(right));
	    }

	    [Test]
	    public void TwoParametersWithDifferenTypeAreNotEqual() 
        {
            var left = new Parameter("name", type, entity);
            var right = new Parameter("name", ClassTypeInformation.From<string>(), stringEntity);

		    Assert.That(left, Is.Not.EqualTo(right));
	    }

    }
    
    class PersistentPropertyImpl : IPersistentProperty
    {
        private IPersistentEntity _owner;

        public PersistentPropertyImpl(IPersistentEntity owner)
        {
            AssertUtils.ArgumentNotNull(owner, "owner must not be null");

            _owner = owner;
        }

        public IPersistentEntity Owner { get { return _owner; }}

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Type Type
        {
            get { throw new System.NotImplementedException(); }
        }

        public ITypeInformation TypeInformation
        {
            get { throw new System.NotImplementedException(); }
        }

        public FieldInfo FieldInfo
        {
            get { throw new System.NotImplementedException(); }
        }

        public string SpelExpression
        {
            get { throw new System.NotImplementedException(); }
        }

        public Association Association
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsEntity
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsIdProperty
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsVersionProperty
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsCollectionLike
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsDictionary
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsArray
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsTransient
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool ShallBePersisted
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsAssociation
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Type ComponentType
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Type RawType
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Type DictionaryValueType
        {
            get { throw new System.NotImplementedException(); }
        }


        public System.Collections.Generic.IEnumerable<ITypeInformation> PersistentEntityTypes
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
