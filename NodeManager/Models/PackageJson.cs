using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NodeManager.Models
{
    public partial class PackageJson
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("license")] public string License { get; set; }

        [JsonProperty("scripts")] public dynamic Scripts { get; set; }

        [JsonProperty("private")] public bool Private { get; set; }

        [JsonProperty("dependencies")] public dynamic Dependencies { get; set; }

        [JsonProperty("devDependencies")] public dynamic DevDependencies { get; set; }
    }

    public partial class PackageJson
    {
        public static PackageJson FromJson(string json)
            => JsonConvert.DeserializeObject<PackageJson>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this PackageJson self)
            => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static partial class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}