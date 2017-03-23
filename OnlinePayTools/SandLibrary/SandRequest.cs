using System;
using System.Collections.Generic;
using System.Text;

namespace SandLibrary
{
    class SandRequest
    {
        string transCode;
        string merId;
        string encryptKey;
        string encryptData;
        string sign;
        string extend;

        IDictionary<string, string> parameters;

    }
}
