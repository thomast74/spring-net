#region License

/*
 * Copyright © 2002-2011 the original author or authors.
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

#region Imports

using System;
using System.ComponentModel;

using NUnit.Framework;

#endregion

namespace Spring.Core.TypeConversion
{
	/// <summary>
    /// Unit tests for the TypeConverterRegistry class.
    /// </summary>
    /// <author>Bruno Baia</author>
	[TestFixture]
    public sealed class TypeConverterRegistryTests
    {
        [Test]
        public void GetConverterForEnums()
        {
            TypeConverter converter = TypeConverterRegistry.GetConverter(typeof(DayOfWeek));
            Assert.IsTrue(converter is EnumConverter);
        }

        [Test]
        public void GetInternalConverter()
        {
            TypeConverter converter = TypeConverterRegistry.GetConverter(typeof(int));
            Assert.IsTrue(converter is Int32Converter);
        }

        [Test]
        public void GetSpringConverter()
        {
            TypeConverter converter = TypeConverterRegistry.GetConverter(typeof(string[]));
            Assert.IsTrue(converter is StringArrayConverter);
        }

        [Test]
        public void GetSpringConverterGeneric()
        {
            TypeConverter converter = TypeConverterRegistry.GetConverter<string[]>();
            Assert.IsTrue(converter is StringArrayConverter);
        }

        [Test]
        public void RegisterConverter()
        {
            TypeConverterRegistry.RegisterConverter("System.DateTime", "System.ComponentModel.DateTimeConverter");
        }

        [Test]
        public void RegisterConverterGeneric()
        {
            var converter = new TestConverter();
            TypeConverterRegistry.RegisterConverter<int>(converter);

            Assert.That(TypeConverterRegistry.GetConverter(typeof(int)), Is.SameAs(converter));
        }

        [Test]
        [ExpectedException(typeof(TypeLoadException))]
        public void RegisterConverterWithNonResolvableType()
        {
            TypeConverterRegistry.RegisterConverter("Systemm.DateTime", "System.ComponentModel.DateTimeConverter");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterConverterWithNonTypeConverter()
        {
            TypeConverterRegistry.RegisterConverter("System.DateTime", "System.DateTime");
        }

        [Test]
        public void CanConvert()
        {
            bool canConvert = TypeConverterRegistry.CanConvert(typeof(string), typeof(string[]));
            Assert.IsTrue(canConvert);

            canConvert = TypeConverterRegistry.CanConvert(typeof(double), typeof(string[]));
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void CanConvertGeneric()
        {
            bool canConvert = TypeConverterRegistry.CanConvert<string, string[]>();
            Assert.IsTrue(canConvert);

            canConvert = TypeConverterRegistry.CanConvert<double, string[]>();
            Assert.IsFalse(canConvert);
        }

    }

    public class TestConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return null;
        }
    }
}
