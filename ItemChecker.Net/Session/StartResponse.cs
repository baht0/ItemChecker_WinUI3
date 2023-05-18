using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ItemChecker.Net.Session
{
    internal class StartResponse
    {
        public string ClientId { get; set; }
        public string RequestId { get; set; }
        public string Interval { get; set; }
        public List<EAuthSessionGuardType> AllowedConfirmations { get; set; }
        public string SteamId { get; set; }
        public string WeakToken { get; set; }
        public string ExtendedErrorMessage { get; set; }

        internal static List<EAuthSessionGuardType> GetAllowedConfirmations(JArray array)
        {
            var list = new List<EAuthSessionGuardType>();
            foreach (JObject type in array)
            {
                bool result = Enum.TryParse(type["confirmation_type"].ToString(), out EAuthSessionGuardType guardType);
                list.Add(guardType);
            }
            return list;
        }
    }
    enum EAuthSessionGuardType
    {
        Unknown = 0,
        None = 1,
        EmailCode = 2,
        DeviceCode = 3,
        DeviceConfirmation = 4,
        EmailConfirmation = 5,
        MachineToken = 6
    }
}
