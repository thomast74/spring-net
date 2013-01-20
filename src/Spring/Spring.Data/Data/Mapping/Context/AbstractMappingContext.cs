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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Spring.Context;
using Spring.Collections.Generic;
using Spring.Context.Events;
using Spring.Data.Mapping.Model;
using Spring.Data.Util;
using Spring.Util;
using ReflectionUtils = Spring.Data.Util.ReflectionUtils;

namespace Spring.Data.Mapping.Context
{
    public abstract class AbstractMappingContext : IMappingContext, IApplicationContextAware
    {
        private readonly ConcurrentDictionary<ITypeInformation, IMutablePersistentEntity> _persistentEntities =
            new ConcurrentDictionary<ITypeInformation, IMutablePersistentEntity>();

        private IApplicationContext _applicationContext;
        private IApplicationEventPublisher _applicationEventPublisher;

        private Collections.Generic.ISet<Type> _initialEntitySet = new HashedSet<Type>();
        private bool _strict;
        private SimpleTypeHolder _simpleTypeHolder = new SimpleTypeHolder();


        /// <summary>
        /// Set the <see cref="IApplicationContext"/>
        /// </summary>
        public IApplicationContext ApplicationContext
        {
            set
            {
                _applicationContext = value;
                _applicationEventPublisher = _applicationContext;

                _applicationContext.ContextEvent += OnApplicationEvent;
            }
        }

        /// <summary>
        /// Sets the <see cref="Collections.Generic.ISet{T}"/> of types to populate the context initially.
        /// </summary>
        public Collections.Generic.ISet<Type> InitialEntitySet
        {
            set { _initialEntitySet = value; }
        }

        /// <summary>
        /// Configures whether the <see cref="IMappingContext"/> is in strict mode which means, 
        /// that it will throw <see cref="IMappingContext"/>s in case one tries to lookup a
        /// <see cref="IPersistentEntity"/>  not already in the context. This defaults to 
        /// <code>false</code> so that unknown types will be transparently added to the  MappingContext
        /// if not known in advance.
        /// </summary>
        public bool Strict
        {
            set { _strict = value; }
        }

        /// <summary>
        /// Configures the <see cref="SimpleTypeHolder"/> to be used by the 
        /// <see cref="IMappingContext"/>. Allows customization of what types will be regarded
        /// as simple types and thus not recursively analysed. Setting this 
        /// to <code>null</code> will reset the context to use the default <see cref="_simpleTypeHolder"/>.
        /// </summary>
        public SimpleTypeHolder SimpleTypeHolder
        {
            set { _simpleTypeHolder = (value ?? new SimpleTypeHolder()); }
        }

        /// <summary>
        /// Returns all stored <see cref="IMutablePersistentEntity"/>s.
        /// </summary>
        /// <returns></returns>
        public ICollection<IMutablePersistentEntity> PersistentEntities
        {
            get { return _persistentEntities.Values; }
        }

        public IMutablePersistentEntity GetPersistentEntity<T>()
        {
            return GetPersistentEntity(ClassTypeInformation.From(typeof (T)));
        }

        public IMutablePersistentEntity GetPersistentEntity(Type type)
        {
            AssertUtils.ArgumentNotNull(type, "type");
            return GetPersistentEntity(ClassTypeInformation.From(type));
        }

        public IMutablePersistentEntity GetPersistentEntity(IPersistentProperty persistentProperty)
        {
            AssertUtils.ArgumentNotNull(persistentProperty, "persistentProperty");

            return GetPersistentEntity(persistentProperty.TypeInformation);
        }

        public IMutablePersistentEntity GetPersistentEntity(ITypeInformation type)
        {
            AssertUtils.ArgumentNotNull(type, "type");

            IMutablePersistentEntity entity;
            if (_persistentEntities.TryGetValue(type, out entity))
                return entity;

            if (_strict)
            {
                throw new MappingException("Unknown persistent entity " + type);
            }

            return !ShouldCreatePersistentEntityFor(type) ? null : AddPersistentEntity(type);
        }

        public IPersistentPropertyPath<T> GetPersistentPropertyPath<T>(PropertyPath propertyPath)
            where T : IPersistentProperty
        {
            IList<T> result = new List<T>();
            IPersistentEntity current = GetPersistentEntity(propertyPath.OwningType);

            foreach (PropertyPath segment in propertyPath)
            {
                var persistentProperty = current.GetPersistentProperty(segment.Segment);

                if (persistentProperty == null)
                {
                    throw new ArgumentException(string.Format("No property {0} found on {1}!", segment.Segment,
                                                              current.Name));
                }

                result.Add((T)persistentProperty);

                if (segment.HasNext)
                {
                    current = GetPersistentEntity(segment.Type);
                }
            }

            return new DefaultPersistentPropertyPath<T>(result);
        }

        public IMutablePersistentEntity AddPersistentEntity<T>()
        {
            return AddPersistentEntity(ClassTypeInformation.From(typeof (T)));
        }

        public IMutablePersistentEntity AddPersistentEntity(Type type)
        {
            return AddPersistentEntity(ClassTypeInformation.From(type));
        }

        /// <summary>
        /// Adds the given <see cref="ITypeInformation"/> to the <see cref="IMappingContext"/>.
        /// </summary>
        /// <param name="typeInformation"></param>
        public IMutablePersistentEntity AddPersistentEntity(ITypeInformation typeInformation)
        {

            if (_persistentEntities.ContainsKey(typeInformation))
            {
                IMutablePersistentEntity persistentEntity;
                _persistentEntities.TryGetValue(typeInformation, out persistentEntity);
                return persistentEntity;
            }

            var type = typeInformation.Type;
            IMutablePersistentEntity entity = CreatePersistentEntity(typeInformation);

            if (!_persistentEntities.TryAdd(typeInformation, entity))
                throw new MappingException("Can't add entity to dictionary");

            ReflectionUtils.DoWithFields(type, new PersistentPropertyCreator(this, entity), new PersistentFieldFilter());

            try
            {
                entity.Verify();
            }
            catch (MappingException e)
            {
                _persistentEntities.TryRemove(typeInformation, out entity);
                throw e;
            }

            // Inform listeners
            if (_applicationEventPublisher != null)
            {
                _applicationEventPublisher.PublishEvent(this, new MappingContextEvent(this, entity));
            }

            return entity;
        }

        /// <summary>
        /// Creates the concrete <see cref="IMutablePersistentEntity"/> instance.
        /// </summary>
        public abstract IMutablePersistentEntity CreatePersistentEntity(ITypeInformation typeInformation);

        /// <summary>
        /// Creates the concrete instance of <see cref="IMutablePersistentEntity"/>.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="owner"></param>
        /// <param name="simpleTypeHolder"></param>
        public abstract IPersistentProperty CreatePersistentProperty(FieldInfo fieldInfo,
                                                                          IPersistentEntity owner,
                                                                          SimpleTypeHolder simpleTypeHolder);

        /// <summary>
        /// Event listener for <see cref="ContextRefreshedEventArgs"/> event. If received it initializes peristent entities.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnApplicationEvent(object sender, ApplicationEventArgs e)
        {
            if (e is ContextRefreshedEventArgs)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initializes the mapping context. Will add the types configured through {@link #setInitialEntitySet(Set)} to the
        /// context.
        /// </summary>
        public void Initialize()
        {

            foreach (Type initialEntity in _initialEntitySet)
            {
                AddPersistentEntity(initialEntity);
            }
        }

        /// <summary>
        /// Returns whether a <see cref="IPersistentEntity"/> instance should be created for the given 
        /// <see cref="ITypeInformation"/>. By default this will reject this for all types considered simple, but it might
        /// be necessary to tweak that in case you have registered custom converters for top level types (which renders them
        /// to be considered simple) but still need meta-information about them.
        /// </summary>
        /// <param name="type">will nenver be null</param>
        protected bool ShouldCreatePersistentEntityFor(ITypeInformation type)
        {
            return !_simpleTypeHolder.IsSimpleType(type.Type);
        }

        /// <summary>
        /// {@link FieldCallback} to create <see cref="IPersistentProperty"/> instances.
        /// </summary>
        /// <author>Oliver Gierke</author>
        /// <author>Thomas Trageser</author>
        internal sealed class PersistentPropertyCreator : IFieldCallback
        {
            private readonly AbstractMappingContext _context;
            private readonly IMutablePersistentEntity _entity;

            /// <summary>
            /// Creates a new <see cref="PersistentPropertyCreator"/> for the given <see cref="IPersistentEntity"/>.
            /// </summary>
            public PersistentPropertyCreator(AbstractMappingContext context, IMutablePersistentEntity entity)
            {
                _context = context;
                _entity = entity;
            }

            public void DoWith(FieldInfo fieldInfo)
            {
                var property = _context.CreatePersistentProperty(fieldInfo, _entity, _context._simpleTypeHolder);

                if (property.IsTransient)
                    return;

                _entity.AddPersistentProperty(property);

                if (property.IsAssociation)
                    _entity.AddAssociation(property.Association);

                if (_entity.Type == property.RawType)
                    return;

                if (!property.IsEntity)
                    return;

                foreach (ITypeInformation candidate in property.PersistentEntityTypes)
                {
                    _context.AddPersistentEntity(candidate);
                }
            }
        }

        /// <summary>
        /// <see cref="IFieldFilter"/> rejecting static fields as well as artifically introduced ones. See
        /// <see cref="_unmappedFields"/> for details.
        /// </summary>
        /// <author>Oliver Gierke</author>
        /// <author>Thomas Trageser</author>
        internal class PersistentFieldFilter : IFieldFilter
        {
            private readonly IList<FieldMatch> _unmappedFields;

            private static HashedSet<FieldMatch> matches = new HashedSet<FieldMatch>()
                                                               {
                                                                   new FieldMatch("class", null),
                                                                   new FieldMatch("this\\$.*", null),
                                                                   new FieldMatch("metaClass", "groovy.lang.MetaClass")
                                                               };

            public PersistentFieldFilter()
            {
                _unmappedFields = new List<FieldMatch>(matches).AsReadOnly();
            }

            public bool Matches(FieldInfo fieldInfo)
            {
                if (fieldInfo.IsStatic)
                    return false;

                foreach (FieldMatch candidate in _unmappedFields)
                {
                    if (candidate.Matches(fieldInfo))
                        return false;
                }

                return true;
            }

            public bool Matches(PropertyInfo propertyInfo)
            {
                foreach (FieldMatch candidate in _unmappedFields)
                {
                    if (candidate.Matches(propertyInfo))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Value object to help defining field eclusion based on name patterns and types.
        /// </summary>
        /// <author>Oliver Gierke </author>
        /// <author>Thomas Trageser</author>
        internal class FieldMatch
        {
            private readonly Regex _namePattern;
            private readonly string _typeName;

            /// <summary>
            /// Creates a new <see cref="FieldMatch"/> for the given name pattern and type name. 
            /// At least one of the paramters must not be <code>null</code>.
            /// </summary>
            /// <param name="namePattern">a regex pattern to match field names, can be <code>null</code>.</param>
            /// <param name="typeName">the name of the type to exclude, can be <code>null</code>.</param>
            public FieldMatch(string namePattern, string typeName)
            {
                AssertUtils.IsTrue(!(namePattern == null && typeName == null),
                                   "Either name patter or type name must be given!");

                _namePattern = string.IsNullOrEmpty(namePattern) ? null : new Regex(namePattern);
                _typeName = typeName;
            }

            /// <summary>
            /// Returns whether the given <see cref="FieldInfo"/> matches the defined <see cref="FieldMatch"/>.
            /// </summary>
            /// <param name="fieldInfo">must not be <code>null</code>.</param>
            public bool Matches(FieldInfo fieldInfo)
            {
                return Matches(fieldInfo.Name, fieldInfo.FieldType);
            }

            /// <summary>
            /// Returns whether the given <see cref="FieldInfo"/> matches the defined <see cref="FieldMatch"/>.
            /// </summary>
            /// <param name="propertyInfo">must not be <code>null</code>.</param>
            public bool Matches(PropertyInfo propertyInfo)
            {
                return Matches(propertyInfo.Name, propertyInfo.PropertyType);
            }

            private bool Matches(string name, Type type)
            {
                if (_namePattern != null && !_namePattern.IsMatch(name))
                {
                    return false;
                }

                if (_typeName != null && !type.Name.Equals(_typeName))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
