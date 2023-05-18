using System.Collections.Generic;

namespace ItemChecker.Net.Session
{
    internal class FinalizeResponse
    {
        public string SteamID { get; set; }
        public string Redir { get; set; }
        public List<Transfer> TransferInfo { get; set; }
        public string PrimaryDomain { get; set;}
    }
    internal class Transfer
    {
        public string Url { get; set; }
        public Param Params { get; set; }
    }
    internal class Param
    {
        public string Nonce { get; set; }
        public string Auth { get; set; }
    }
}
