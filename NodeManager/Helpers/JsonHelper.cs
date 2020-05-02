using System.Collections.Generic;
using Newtonsoft.Json;

namespace NodeManager.Helpers
{
    internal static class JsonHelper
    {
        public static Dictionary<string, string> DynamicToDictionary(dynamic dynamicObject)
        {
            string json = dynamicObject.ToString(); // suppose `dynamicObject` is your input
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dictionary;
        }
    }
}