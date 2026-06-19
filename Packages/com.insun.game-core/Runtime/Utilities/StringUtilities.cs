using System;
using System.Globalization;
using TMPro;

namespace InSun.GameCore.Utilities
{
    // Some insp was taken from https://gist.github.com/yasirkula/31704321d6eef8df19933fe502ae6219
    public static class StringUtilities
    {
        private static readonly char[] CharBuffer = new char[128];
        private static int charBufferLength;

        // 4955 -> 5k WRONG
        public static string ToAbbreviatedString(
            this float value,
            string trillionFormat = "0.#'T'",
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string hundredFormat = "0",
            string tenFormat = "0.#",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000_000f)
            {
                return (Math.Truncate(value / 100_000_000_000f) / 10f).ToString(trillionFormat);
            }

            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000f) / 10f).ToString(billionFormat);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000f) / 10f).ToString(millionFormat);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100f) / 10f).ToString(thousandFormat);
            }

            if (value >= 100)
            {
                return value.ToString(hundredFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 10)
            {
                return value.ToString(tenFormat, CultureInfo.InvariantCulture);
            }

            return value.ToString(fallbackFormat);
        }

        public static string ToAbbreviatedString(
            this double value,
            string trillionFormat = "0.#'T'",
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string hundredFormat = "0",
            string tenFormat = "0.#",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000_000d)
            {
                return (Math.Truncate(value / 100_000_000_000d) / 10d).ToString(trillionFormat);
            }

            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000d) / 10d).ToString(billionFormat);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000d) / 10d).ToString(millionFormat);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100d) / 10d).ToString(thousandFormat);
            }

            if (value >= 100)
            {
                return value.ToString(hundredFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 10)
            {
                return value.ToString(tenFormat, CultureInfo.InvariantCulture);
            }

            return value.ToString(fallbackFormat);
        }

        public static string ToAbbreviatedString(
            this int value,
            string trillionFormat = "0.#'T'",
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string hundredFormat = "0",
            string tenFormat = "0.#",
            string fallbackFormat = "0.##"
        )
        {
            return ((long)value).ToAbbreviatedString(
                trillionFormat,
                billionFormat,
                millionFormat,
                thousandFormat,
                hundredFormat,
                tenFormat,
                fallbackFormat
            );
        }

        public static string ToAbbreviatedString(
            this long value,
            string trillionFormat = "0.#'T'",
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string hundredFormat = "0",
            string tenFormat = "0.#",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000_000L)
            {
                return (Math.Truncate(value / 100_000_000_000d) / 10d).ToString(trillionFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000d) / 10d).ToString(billionFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000d) / 10d).ToString(millionFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100d) / 10d).ToString(thousandFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 100)
            {
                return value.ToString(hundredFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 10)
            {
                return value.ToString(tenFormat, CultureInfo.InvariantCulture);
            }

            return value.ToString(fallbackFormat, CultureInfo.InvariantCulture);
        }

        public static void SetAbbreviatedText(this TMP_Text text, long value, bool isTruncateDecimal = false)
        {
            var length = WriteAbbreviatedValue(CharBuffer, 0, value, isTruncateDecimal);
            text.SetCharArray(CharBuffer, 0, length);
        }

        public static void SetAbbreviatedText(this TMP_Text text, double value, bool isTruncateDecimal = false)
        {
            var length = WriteAbbreviatedValue(CharBuffer, 0, value, isTruncateDecimal);
            text.SetCharArray(CharBuffer, 0, length);
        }

        public static SetTextAction BeginAbbreviatedText(this TMP_Text text, long value)
        {
            charBufferLength = WriteAbbreviatedValue(CharBuffer, 0, value);
            return new SetTextAction(text);
        }

        public static SetTextAction BeginAbbreviatedText(this TMP_Text text, double value)
        {
            charBufferLength = WriteAbbreviatedValue(CharBuffer, 0, value);
            return new SetTextAction(text);
        }

        public static SetTextAction BeginAbbreviatedText(this TMP_Text text)
        {
            charBufferLength = 0;
            return new SetTextAction(text);
        }

        private static int WriteAbbreviatedValue(char[] buffer, int position, long value, bool isTruncateDecimal = false)
        {
            if (value >= 1_000_000_000_000L)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: value / 100_000_000_000L,
                    suffix: 'T',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000_000_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: value / 100_000_000,
                    suffix: 'B',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: value / 100_000,
                    suffix: 'M',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: value / 100,
                    suffix: 'K',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            return WriteInt(buffer, position, value);
        }

        private static int WriteAbbreviatedValue(char[] buffer, int position, double value, bool isTruncateDecimal = false)
        {
            if (value >= 1_000_000_000_000d)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: (long)(value / 100_000_000_000d),
                    suffix: 'T',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000_000_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: (long)(value / 100_000_000),
                    suffix: 'B',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: (long)(value / 100_000),
                    suffix: 'M',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 1_000)
            {
                return WriteWithSuffix(
                    buffer: buffer,
                    position: position,
                    tenths: (long)(value / 100),
                    suffix: 'K',
                    isTruncateDecimal: isTruncateDecimal
                );
            }

            if (value >= 100)
            {
                return WriteInt(buffer, position, (long)Math.Floor(value));
            }

            if (value >= 10)
            {
                // Shift decimal by 1 to make an integer - this avoids float math (69.9 -> 699 -> "69.9")
                var tenths = (long)Math.Floor(value * 10);
                position = WriteInt(buffer, position, tenths / 10);

                var remainder = tenths % 10;
                if (remainder != 0)
                {
                    buffer[position++] = '.';
                    position = WriteInt(buffer, position, remainder);
                }

                return position;
            }
            else
            {
                // Shift decimal by 2 to make an integer - this avoids float math (1.69 -> 169 -> "1.69")
                var hundredths = (long)Math.Floor(value * 100);
                position = WriteInt(buffer, position, hundredths / 100);

                var remainder = hundredths % 100;
                if (remainder != 0)
                {
                    buffer[position++] = '.';

                    if (remainder < 10)
                    {
                        buffer[position++] = '0';
                        buffer[position++] = (char)('0' + remainder);
                    }
                    else
                    {
                        buffer[position++] = (char)('0' + remainder / 10);

                        // If needed to not pad the numbers
                        if (remainder % 10 != 0)
                        {
                            buffer[position++] = (char)('0' + remainder % 10);
                        }
                    }
                }

                return position;
            }
        }

        private static int WriteWithSuffix(
            char[] buffer,
            int position,
            long tenths,
            char suffix,
            bool isTruncateDecimal
        )
        {
            position = WriteInt(buffer, position, tenths / 10);

            if (!isTruncateDecimal)
            {
                var remainder = tenths % 10;
                if (remainder != 0)
                {
                    buffer[position++] = '.';
                    position = WriteInt(buffer, position, remainder);
                }
            }

            buffer[position++] = suffix;

            return position;
        }

        private static int WriteInt(char[] buffer, int position, long value)
        {
            if (value == 0)
            {
                buffer[position] = '0';
                return position + 1;
            }

            // Write digits in reverse
            var start = position;
            while (value > 0)
            {
                buffer[position++] = (char)('0' + value % 10);
                value /= 10;
            }

            // Fix reversal
            var end = position - 1;
            while (start < end)
            {
                (buffer[start], buffer[end]) = (buffer[end], buffer[start]);
                start++;
                end--;
            }

            return position;
        }

        public readonly ref struct SetTextAction
        {
            private readonly TMP_Text text;

            public SetTextAction(TMP_Text text)
            {
                this.text = text;
            }

            public SetTextAction TruncateAbbreviatedDecimal()
            {
                var lastChar = CharBuffer[charBufferLength - 1];
                var isAbbreviated = lastChar is 'K' or 'M' or 'B' or 'T';

                if (isAbbreviated == false)
                {
                    return this;
                }

                // Strip ".X" before suffix: "9.9K" -> "9K"
                var suffixIdx = charBufferLength - 1;
                var decimalDigitIdx = charBufferLength - 2;
                var decimalPointIdx = charBufferLength - 3;

                var isDecimalPresent = charBufferLength >= 4
                    && char.IsDigit(CharBuffer[decimalDigitIdx])
                    && CharBuffer[decimalPointIdx] == '.';

                if (isDecimalPresent)
                {
                    CharBuffer[decimalPointIdx] = CharBuffer[suffixIdx];
                    charBufferLength -= 2;
                }

                return this;
            }

            public SetTextAction Append(string value)
            {
                for (var index = 0; index < value.Length; index++)
                {
                    CharBuffer[charBufferLength++] = value[index];
                }

                return this;
            }

            public SetTextAction Append(double value)
            {
                charBufferLength = WriteAbbreviatedValue(CharBuffer, charBufferLength, value);
                return this;
            }

            public SetTextAction Append(long value)
            {
                charBufferLength = WriteAbbreviatedValue(CharBuffer, charBufferLength, value);
                return this;
            }

            public void Apply()
            {
                text.SetCharArray(CharBuffer, 0, charBufferLength);
            }

            public void Dispose()
            {
                Apply();
            }

            /// <summary>
            /// Note, this will cause GC allocations, use <see cref="Apply"/> instead. This should
            /// only be used in tests or similar.
            /// </summary>
            public string GetText()
            {
                return new string(CharBuffer, 0, charBufferLength);
            }
        }
    }
}
