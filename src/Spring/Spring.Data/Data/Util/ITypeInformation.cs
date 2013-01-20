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

namespace Spring.Data.Util
{
    /// <summary>
    /// Interface to access property types and resolving generics on the way. Starting with a <see cref="Type"/>
    /// you can travers properties using <see cref="GetProperty"/> to access type information.
    /// </summary>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface ITypeInformation
    {
        /// <summary>
        /// Returns the <see cref="ITypeInformation"/>s for the parameters of the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructorInfo">must not be <code>null</code></param>
        IList<ITypeInformation> GetParameterTypes(ConstructorInfo constructorInfo);

        /// <summary>
        /// Returns the property information for the property with the given name. Supports property traversal through dot
        /// notation.
        /// </summary>
        /// <param name="fieldName"></param>
	    ITypeInformation GetProperty(string fieldName);

        /// <summary>
	    /// Returns whether the type can be considered a collection, which means it's a container of elements, e.g. a
	    /// <see cref="IEnumerable{T}"/> and <see cref="Array"/>. If this returns <code>true</code> you can expect 
	    /// <see cref="ComponentType"/> to return a non-<code>null</code> value.
        /// </summary>
        bool IsCollectionLike { get; }

        /// <summary>
        /// Returns the component type for <see cref="IEnumerable{T}"/>s or the key type for
        /// <see cref="Dictionary{TKey,TValue}"/>s.
        /// </summary>
        ITypeInformation ComponentType { get; }

        /// <summary>
	    /// Returns whether the property is a <see cref="Dictionary{TKey,TValue}"/>. If this returns <code>true></code>
	    /// you can expect <see cref="ComponentType"/> as well as <see cref="ValueType"/> to return something not 
	    /// <code>null</code>.
        /// </summary>
	    bool IsDictionary { get; }

        /// <summary>
        /// Will return the type of the value in case the underlying type is a <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        ITypeInformation DictionaryValueType { get; }

        /// <summary>
        /// Returns the type of the property. Will resolve generics and the generic context of
        /// </summary>
        Type Type { get; }

        /// <summary>
	    /// Transparently returns the <see cref="Dictionary{TKey,TValue}"/> value type if the type 
	    /// is a <see cref="Dictionary{TKey,TValue}"/>,
	    /// returns the component type if the type <see cref="IsCollectionLike"/> or the simple type if none of this applies.
        /// </summary>
        ITypeInformation ActualType { get; }

        /// <summary>
	    /// Returns a <see cref="ITypeInformation"/> for the return type of the given <see cref="MethodInfo"/>. Will potentially resolve
	    /// generics information against the current types type parameter bindings.
        /// </summary>
        /// <param name="methodInfo">must not be <code>null</code></param>
	    ITypeInformation GetReturnType(MethodInfo methodInfo);

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/>s for the parameters of the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">must not be <code>null</code></param>
	    IList<ITypeInformation> GetParameterTypes(MethodInfo methodInfo);

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/> for the given raw base type.
        /// </summary>
        /// <returns>
	    /// the <see cref="ITypeInformation"/> for the given base type or <code>null</code> in case the current
	    /// <see cref="ITypeInformation"/> does not implement the given type.
	    /// <param name="superType">must not be <code>null</code></param>
        /// </returns>
        ITypeInformation GetSuperTypeInformation(Type superType);

        /// <summary>
	    ///  Returns if the current <see cref="ITypeInformation"/> can be safely assigned to the given one. Thus it will allow 
	    /// to detect that a <see cref="IList{T}"/> where T is <code>long</code> is assignable to <see cref="IList{T}"/> 
	    /// where T is a number.
        /// </summary>
        /// <param name="target"></param>
	    bool IsAssignableFrom(ITypeInformation target);

        /// <summary>
        /// Returns the <see cref="ITypeInformation"/>s for the type arguments of the current <see cref="ITypeInformation"/>
        /// </summary>
        IList<ITypeInformation> TypeArguments { get; }
    }
}
