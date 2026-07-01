using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PathOfTenThousandWays.Demo.Common
{
    public static class SimpleJsonParser
    {
        public static object Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            Parser parser = new Parser(json);
            object value = parser.ParseValue();
            parser.SkipWhitespace();

            if (!parser.IsAtEnd)
            {
                throw new FormatException($"Unexpected trailing characters at index {parser.Index}.");
            }

            return value;
        }

        private sealed class Parser
        {
            private readonly string json;
            private int index;

            public Parser(string json)
            {
                this.json = json;
            }

            public int Index => index;

            public bool IsAtEnd => index >= json.Length;

            public object ParseValue()
            {
                SkipWhitespace();

                if (IsAtEnd)
                {
                    throw new FormatException("Unexpected end of JSON input.");
                }

                char token = json[index];
                switch (token)
                {
                    case '{':
                        return ParseObject();
                    case '[':
                        return ParseArray();
                    case '"':
                        return ParseString();
                    case 't':
                        ConsumeLiteral("true");
                        return true;
                    case 'f':
                        ConsumeLiteral("false");
                        return false;
                    case 'n':
                        ConsumeLiteral("null");
                        return null;
                    default:
                        if (token == '-' || char.IsDigit(token))
                        {
                            return ParseNumber();
                        }

                        throw new FormatException($"Unexpected token '{token}' at index {index}.");
                }
            }

            public void SkipWhitespace()
            {
                while (!IsAtEnd && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }
            }

            private Dictionary<string, object> ParseObject()
            {
                Consume('{');

                Dictionary<string, object> result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                SkipWhitespace();

                if (TryConsume('}'))
                {
                    return result;
                }

                while (true)
                {
                    SkipWhitespace();
                    string key = ParseString();
                    SkipWhitespace();
                    Consume(':');

                    object value = ParseValue();
                    result[key] = value;

                    SkipWhitespace();
                    if (TryConsume('}'))
                    {
                        return result;
                    }

                    Consume(',');
                }
            }

            private List<object> ParseArray()
            {
                Consume('[');

                List<object> result = new List<object>();
                SkipWhitespace();

                if (TryConsume(']'))
                {
                    return result;
                }

                while (true)
                {
                    result.Add(ParseValue());
                    SkipWhitespace();

                    if (TryConsume(']'))
                    {
                        return result;
                    }

                    Consume(',');
                }
            }

            private string ParseString()
            {
                Consume('"');

                StringBuilder builder = new StringBuilder();
                while (!IsAtEnd)
                {
                    char current = json[index++];
                    if (current == '"')
                    {
                        return builder.ToString();
                    }

                    if (current != '\\')
                    {
                        builder.Append(current);
                        continue;
                    }

                    if (IsAtEnd)
                    {
                        break;
                    }

                    char escaped = json[index++];
                    switch (escaped)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            builder.Append(escaped);
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            builder.Append(ParseUnicodeEscape());
                            break;
                        default:
                            throw new FormatException($"Unsupported escape sequence '\\{escaped}' at index {index - 1}.");
                    }
                }

                throw new FormatException("Unterminated JSON string.");
            }

            private object ParseNumber()
            {
                int start = index;

                if (json[index] == '-')
                {
                    index++;
                }

                ConsumeDigits();

                bool isFloatingPoint = false;
                if (!IsAtEnd && json[index] == '.')
                {
                    isFloatingPoint = true;
                    index++;
                    ConsumeDigits();
                }

                if (!IsAtEnd && (json[index] == 'e' || json[index] == 'E'))
                {
                    isFloatingPoint = true;
                    index++;

                    if (!IsAtEnd && (json[index] == '+' || json[index] == '-'))
                    {
                        index++;
                    }

                    ConsumeDigits();
                }

                string token = json.Substring(start, index - start);
                if (!isFloatingPoint &&
                    long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integerValue))
                {
                    if (integerValue >= int.MinValue && integerValue <= int.MaxValue)
                    {
                        return (int)integerValue;
                    }

                    return integerValue;
                }

                return double.Parse(token, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            private char ParseUnicodeEscape()
            {
                if (index + 4 > json.Length)
                {
                    throw new FormatException("Incomplete unicode escape sequence.");
                }

                string hex = json.Substring(index, 4);
                index += 4;

                if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort codePoint))
                {
                    throw new FormatException($"Invalid unicode escape sequence '\\u{hex}'.");
                }

                return (char)codePoint;
            }

            private void ConsumeDigits()
            {
                int start = index;
                while (!IsAtEnd && char.IsDigit(json[index]))
                {
                    index++;
                }

                if (start == index)
                {
                    throw new FormatException($"Expected digit at index {index}.");
                }
            }

            private void ConsumeLiteral(string literal)
            {
                for (int i = 0; i < literal.Length; i++)
                {
                    if (IsAtEnd || json[index] != literal[i])
                    {
                        throw new FormatException($"Expected '{literal}' at index {index}.");
                    }

                    index++;
                }
            }

            private void Consume(char expected)
            {
                if (IsAtEnd || json[index] != expected)
                {
                    char actual = IsAtEnd ? '\0' : json[index];
                    throw new FormatException($"Expected '{expected}' but found '{actual}' at index {index}.");
                }

                index++;
            }

            private bool TryConsume(char expected)
            {
                if (IsAtEnd || json[index] != expected)
                {
                    return false;
                }

                index++;
                return true;
            }
        }
    }
}
