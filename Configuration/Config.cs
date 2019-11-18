namespace RoleBot.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    public class Config
    {
        public const string ConfigFileName = "config.json";

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("guilds")]
        public Dictionary<ulong, List<ulong>> Guilds { get; set; }

        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("roleCheckIntervalM")]
        public int RoleCheckInterval { get; set; }

        [JsonProperty("desiredGroupName")]
        public string DesiredGroupName { get; set; }

        public Config()
        {
            Guilds = new Dictionary<ulong, List<ulong>>();
        }

        public void Save()
        {
            Save(ConfigFileName);
        }
        public void Save(string filePath)
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, data);
        }

        public static Config Load()
        {
            return Load(ConfigFileName);
        }
        public static Config Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Config not loaded because file not found.", filePath);
            }

            var data = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine($"ERROR: {filePath} database is empty.");
                return null;
            }

            return JsonConvert.DeserializeObject<Config>(data);
        }
    }
}