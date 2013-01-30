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
using Spring.Data.Annotation;

namespace Spring.Data.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Jon Brisbin</author>
    /// <author>Thomas Trageser</author>
    public class PersonWithId : Person
    {
	    public PersonWithId(int ssn, string firstName, string lastName)
            : base(ssn, firstName, lastName)
        {
	    }

        [Id]
        public string Id { get; set; }
    }
}
