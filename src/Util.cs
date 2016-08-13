using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Security.Cryptography;
using System.Globalization;
using System.IO.Compression;

namespace Volte.Data.JSONObject
{

    internal class Util {
        const string ZFILE_NAME = "Util";

        internal Util()
        {

        }
        
        internal static DateTime DateTime_MinValue
        {
            get {
                return DateTime.MinValue;
            }
        }
     
        internal static bool IsNumeric(object str)
        {
            decimal d;
            return decimal.TryParse(str.ToString(), out d);
        }

        internal static bool ToBoolean(object cValue)
        {
            bool d;
            return bool.TryParse(Convert.ToString(cValue), out d) ? d : false;
        }

        internal static decimal ToDecimal(object cValue)
        {
            decimal d;
            return decimal.TryParse(Convert.ToString(cValue), out d) ? d : 0M;
        }

        internal static int ToInt(object oValue)
        {
            return Util.ToInt32(oValue);
        }

        internal static int ToInt32(object oValue)
        {
            int d;
            return int.TryParse(oValue.ToString(), out d) ? d : 0;
        }

        internal static void EscapeString(StringBuilder ss, string text)
        {
            foreach (char ch in text) {
                switch (ch) {
                case '"':
                    ss.Append("\\\"");
                    break;

                case '\\':
                    ss.Append(@"\\");
                    break;

                case '\b':
                    ss.Append(@"\b");
                    break;

                case '\f':
                    ss.Append(@"\f");
                    break;

                case '\n':
                    ss.Append(@"\n");
                    break;

                case '\r':
                    ss.Append(@"\r");
                    break;

                case '\t':
                    ss.Append(@"\t");
                    break;

                default:
                    if (char.IsLetterOrDigit(ch)) {
                        ss.Append(ch);
                    } else if (char.IsPunctuation(ch)) {
                        ss.Append(ch);
                    } else if (char.IsSeparator(ch)) {
                        ss.Append(ch);
                    } else if (char.IsWhiteSpace(ch)) {
                        ss.Append(ch);
                    } else if (char.IsSymbol(ch)) {
                        ss.Append(ch);
                    } else {
                        ss.Append("\\u");
                        ss.Append(((int) ch).ToString("X4", NumberFormatInfo.InvariantInfo));
                    }

                    break;
                }
            }
        }
    }
}
