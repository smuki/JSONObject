using System;
using System.Xml;
using System.Text;

namespace Volte.Data.Json
{
    internal sealed class  Lexer {
        const  string ZFILE_NAME = "Lexer";
        public Lexer(string text)
        {
            _charPos = 0;

            if (text == null) {
                throw new FormatException();
            }

            _Data = text;
            _length = _Data.Length;
        }

        public bool Next()
        {
            if (_charPos >= _length) {
                return false;
            } else {
                _charPos++;
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
            if (_Data[_charPos] == '"') {
                return ParseStringValue();
            } else if (char.IsDigit(_Data[_charPos]) || _Data[_charPos] == '-') {
                // number?
                return ParseNumericValue();
            } else if (_Data[_charPos] == 't' || _Data[_charPos] == 'f' || _Data[_charPos] == 'n') {
                // literal?
                return ParseLiteralValue();
            } else {
                return "";
            }
        }

        private string ParseLiteralValue()
        {

            int deb = _charPos;

            for (; !" ,]}\n\r".Contains("" + _Data[_charPos]); ++_charPos);

            string s= _Data.Substring(deb, _charPos - deb);

            if (s=="null"){
                return "";
            }
            return s;
        }

        private string ParseNumericValue()
        {
            int deb = _charPos;

            for (; !",]}\n\r".Contains("" + _Data[_charPos]); ++_charPos);

            return _Data.Substring(deb, _charPos - deb);
        }

        private string ParseStringValue()
        {
            if (_Data[_charPos] != '"') {
                throw new FormatException();
            }

            s.Length = 0;
            int runIndex = -1;

            // skip open quote
            _charPos++;

            while (_charPos < _length) {
                char c = _Data[_charPos++];

                if (c == '"') {
                    if (runIndex != -1) {
                        if (s.Length == 0){
                            return _Data.Substring(runIndex, _charPos - runIndex - 1);
                        }

                        s.Append(_Data, runIndex, _charPos - runIndex - 1);
                    }
                    return s.ToString();
                }

                if (c != '\\') {
                    if (runIndex == -1){
                        runIndex = _charPos - 1;
                    }
                    continue;
                }

                if (_charPos == _length) {
                    break;
                }

                if (runIndex != -1) {
                    s.Append(_Data, runIndex, _charPos - runIndex - 1);
                    runIndex = -1;
                }

                switch (_Data[_charPos++]) {
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
                                  if (_length - _charPos < 4) {
                                      break;
                                  }

                                  // parse the 32 bit hex into an integer codepoint
                                  s.Append((char)ParseUnicode(_Data[_charPos], _Data[_charPos + 1], _Data[_charPos + 2], _Data[_charPos + 3]));

                                  // skip 4 chars
                                  _charPos += 4;
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
            if (_charPos >= _length) {
                throw new FormatException("Cannot find object item'_Data name.");
            }

            if (_Data[_charPos] != '"') {
                throw new FormatException();
            }

            // skip open quote
            _charPos++;

            int deb = _charPos;

            while (_charPos < _length) {
                if (_Data[_charPos] == '"') {
                    if (_Data[_charPos - 1] != '\\') {
                        break;
                    }
                }

                _charPos++;
            }

            int end=_charPos;
            _charPos++;

            SkipWhiteSpace();

            if (_charPos >= _length) {
                throw new FormatException();
            }

            if (_Data[_charPos] != ':') {
                throw new FormatException();
            }

            _charPos++;
            return _Data.Substring(deb, end - deb);
        }


        public char NextChar
        {
            get {
                if (_charPos >= _length) {
                    return ' ';
                } else {
                    return _Data[_charPos + 1];
                }
            }
        }

        public void Seek(int position)
        {
            _charPos = position;
        }

        public void SkipWhiteSpace()
        {
            while (_charPos < _length) {
                if (char.IsWhiteSpace(_Data[_charPos])) {
                    _charPos++;
                    continue;
                } else {
                    break;
                }
            }
        }

        public bool MatchChar(char c)
        {
            if (_Data[_charPos]==c){
                return true;
            }else{
                this.SkipWhiteSpace();
                return _Data[_charPos]==c;
            }
        }

        public char Current
        {
            get {
                return _Data[_charPos];
            }
        }
        public int Position
        {
            get {
                return _charPos;
            }
        }
        private bool IsEOS
        {
            get {
                return _charPos >= _length;
            }
        }

        private int _charPos             = 0;
        private int _length              = -1;
        private readonly StringBuilder s = new StringBuilder();
        private readonly string _Data;
    }
}
