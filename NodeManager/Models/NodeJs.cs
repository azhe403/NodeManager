using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NodeManager.Models
{
    public partial class NodeJs
    {
        [JsonProperty("version")] 
        public string NodeVersion { get; set; }

        [JsonProperty("date")] 
        public DateTimeOffset Date { get; set; }

        [JsonProperty("files")] 
        public NodeFile[] NodeFiles { get; set; }

        [JsonProperty("npm", NullValueHandling = NullValueHandling.Ignore)]
        public string NpmVersion { get; set; }

        [JsonProperty("v8")] 
        public string V8 { get; set; }

        [JsonProperty("uv", NullValueHandling = NullValueHandling.Ignore)]
        public string Uv { get; set; }

        [JsonProperty("zlib", NullValueHandling = NullValueHandling.Ignore)]
        public Zlib? Zlib { get; set; }

        [JsonProperty("openssl", NullValueHandling = NullValueHandling.Ignore)]
        public string Openssl { get; set; }

        [JsonProperty("modules", NullValueHandling = NullValueHandling.Ignore)]
        public ModulesUnion? Modules { get; set; }

        [JsonProperty("lts")] 
        public LtsUnion Lts { get; set; }

        [JsonProperty("security")] 
        public bool Security { get; set; }
    }

    public enum NodeFile
    {
        AixPpc64,
        Headers,
        LinuxArm64,
        LinuxArmv6L,
        LinuxArmv7L,
        LinuxPpc64Le,
        LinuxS390X,
        LinuxX64,
        LinuxX86,
        OsxX64Pkg,
        OsxX64Tar,
        OsxX86Tar,
        Src,
        SunosX64,
        SunosX86,
        WinX647Z,
        WinX64Exe,
        WinX64Msi,
        WinX64Zip,
        WinX867Z,
        WinX86Exe,
        WinX86Msi,
        WinX86Zip
    };
    public enum LtsEnum
    {
        Argon,
        Boron,
        Carbon,
        Dubnium,
        Erbium,
        NonLTS,
        Fermium
    }
    public enum ModulesEnum
    {
        The0X000A,
        The0X000B,
        The0X000C
    };

    public enum Zlib
    {
        The1211,
        The123,
        The128
    };

    public struct LtsUnion
    {
        public bool? Bool;
        public LtsEnum? Enum;

        public static implicit operator LtsUnion(bool @bool) => new LtsUnion { Bool = @bool };

        public static implicit operator LtsUnion(LtsEnum @enum) => new LtsUnion { Enum = @enum };
    }

    public struct ModulesUnion
    {
        public ModulesEnum? Enum;
        public long? Integer;

        public static implicit operator ModulesUnion(ModulesEnum @enum) => new ModulesUnion { Enum = @enum };

        public static implicit operator ModulesUnion(long integer) => new ModulesUnion { Integer = integer };
    }

    public partial class NodeJs
    {
        public static NodeJs[] FromJson(string json) =>
            JsonConvert.DeserializeObject<NodeJs[]>(json, NodeJsConverter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this NodeJs[] self) => JsonConvert.SerializeObject(self, NodeJsConverter.Settings);
    }

    internal static partial class NodeJsConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                FileConverter.Singleton,
                LtsUnionConverter.Singleton,
                LtsEnumConverter.Singleton,
                ModulesUnionConverter.Singleton,
                ModulesEnumConverter.Singleton,
                ZlibConverter.Singleton,
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }

    internal class FileConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(NodeFile) || t == typeof(NodeFile?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "aix-ppc64":
                    return NodeFile.AixPpc64;

                case "headers":
                    return NodeFile.Headers;

                case "linux-arm64":
                    return NodeFile.LinuxArm64;

                case "linux-armv6l":
                    return NodeFile.LinuxArmv6L;

                case "linux-armv7l":
                    return NodeFile.LinuxArmv7L;

                case "linux-ppc64le":
                    return NodeFile.LinuxPpc64Le;

                case "linux-s390x":
                    return NodeFile.LinuxS390X;

                case "linux-x64":
                    return NodeFile.LinuxX64;

                case "linux-x86":
                    return NodeFile.LinuxX86;

                case "osx-x64-pkg":
                    return NodeFile.OsxX64Pkg;

                case "osx-x64-tar":
                    return NodeFile.OsxX64Tar;

                case "osx-x86-tar":
                    return NodeFile.OsxX86Tar;

                case "src":
                    return NodeFile.Src;

                case "sunos-x64":
                    return NodeFile.SunosX64;

                case "sunos-x86":
                    return NodeFile.SunosX86;

                case "win-x64-7z":
                    return NodeFile.WinX647Z;

                case "win-x64-exe":
                    return NodeFile.WinX64Exe;

                case "win-x64-msi":
                    return NodeFile.WinX64Msi;

                case "win-x64-zip":
                    return NodeFile.WinX64Zip;

                case "win-x86-7z":
                    return NodeFile.WinX867Z;

                case "win-x86-exe":
                    return NodeFile.WinX86Exe;

                case "win-x86-msi":
                    return NodeFile.WinX86Msi;

                case "win-x86-zip":
                    return NodeFile.WinX86Zip;
            }

            throw new Exception("Cannot unmarshal type NodeFile");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (NodeFile)untypedValue;
            switch (value)
            {
                case NodeFile.AixPpc64:
                    serializer.Serialize(writer, "aix-ppc64");
                    return;

                case NodeFile.Headers:
                    serializer.Serialize(writer, "headers");
                    return;

                case NodeFile.LinuxArm64:
                    serializer.Serialize(writer, "linux-arm64");
                    return;

                case NodeFile.LinuxArmv6L:
                    serializer.Serialize(writer, "linux-armv6l");
                    return;

                case NodeFile.LinuxArmv7L:
                    serializer.Serialize(writer, "linux-armv7l");
                    return;

                case NodeFile.LinuxPpc64Le:
                    serializer.Serialize(writer, "linux-ppc64le");
                    return;

                case NodeFile.LinuxS390X:
                    serializer.Serialize(writer, "linux-s390x");
                    return;

                case NodeFile.LinuxX64:
                    serializer.Serialize(writer, "linux-x64");
                    return;

                case NodeFile.LinuxX86:
                    serializer.Serialize(writer, "linux-x86");
                    return;

                case NodeFile.OsxX64Pkg:
                    serializer.Serialize(writer, "osx-x64-pkg");
                    return;

                case NodeFile.OsxX64Tar:
                    serializer.Serialize(writer, "osx-x64-tar");
                    return;

                case NodeFile.OsxX86Tar:
                    serializer.Serialize(writer, "osx-x86-tar");
                    return;

                case NodeFile.Src:
                    serializer.Serialize(writer, "src");
                    return;

                case NodeFile.SunosX64:
                    serializer.Serialize(writer, "sunos-x64");
                    return;

                case NodeFile.SunosX86:
                    serializer.Serialize(writer, "sunos-x86");
                    return;

                case NodeFile.WinX647Z:
                    serializer.Serialize(writer, "win-x64-7z");
                    return;

                case NodeFile.WinX64Exe:
                    serializer.Serialize(writer, "win-x64-exe");
                    return;

                case NodeFile.WinX64Msi:
                    serializer.Serialize(writer, "win-x64-msi");
                    return;

                case NodeFile.WinX64Zip:
                    serializer.Serialize(writer, "win-x64-zip");
                    return;

                case NodeFile.WinX867Z:
                    serializer.Serialize(writer, "win-x86-7z");
                    return;

                case NodeFile.WinX86Exe:
                    serializer.Serialize(writer, "win-x86-exe");
                    return;

                case NodeFile.WinX86Msi:
                    serializer.Serialize(writer, "win-x86-msi");
                    return;

                case NodeFile.WinX86Zip:
                    serializer.Serialize(writer, "win-x86-zip");
                    return;
            }

            throw new Exception("Cannot marshal type NodeFile");
        }

        public static readonly FileConverter Singleton = new FileConverter();
    }

    internal class LtsUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(LtsUnion) || t == typeof(LtsUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    var boolValue = serializer.Deserialize<bool>(reader);
                    return new LtsUnion { Bool = boolValue };

                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "Argon":
                            return new LtsUnion { Enum = LtsEnum.Argon };

                        case "Boron":
                            return new LtsUnion { Enum = LtsEnum.Boron };

                        case "Carbon":
                            return new LtsUnion { Enum = LtsEnum.Carbon };

                        case "Dubnium":
                            return new LtsUnion { Enum = LtsEnum.Dubnium };

                        case "Erbium":
                            return new LtsUnion { Enum = LtsEnum.Erbium };

                        case "Fermium":
                            return new LtsUnion { Enum = LtsEnum.Fermium };
                    }

                    break;
            }

            throw new Exception("Cannot unmarshal type LtsUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (LtsUnion)untypedValue;
            if (value.Bool != null)
            {
                serializer.Serialize(writer, value.Bool.Value);
                return;
            }

            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case LtsEnum.Argon:
                        serializer.Serialize(writer, "Argon");
                        return;

                    case LtsEnum.Boron:
                        serializer.Serialize(writer, "Boron");
                        return;

                    case LtsEnum.Carbon:
                        serializer.Serialize(writer, "Carbon");
                        return;

                    case LtsEnum.Dubnium:
                        serializer.Serialize(writer, "Dubnium");
                        return;

                    case LtsEnum.Erbium:
                        serializer.Serialize(writer, "Erbium");
                        return;
                }
            }

            throw new Exception("Cannot marshal type LtsUnion");
        }

        public static readonly LtsUnionConverter Singleton = new LtsUnionConverter();
    }

    internal class LtsEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(LtsEnum) || t == typeof(LtsEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Argon":
                    return LtsEnum.Argon;

                case "Boron":
                    return LtsEnum.Boron;

                case "Carbon":
                    return LtsEnum.Carbon;

                case "Dubnium":
                    return LtsEnum.Dubnium;

                case "Erbium":
                    return LtsEnum.Erbium;
            }

            throw new Exception("Cannot unmarshal type LtsEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (LtsEnum)untypedValue;
            switch (value)
            {
                case LtsEnum.Argon:
                    serializer.Serialize(writer, "Argon");
                    return;

                case LtsEnum.Boron:
                    serializer.Serialize(writer, "Boron");
                    return;

                case LtsEnum.Carbon:
                    serializer.Serialize(writer, "Carbon");
                    return;

                case LtsEnum.Dubnium:
                    serializer.Serialize(writer, "Dubnium");
                    return;

                case LtsEnum.Erbium:
                    serializer.Serialize(writer, "Erbium");
                    return;
            }

            throw new Exception("Cannot marshal type LtsEnum");
        }

        public static readonly LtsEnumConverter Singleton = new LtsEnumConverter();
    }

    internal class ModulesUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ModulesUnion) || t == typeof(ModulesUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "0x000A":
                            return new ModulesUnion { Enum = ModulesEnum.The0X000A };

                        case "0x000B":
                            return new ModulesUnion { Enum = ModulesEnum.The0X000B };

                        case "0x000C":
                            return new ModulesUnion { Enum = ModulesEnum.The0X000C };
                    }

                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return new ModulesUnion { Integer = l };
                    }

                    break;
            }

            throw new Exception("Cannot unmarshal type ModulesUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ModulesUnion)untypedValue;
            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case ModulesEnum.The0X000A:
                        serializer.Serialize(writer, "0x000A");
                        return;

                    case ModulesEnum.The0X000B:
                        serializer.Serialize(writer, "0x000B");
                        return;

                    case ModulesEnum.The0X000C:
                        serializer.Serialize(writer, "0x000C");
                        return;
                }
            }

            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value.ToString());
                return;
            }

            throw new Exception("Cannot marshal type ModulesUnion");
        }

        public static readonly ModulesUnionConverter Singleton = new ModulesUnionConverter();
    }

    internal class ModulesEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ModulesEnum) || t == typeof(ModulesEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "0x000A":
                    return ModulesEnum.The0X000A;

                case "0x000B":
                    return ModulesEnum.The0X000B;

                case "0x000C":
                    return ModulesEnum.The0X000C;
            }

            throw new Exception("Cannot unmarshal type ModulesEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (ModulesEnum)untypedValue;
            switch (value)
            {
                case ModulesEnum.The0X000A:
                    serializer.Serialize(writer, "0x000A");
                    return;

                case ModulesEnum.The0X000B:
                    serializer.Serialize(writer, "0x000B");
                    return;

                case ModulesEnum.The0X000C:
                    serializer.Serialize(writer, "0x000C");
                    return;
            }

            throw new Exception("Cannot marshal type ModulesEnum");
        }

        public static readonly ModulesEnumConverter Singleton = new ModulesEnumConverter();
    }

    internal class ZlibConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Zlib) || t == typeof(Zlib?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "1.2.11":
                    return Zlib.The1211;

                case "1.2.3":
                    return Zlib.The123;

                case "1.2.8":
                    return Zlib.The128;
            }

            throw new Exception("Cannot unmarshal type Zlib");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (Zlib)untypedValue;
            switch (value)
            {
                case Zlib.The1211:
                    serializer.Serialize(writer, "1.2.11");
                    return;

                case Zlib.The123:
                    serializer.Serialize(writer, "1.2.3");
                    return;

                case Zlib.The128:
                    serializer.Serialize(writer, "1.2.8");
                    return;
            }

            throw new Exception("Cannot marshal type Zlib");
        }

        public static readonly ZlibConverter Singleton = new ZlibConverter();
    }
}