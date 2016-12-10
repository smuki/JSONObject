using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Volte.Data.Json
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AttributeMapping : Attribute {
        // Fields
        private DbType _Type         = DbType.Object;
        private bool   _AutoIdentity = false;
        private bool   _CanWrite     = true;
        private bool   _Compulsory   = false;
        private bool   _Ignore       = false;
        private bool   _NonPrintable = false;
        private bool   _Nullable     = false;
        private bool   _PrimaryKey   = false;
        private bool   _Readonly     = true;
        private bool   _timestamp    = false;
        private int    _Index        = -1;
        private int    _MaxWidth     = 0;
        private int    _MinWidth     = 0;
        private int    _Scale        = 0;
        private int    _Width        = 0;
        private string _AliasName    = "";
        private string _AlignName    = "";
        private string _Caption      = "";
        private string _ColumnName   = "";
        private string _DataBand     = "";
        private string _DataType     = "";
        private string _Name         = "";
        private string _Options      = "";
        private string _Reference    = "";
        private string _Status       = "";
        private string _TableName    = "";
        private string _TypeChar     = "c";
        private string _enabledMode  = "";

        private Dictionary<string, string> _Contexts = new Dictionary<string, string> (StringComparer.InvariantCultureIgnoreCase);

        // Properties
        public DbType Type       { get { return _Type;         } set { _Type         = value; }  }
        public bool AutoIdentity { get { return _AutoIdentity; } set { _AutoIdentity = value; }  }
        public bool Compulsory   { get { return _Compulsory;   } set { _Compulsory   = value; }  }
        public bool Ignore       { get { return _Ignore;       } set { _Ignore       = value; }  }
        public bool NonPrintable { get { return _NonPrintable; } set { _NonPrintable = value; }  }
        public bool Nullable     { get { return _Nullable;     } set { _Nullable     = value; }  }
        public bool PrimaryKey   { get { return _PrimaryKey;   } set { _PrimaryKey   = value; }  }
        public bool Readonly     { get { return _Readonly;     } set { _Readonly     = value; }  }
        public bool Timestamp    { get { return _timestamp;    } set { _timestamp    = value; }  }
        public int Index         { get { return _Index;        } set { _Index        = value; }  }
        public int MaxWidth      { get { return _MaxWidth;     } set { _MaxWidth     = value; }  }
        public int MinWidth      { get { return _MinWidth;     } set { _MinWidth     = value; }  }
        public int Scale         { get { return _Scale;        } set { _Scale        = value; }  }
        public int Width         { get { return _Width;        } set { _Width        = value; }  }
        public string AliasName  { get { return _AliasName;    } set { _AliasName    = value; }  }
        public string AlignName  { get { return _AlignName;    } set { _AlignName    = value; }  }
        public string Caption    { get { return _Caption;      } set { _Caption      = value; }  }
        public string ColumnName { get { return _ColumnName;   } set { _ColumnName   = value; }  }
        public string DataBand   { get { return _DataBand;     } set { _DataBand     = value; }  }
        public string DataType   { get { return _DataType;     } set { _DataType     = value; }  }
        public string EnableMode { get { return _enabledMode;  } set { _enabledMode  = value; }  }
        public string Name       { get { return _Name;         } set { _Name         = value; }  }
        public string Options    { get { return _Options;      } set { _Options      = value; }  }
        public string Reference  { get { return _Reference;    } set { _Reference    = value; }  }
        public string Status     { get { return _Status;       } set { _Status       = value; }  }
        public string TableName  { get { return _TableName;    } set { _TableName    = value; }  }
        public string TypeChar   { get { return _TypeChar;     } set { _TypeChar     = value; }  }

        public bool CanWrite     { get { return _CanWrite && !_AutoIdentity; } set { _CanWrite     = value; }  }

        public string ClassName
        {
            get {
                string _ccss = "";

                if (this.Compulsory) {
                    _ccss = _ccss + "r";
                } else {
                    _ccss = _ccss + "n";
                }

                if (this.Readonly) {
                    _ccss = _ccss + "v";
                } else {
                    _ccss = _ccss + "e";
                }

                return "c" + this.TypeChar + _ccss;
            }
        }

        public string this[string name]
        {
            get {
                if (_Contexts.ContainsKey(name)) {
                    return _Contexts[name];
                } else {
                    return "";
                }
            } set {
                _Contexts[name] = value;
            }
        }

        // Methods
        public AttributeMapping()
        {
        }

        public AttributeMapping(string name)
        {
            _Name       = name;
            _ColumnName = name;
        }
    }
}
