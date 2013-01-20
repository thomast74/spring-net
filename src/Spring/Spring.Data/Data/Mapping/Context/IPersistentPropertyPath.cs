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

namespace Spring.Data.Mapping.Context
{

    public interface IPersistentPropertyPath<T> : IEnumerable<T> where T : IPersistentProperty
    {
        /// <summary>
        /// Returns the dot based path notation using <see cref="IPersistentProperty.Name"/>.
        /// </summary>
        /// <returns></returns>
	    string ToDotPath();

        /// <summary>
	    /// Returns the dot based path notation using the given {@link Converter} to translate individual
	    /// <see cref="IPersistentProperty"/>s to path segments.
        /// </summary>
	    string ToDotPath(Converter<T, string> converter);

        /// <summary>
        /// Returns a <see cref="string"/> path with the given delimiter based on the
        /// <see cref="IPersistentProperty.Name"/>.
        /// </summary>
	    string ToPath(string delimiter);

        /// <summary>
        /// Returns a <see cref="string"/> path with the given delimiter using the given {@link Converter} for
	    /// <see cref="IPersistentProperty"/> to String conversion.
        /// </summary>
        /// <param name="delimiter">will default to <code>.</code> if <code>null</code> is given.</param>
        /// <param name="converter">will default to use <see cref="IPersistentProperty.Name"/>.</param>
	    string ToPath(string delimiter, Converter<T, string> converter);

        /// <summary>
	    /// Returns the last property in the <see cref="IPersistentPropertyPath{T}"/>. So for <code>foo.bar</code> 
	    /// it will return the <see cref="IPersistentProperty"/> for <code>bar</code>. For a simple <code>foo</code>
	    /// it returns <see cref="IPersistentProperty"/> for <code>foo</code>.
        /// </summary>
	    T LeafProperty { get; }

        /// <summary>
        /// Returns the first property in the <see cref="IPersistentPropertyPath{T}"/>. So for <code>foo.bar</code>
        /// it will return the <see cref="IPersistentProperty"/> for <code>foo</code>. For a simple 
        /// <code>foo</code> it returns <see cref="IPersistentProperty"/> for <code>foo</code>.
        /// </summary>
	    T BaseProperty { get; }

        /// <summary>
	    /// Returns whether the given <see cref="IPersistentPropertyPath{T}"/> is a base path of the current one.
	    /// This means that the current <see cref="IPersistentPropertyPath{T}"/> is basically an extension of the 
	    /// given one.
        /// </summary>
	    bool IsBasePathOf(IPersistentPropertyPath<T> path);

        /// <summary>
        /// Returns the sub-path of the current one as if it was based on the given base path. So for a current path
        /// <code>foo.bar</code> and a given base <code>foo</code> it would return <code>bar</code>. If the given
        /// path is not a base of the the current one the current <see cref="IPersistentPropertyPath{T}"/> will be
        /// returned as is.
        /// </summary>
        IPersistentPropertyPath<T> GetExtensionForBaseOf(IPersistentPropertyPath<T> baseProperty);

        /// <summary>
	    /// Returns the parent path of the current <see cref="IPersistentPropertyPath{T}"/>, i.e. the path without
	    /// the leaf property. This happens up to the base property. So for a direct property reference calling this
	    /// method will result in returning the property.
        /// </summary>
	    IPersistentPropertyPath<T> ParentPath { get; }

        /// <summary>
        /// Returns the length of the <see cref="IPersistentPropertyPath{T}"/>.
        /// </summary>
	    int Length { get; }
    }
}
