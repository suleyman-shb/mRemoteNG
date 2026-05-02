using System;
using System.Collections;
using System.Runtime.Versioning;
using mRemoteNG.App;
using mRemoteNG.Resources.Language;

namespace mRemoteNG.Connection.Protocol.RDP
{
    [SupportedOSPlatform("windows")]
    public static class RdpErrorCodes
    {
        private static Hashtable _description;

        private static void InitDescription()
        {
            _description = new Hashtable
            {
                {"0", Language.RdpErrorUnknown},
                {"1", Language.RdpErrorCode1},
                {"2", Language.RdpErrorOutOfMemory},
                {"3", Language.RdpErrorWindowCreation},
                {"4", Language.RdpErrorCode2},
                {"5", Language.RdpErrorCode3},
                {"6", Language.RdpErrorCode4},
                {"7", Language.RdpErrorConnection},
                {"100", Language.RdpErrorWinsock},
                {"2825", Language.RdpError2825}
            };
        }

        public static string GetError(int id)
        {
            try
            {
                if (_description == null)
                    InitDescription();

                return (string)_description?[id.ToString()];
            }
            catch (Exception ex)
            {
                Runtime.MessageCollector.AddExceptionStackTrace(Language.RdpErrorGetFailure, ex);
                return string.Format(Language.RdpErrorUnknown, id);
            }
        }
    }
}