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

using System.ComponentModel;
using Spring.Data.Mapping;
using Spring.Data.Mapping.Context;

namespace Spring.Data.Convert
{
    /// <summary>
    /// Combined <see cref="IEntityReader{T,TSource}"/> and <see cref="IEntityWriter{T,TSink}"/> and 
    /// add the ability to access a <see cref="IMappingContext"/> and {@link ConversionService}.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSourceOrSink"></typeparam>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IEntityConverter<in T, in TSourceOrSink> : IEntityReader<T, TSourceOrSink>, IEntityWriter<T, TSourceOrSink>
        where T : class
        where TSourceOrSink : class
    {
        /// <summary>
        /// Returns the underlying <see cref="IMappingContext"/> used by the converter.
        /// </summary>
        /// <returns>never <code>null</code></returns>
        IMappingContext MappingContext { get; }

        /*
        /// <summary>
        /// Returns the underlying {@link ConversionService} used by the converter.
        /// </summary>
        /// <returns>never <code>null</code></returns>
        ConversionService ConversionService { get; }
        */
    }
}
