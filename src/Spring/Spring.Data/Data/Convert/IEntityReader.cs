﻿#region License

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

namespace Spring.Data.Convert
{
    /// <summary>
    /// Interface to read object from store specific sources.
    /// </summary>
    /// <typeparam name="T">the entity type the converter can handle</typeparam>
    /// <typeparam name="TSource">source the source to create an object of the given type from</typeparam>
    /// <author>Oliver Gierke</author>
    /// <author>Thomas Trageser</author>
    public interface IEntityReader<T, TSource>
    {
        /// <summary>
        /// Reads the given source into the given type.
        /// </summary>
        /// <typeparam name="TRead"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        TRead Read<TRead>(TSource source) where TRead : T;
    }
}
