﻿namespace ItemChecker.Net.Session
{
    internal class PollResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public bool HadRemoteInteraction { get; set; }
        public string AccountName { get; set; }
    }
}
