// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using CppRelativeIncludes;
//
//    var config = Config.FromJson(jsonString);

namespace IncludeFixor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Config
    {
        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        [JsonProperty("includes")]
        public List<Include> Includes { get; set; } = new List<Include>();

        [JsonProperty("sources")]
        public List<Source> Sources { get; set; } = new List<Source>();
    }

    public partial class Include
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("readonly")]
        public bool ReadOnly { get; set; } = false;

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("extensions")]
        public string[] Extensions { get; set; } = new string[0];

        [JsonProperty("file-renames")]
        public List<Rename> FileRenames { get; set; } = new List<Rename>();

        [JsonProperty("folder-renames")]
        public List<Rename> FolderRenames { get; set; } = new List<Rename>();
    }

    public partial class Rename
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public partial class Settings
    {
        [JsonProperty("path-separator")]
        public char PathSeparator { get; set; }
        [JsonProperty("dry-run")]
        public bool DryRun { get; set; }
        [JsonProperty("verbose")]
        public bool Verbose { get; set; }
        [JsonProperty("include-regex")]
        public string IncludeRegex{ get; set; } = "\\s*#\\s*include\\s*([<\"])([^>\"]+)([>\"])";
    }

    public partial class Source
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("extensions")]
        public string[] Extensions { get; set; } = new string[0];
    }

    public partial class Config
    {
        private static Config FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Config>(json, IncludeFixor.Converter.Settings);
        }

        public static bool Read(string filepath, out Config config)
        {
            config = null;

            if (File.Exists(filepath))
            {
                try
                {
                    string json = File.ReadAllText(filepath);
                    config = FromJson(json);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error: your json configuration file has an issue (\"\")", e.Message);
                }
                return true;
            }
            return false;
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Config self)
        {
            return JsonConvert.SerializeObject(self, IncludeFixor.Converter.Settings);
        }
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = { 
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
