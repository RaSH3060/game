using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEngine
{
    public class ConfigSystem : IUpdateable, IDrawable
    {
        private Dictionary<string, string> _configValues;
        private Dictionary<string, object> _parsedConfigs;
        
        public ConfigSystem()
        {
            _configValues = new Dictionary<string, string>();
            _parsedConfigs = new Dictionary<string, object>();
            LoadDefaultConfig();
        }

        private void LoadDefaultConfig()
        {
            // Default settings
            _configValues["Resolution.Width"] = "1024";
            _configValues["Resolution.Height"] = "768";
            _configValues["Fullscreen"] = "false";
            _configValues["Sound.Volume"] = "100";
            _configValues["Music.Volume"] = "80";
            _configValues["Difficulty"] = "Normal";
            _configValues["Controls.Forward"] = "W";
            _configValues["Controls.Backward"] = "S";
            _configValues["Controls.Left"] = "A";
            _configValues["Controls.Right"] = "D";
            _configValues["Controls.Jump"] = "Space";
            _configValues["Controls.Use"] = "E";
            _configValues["Controls.Attack"] = "LeftMouse";
        }

        public void LoadConfigFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Config file not found: {filePath}");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            string currentSection = "";

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                // Skip comments and empty lines
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("//"))
                    continue;
                    
                // Section header
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    continue;
                }
                
                // Key-value pair
                int separatorIndex = trimmedLine.IndexOf('=');
                if (separatorIndex > 0)
                {
                    string key = trimmedLine.Substring(0, separatorIndex).Trim();
                    string value = trimmedLine.Substring(separatorIndex + 1).Trim();
                    
                    if (!string.IsNullOrEmpty(currentSection))
                        key = $"{currentSection}.{key}";
                        
                    _configValues[key] = value;
                }
            }
        }

        public string GetConfigValue(string key, string defaultValue = "")
        {
            return _configValues.ContainsKey(key) ? _configValues[key] : defaultValue;
        }

        public int GetConfigValueAsInt(string key, int defaultValue = 0)
        {
            string value = GetConfigValue(key);
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }

        public float GetConfigValueAsFloat(string key, float defaultValue = 0f)
        {
            string value = GetConfigValue(key);
            if (float.TryParse(value, out float result))
                return result;
            return defaultValue;
        }

        public bool GetConfigValueAsBool(string key, bool defaultValue = false)
        {
            string value = GetConfigValue(key);
            if (bool.TryParse(value, out bool result))
                return result;
            // Handle "true"/"false" strings that might not be parsed by TryParse
            if (value.ToLower() == "true")
                return true;
            if (value.ToLower() == "false")
                return false;
            return defaultValue;
        }

        public Vector2 GetResolution()
        {
            int width = GetConfigValueAsInt("Resolution.Width", 1024);
            int height = GetConfigValueAsInt("Resolution.Height", 768);
            return new Vector2(width, height);
        }

        public bool IsFullscreen()
        {
            return GetConfigValueAsBool("Fullscreen", false);
        }

        public void Update(GameTime gameTime)
        {
            // Configuration system doesn't need real-time updates
        }

        public void Draw(GameTime gameTime)
        {
            // Configuration system doesn't draw anything
        }
    }
}