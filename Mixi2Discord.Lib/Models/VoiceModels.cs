using System;
using System.Collections.Generic;

namespace Mixi2Discord.Lib.Models
{
    public class VoiceModel
    {
        public string NickName;
        public string Voice;
        public DateTime PostTime;
        public string Url;
        public IList<VoiceResponse> Response;
    }

    public class VoiceResponse
    {
        public VoiceModel Parent;
        public string NickName;
        public string Voice;
        public DateTime PostTime;
    }
}
