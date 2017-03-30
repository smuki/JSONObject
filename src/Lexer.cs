using System;
using System.Xml;
using System.Text;

namespace Volte.Data.Json
{
    internal class Lexer {
        const  string ZFILE_NAME = "Lexer";
        public Lexer(string text)
        {
            _position = 0;

            if (text == null) {
                throw new FormatException();
            }

            _Data = text;
            _length = _Data.Length;
        }

        public bool Next()
        {
            if (_position >= _length) {
                return false;
            } else {
                _position++;
                return true;
            }
        }

        public bool NextToken()
        {
            this.SkipWhiteSpace();

            if (this.Next()) {
                this.SkipWhiteSpace();
                return true;
            }

            return false;
        }

        public string ParseValue()
        {
            // string?
            if (_Data[_position] == '"') {
                return ParseStringValue();
            } else if (char.IsDigit(_Data[_position]) || _Data[_position] == '-') {
                // number?
                return ParseNumericValue();
            } else if (_Data[_position] == 't' || _Data[_position] == 'f' || _Data[_position] == 'n') {
                // literal?
                return ParseLiteralValue();
            } else {
                return "";
            }
        }

        private string ParseLiteralValue()
        {

            int deb = _position;

            for (; !" ,]}\n\r".Contains("" + _Data[_position]); ++_position);

            string s= _Data.Substring(deb, _position - deb);
            if (s=="null"){
                s="";
            }
            return s;
        }

        private string ParseNumericValue()
        {
            int deb = _position;

            for (; !",]}\n\r".Contains("" + _Data[_position]); ++_position);

            return _Data.Substring(deb, _position - deb);
        }

        private string ParseStringValue()
        {
            if (_Data[_position] != '"') {
                throw new FormatException();
            }

            s.Length = 0;

            // skip open quote
            _position++;

            while (_position < _length) {
                char c = _Data[_position++];

                if (c == '"') {
                    return s.ToString();
                }

                if (c != '\\') {
                    s.Append(c);
                    continue;
                }

                if (_position == _length) {
                    break;
                }

                switch (_Data[_position++]) {
                case '"':
                    s.Append('"');
                    break;

                case '\\':
                    s.Append('\\');
                    break;

                case '/':
                    s.Append('/');
                    break;

                case 'b':
                    s.Append('\b');
                    break;

                case 'f':
                    s.Append('\f');
                    break;

                case 'n':
                    s.Append('\n');
                    break;

                case 'r':
                    s.Append('\r');
                    break;

                case 't':
                    s.Append('\t');
                    break;

                case 'u': {
                    if (_length - _position < 4) {
                        break;
                    }

                    // parse the 32 bit hex into an integer codepoint
                    uint codePoint = ParseUnicode(_Data[_position], _Data[_position + 1], _Data[_position + 2], _Data[_position + 3]);
                    s.Append((char) codePoint);

                    // skip 4 chars
                    _position += 4;
                }
                break;
                }
            }

            throw new Exception("Unexpectedly reached end of string");

        }

        private uint ParseSingleChar(char c1, uint multipliyer)
        {
            uint p1 = 0;

            if (c1 >= '0' && c1 <= '9') {
                p1 = (uint)(c1 - '0') * multipliyer;
            } else if (c1 >= 'A' && c1 <= 'F') {
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            } else if (c1 >= 'a' && c1 <= 'f') {
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            }

            return p1;
        }

        private uint ParseUnicode(char c1, char c2, char c3, char c4)
        {
            uint p1 = ParseSingleChar(c1 , 0x1000);
            uint p2 = ParseSingleChar(c2 , 0x100);
            uint p3 = ParseSingleChar(c3 , 0x10);
            uint p4 = ParseSingleChar(c4 , 1);

            return p1 + p2 + p3 + p4;
        }

        public string ParseName()
        {
            if (_position >= _length) {
                throw new FormatException("Cannot find object item'_Data name.");
            }

            if (_Data[_position] != '"') {
                throw new FormatException();
            }

            // skip open quote
            _position++;

            s.Length = 0;

            while (_position < _length) {
                if (_Data[_position] == '"') {
                    if (_Data[_position - 1] != '\\') {
                        break;
                    }
                }

                s.Append(_Data[_position]);
                _position++;
            }

            _position++;

            SkipWhiteSpace();

            if (_position >= _length) {
                throw new FormatException();
            }

            if (_Data[_position] != ':') {
                throw new FormatException();
            }

            _position++;
            return s.ToString().Trim();
        }


        public char NextChar
        {
            get {
                if (_position >= _length) {
                    return ' ';
                } else {
                    return _Data[_position + 1];
                }
            }
        }

        public void Seek(int position)
        {
            _position = position;
        }

        public void SkipWhiteSpace()
        {
            while (_position < _length) {
                if (char.IsWhiteSpace(_Data[_position])) {
                    _position++;
                    continue;
                } else {
                    break;
                }
            }
        }

        public char Current
        {
            get {
                return _Data[_position];
            }
        }
        public int Position
        {
            get {
                return _position;
            }
        }
        private bool IsEOS
        {
            get {
                return _position >= _length;
            }
        }

        private int _position            = 0;
        private int _length              = -1;
        private readonly StringBuilder s = new StringBuilder();
        private readonly string _Data;
    }
}
