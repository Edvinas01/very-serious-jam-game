using InSun.GameCore.Utilities;
using NUnit.Framework;

namespace InSun.GameCore.Tests.Editor
{
    internal sealed class StringUtilitiesTests
    {
        [TestCase(0L, "0")]
        [TestCase(9_999L, "9.9K")]
        [TestCase(10_000L, "10K")]
        [TestCase(1_500_000L, "1.5M")]
        [TestCase(1_500_000_000L, "1.5B")]
        [TestCase(1_500_000_000_000L, "1.5T")]
        public void Should_AbbreviateString_Long(long value, string expected)
        {
            Assert.AreEqual(expected, value.ToAbbreviatedString());
        }

        [TestCase(0L, "0")]
        [TestCase(9_999L, "9.9K")]
        [TestCase(10_000L, "10K")]
        [TestCase(1_500_000L, "1.5M")]
        [TestCase(1_500_000_000L, "1.5B")]
        [TestCase(1_500_000_000_000L, "1.5T")]
        public void Should_BeginAbbreviatedText_Long(long value, string expected)
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, value)
                .GetText();

            Assert.AreEqual(expected, result);
        }

        [TestCase(0.0, "0")]
        [TestCase(9_999.123, "9.9K")]
        [TestCase(10_000.123, "10K")]
        [TestCase(1_500_000.123, "1.5M")]
        [TestCase(1_500_000_000.123, "1.5B")]
        [TestCase(1_500_000_000_000.123, "1.5T")]
        public void Should_BeginAbbreviatedText_Double(double value, string expected)
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, value)
                .GetText();

            Assert.AreEqual(expected, result);
        }

        [TestCase(0L, "0")]
        [TestCase(9L, "9")]
        [TestCase(99L, "99")]
        [TestCase(999L, "999")]
        [TestCase(9_999L, "9K")]
        [TestCase(10_000L, "10K")]
        [TestCase(1_500_000L, "1M")]
        [TestCase(1_500_000_000L, "1B")]
        [TestCase(1_500_000_000_000L, "1T")]
        public void Should_BeginAbbreviatedText_Long_TruncateAbbreviatedDecimal(long value, string expected)
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, value)
                .TruncateAbbreviatedDecimal()
                .GetText();

            Assert.AreEqual(expected, result);
        }

        [TestCase(0.0, "0")]
        [TestCase(0.9, "0.9")]
        [TestCase(0.09, "0.09")]
        [TestCase(0.009, "0")]
        [TestCase(9.999, "9.99")]
        [TestCase(99.999, "99.9")]
        [TestCase(999.999, "999")]
        [TestCase(9_999.123, "9K")]
        [TestCase(10_000.123, "10K")]
        [TestCase(1_500_000.123, "1M")]
        [TestCase(1_500_000_000.123, "1B")]
        [TestCase(1_500_000_000_000.123, "1T")]
        public void Should_BeginAbbreviatedText_Double_TruncateAbbreviatedDecimal(double value, string expected)
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, value)
                .TruncateAbbreviatedDecimal()
                .GetText();

            Assert.AreEqual(expected, result);
        }

        [TestCase(1.05, "1.05")]
        [TestCase(1.25, "1.25")]
        [TestCase(1.999, "1.99")]
        [TestCase(7.005, "7")]
        [TestCase(14.7, "14.7")]
        [TestCase(14.96, "14.9")]
        [TestCase(55.0, "55")]
        [TestCase(150.0, "150")]
        [TestCase(500.7, "500")]
        public void Should_BeginAbbreviatedText_SmallDouble(double value, string expected)
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, value)
                .GetText();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Should_BeginAbbreviatedText_Append_Double()
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, 1_500L)
                .Append("/")
                .Append(1_500_000d)
                .GetText();

            Assert.AreEqual("1.5K/1.5M", result);
        }

        [Test]
        public void Should_BeginAbbreviatedText_Append_Long()
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, 1_500L)
                .Append("/")
                .Append(1_500_000L)
                .GetText();

            Assert.AreEqual("1.5K/1.5M", result);
        }

        [Test]
        public void Should_BeginAbbreviatedText_Append_Suffix()
        {
            var result = StringUtilities
                .BeginAbbreviatedText(null, 1_500L)
                .Append("/s")
                .GetText();

            Assert.AreEqual("1.5K/s", result);
        }
    }
}
