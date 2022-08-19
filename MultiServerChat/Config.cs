using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MultiServerChat
{
	public class ConfigFile
	{
        public List<string> RestURLs = new List<string>();
        public string Token = "abcdef";
        public string ChatFormat = "[{0}] {1}";
        public string JoinFormat = "[{0}] {1} has joined.";
        public string GeoJoinFormat = "[{0}] {1} ({2}) has joined.";
        public string LeaveFormat = "[{0}] {1} has left.";
        public bool SendChat = true;
        public bool SendJoinLeave = true;
        public bool DisplayChat = true;
        public bool DisplayJoinLeave = true;

        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
            {
                ConfigFile config = new ConfigFile();
                config.RestURLs.Add("http://server1.com:7878");
                config.RestURLs.Add("http://server1.com:8205");
                config.RestURLs.Add("http://server2.com:7878");

                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
                return config;
            }
            return JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(path));
        }

        /// <summary>
        /// On config read hook
        /// </summary>
		public static Action<ConfigFile> ConfigRead;
	}
}
