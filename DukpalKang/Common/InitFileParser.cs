using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasingDeokPal.Common
{
    internal class InitFileParser
    {
        private readonly Dictionary<string, Dictionary<string, string>> _iniData;

        public InitFileParser(string filePath)
        {
            _iniData = new Dictionary<string, Dictionary<string, string>>();
            ParseFile(filePath);
        }

        private void ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("INI file not found.", filePath);

            string currentSection = null;
            foreach (var line in File.ReadLines(filePath))
            {
                // Trim whitespace
                var trimmedLine = line.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;

                // Check for section header
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.TrimStart('[').TrimEnd(']');
                    _iniData[currentSection] = new Dictionary<string, string>();
                }
                else if (currentSection != null)
                {
                    // Key-value pair
                    var keyValue = trimmedLine.Split(new[] { ':' }, 2);
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();
                        _iniData[currentSection][key] = value;
                    }
                }
            }
        }

        public string GetValue(string section, string key)
        {
            if (_iniData.TryGetValue(section, out var sectionData))
            {
                if (sectionData.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Json 형식 ini 파일 read
    /// </summary>
    internal class IniJsonParser
    {
        private string? filePath;
        public IniJsonParser(string path)
        {
            filePath = path;
        }

        /// <summary>
        /// Json 문자열 반환
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public string ParseFile()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("INI file not found.", filePath);
            // json read
            string jsonString = "";
            foreach(var line in File.ReadAllLines(filePath))
            {
                jsonString += line;
            }
            return jsonString;
        }

        public T ParseFile<T>()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("INI file not found.", filePath);
            // json read
            string jsonString = "";
            foreach (var line in File.ReadAllLines(filePath))
            {
                jsonString += line;
            }
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<T>(jsonString);
                return data;
            }
            catch(Exception e)
            {
                throw new FileNotFoundException("INI file 변환 오류.", filePath);
            }
        }
    }
}
