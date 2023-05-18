using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChecker.Net.Session
{
    internal class PollResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public bool HadRemoteInteraction { get; set; }
        public string AccountName { get; set; }
    }
}
