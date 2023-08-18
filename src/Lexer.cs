using System;
using System.Xml;
using System.Text;
using Volte.Utils;

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
                return ParseLiteralValue();
                //return "";
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

        public char NextClean()
        {
            this.Back();
            while (true)
            {
                char c = this.MoveNextChar;
                //Console.WriteLine(_charPos);
                //Console.WriteLine(c);
                if (c == '/')
                {
                    switch (this.MoveNextChar)
                    {
                        case '/':
                            do
                            {
                                c = this.MoveNextChar;
                            } while (c != '\n' && c != '\r' && c != 0);
                            break;
                        case '*':
                            while (true)
                            {
                                c = this.MoveNextChar;
                                if (c == 0)
                                {
                                    throw (new Exception("Unclosed comment."));
                                }
                                if (c == '*')
                                {
                                    if (this.MoveNextChar == '/')
                                    {
                                        break;
                                    }
                                    this.Back();
                                }
                            }
                            break;
                        default:
                            this.Back();
                            return '/';
                    }
                }
                else if (c == 0 || c > ' ')
                {
                    return c;
                }
            }
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

                    case 'u':
                        {
                            if (_length - _charPos < 4)
                            {
                                      break;
                                  }

                                  // parse the 32 bit hex into an integer codepoint
                            s.Append((char)Util.ParseUnicode(_Data[_charPos], _Data[_charPos + 1], _Data[_charPos + 2], _Data[_charPos + 3]));

                                  // skip 4 chars
                                  _charPos += 4;
                              }
                              break;
                }
            }

            throw new Exception("Unexpectedly reached end of string");

        }
       
        public string ParseName()
        {
            if (_charPos >= _length)
            {
                throw new FormatException("Cannot find object item'_Data name.");
            }

            bool quote = true;

            if (_Data[_charPos] != '"')
            {
                quote = false;
            }
            else
            {
                // skip open quote
                _charPos++;
            }

            int deb = _charPos;
            bool hasUnicode= false;

            while (_charPos < _length)
            {
                if ((_Data[_charPos] == ':' && quote == false) || (_Data[_charPos] == '"' && quote))
                {
                    if (_Data[_charPos - 1] != '\\')
                    {
                        break;
                    }
                }
                else if (_Data[_charPos] == '\\')
                {
                    if (_charPos < _length && _Data[_charPos++] == 'u')
                    {
                        hasUnicode = true;
                    }
                }
                _charPos++;
            }

            int end=_charPos;
            if (quote)
            {
                _charPos++;

                SkipWhiteSpace();

                if (_charPos >= _length)
                {
                    throw new FormatException();
                }
            }
            if (_Data[_charPos] != ':')
            {
                throw new FormatException();
            }

            _charPos++;
             if (hasUnicode)
            {
                return Util.DeUnicode(_Data.Substring(deb, end - deb));
            }
            else
            {
                return _Data.Substring(deb, end - deb);
            }
        }

        public void Back()
        {
            if (_charPos > 0)
            {
                _charPos -= 1;
            }
        }

        public char Peek
        {
            get
            {
                if (_charPos >= _length)
                {
                    return ' ';
                }
                else
                {
                    return _Data[_charPos + 1];
                }
            }
        }
        public char MoveNextChar
        {
            get
            {
                if (_charPos >= _length)
                {
                    return ' ';
                }
                else
                {
                    _charPos++;
                    return _Data[_charPos];
                }
            }
        }

        public void Seek(int position)
        {
            _charPos = position;
        }

        public void SkipWhiteSpace()
        {
            while (_charPos < _length)
            {
                if (char.IsWhiteSpace(_Data[_charPos]))
                {
                    _charPos++;
                    continue;
                }
                else if (_Data[_charPos] == '/' && (_charPos + 1 < _length && (_Data[_charPos + 1] == '/' || _Data[_charPos + 1] == '*')))
                {
                    this.NextClean();
                }
                else
                {
                    break;
                }
            }
        }
        public bool MatchChar(char c)
        {
            if (_Data[_charPos] == c)
            {
                return true;
            }
            else
            {
                this.SkipWhiteSpace();
                return _Data[_charPos]==c;
            }
        }

        public char Current
        {
            get
            {
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
