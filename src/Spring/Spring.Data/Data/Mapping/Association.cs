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

namespace Spring.Data.Mapping
{
    /// <summary>
    /// Value object to capture <see cref="Association"/>s.
    /// </summary>
    /// <author>Jon Brisbin</author>
    /// <author>Thomas Trageser</author>
    public class Association
    {
        private readonly IPersistentProperty _inverse;
        private readonly IPersistentProperty _obverse;

        /// <summary>
        /// Creates a new <see cref="Association"/> between the two given <see cref="IPersistentProperty"/>s.
        /// </summary>
        /// <param name="inverse"></param>
        /// <param name="obverse"></param>
        public Association(IPersistentProperty inverse, IPersistentProperty obverse)
        {
		    _inverse = inverse;
		    _obverse = obverse;
	    }

        public IPersistentProperty Inverse
        {
            get { return _inverse; }
        }

        public IPersistentProperty Obverse
        {
            get { return _obverse; }
        }
    }
}
