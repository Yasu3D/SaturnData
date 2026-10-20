using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Serialization;

public static class SatContentV2Reader
{
    public static T ToContentItem<T>(string[] data) where T : ContentItem, new()
    {
        PropertyInfo[] propertyList = typeof(T).GetProperties();
        T contentItem = new();

        Dictionary<string, PropertyInfo> propertyAliases = [];
        foreach (PropertyInfo property in propertyList)
        {
            Attribute[] attributes = property.GetCustomAttributes().ToArray();
            SctKeyAttribute? sctKey = (SctKeyAttribute?)attributes.FirstOrDefault(x => x is SctKeyAttribute);

            if (sctKey == null || sctKey.Aliases.Length == 0)
            {
                string aliasName = property.Name.PascalCaseToMacroCase();
                propertyAliases[aliasName] = property;
                continue;
            }

            foreach (string alias in sctKey.Aliases)
            {
                propertyAliases[alias] = property;
            }
        }

        for (int i = 0; i < data.Length; i++)
        {
            string line = data[i];
            if (line.StartsWith('#')) continue;
            
            Match match = SctKeyValueRegex.Match(line);
            if (!match.Success) continue;

            string key = match.Groups[1].Value;
            string value = match.Groups.Count == 3 ? match.Groups[2].Value : "";
            
            if (!propertyAliases.TryGetValue(key, out PropertyInfo? property)) continue;

            try
            {
                // Single Line Data
                if (property.PropertyType == typeof(bool))
                {
                    bool convertedValue = value == "TRUE";
                    property.SetValue(contentItem, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(int))
                {
                    int convertedValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    property.SetValue(contentItem, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(uint))
                {
                    uint convertedValue = Convert.ToUInt32(value, 16);
                    property.SetValue(contentItem, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(float))
                {
                    float convertedValue = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    property.SetValue(contentItem, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(contentItem, value);
                    continue;
                }

                if (property.PropertyType.BaseType == typeof(Enum))
                {
                    object convertedValue = Enum.Parse(property.PropertyType, value, false);
                    property.SetValue(contentItem, convertedValue);
                    continue;
                }
                
                // Multi Line Data
                List<string> subData = [value];
                int j = i + 1;
                while (j < data.Length)
                {
                    string subLine = data[j];
                    if (subLine.StartsWith('#'))
                    {
                        j++;
                        continue;
                    }
                    
                    Match subMatch = SctKeyValueRegex.Match(subLine);
                    if (subMatch.Success)
                    {
                        break;
                    }

                    subData.Add(subLine);
                    j++;
                }

                i = j - 1;

                if (property.PropertyType == typeof(List<string>))
                {
                    List<string>? list = (List<string>?)property.GetValue(contentItem);
                    if (list == null) continue;
                    
                    foreach (string subLine in subData)
                    {
                        if (string.IsNullOrWhiteSpace(subLine)) continue;
                        list.Add(subLine.TrimStart());
                    }
                }

                if (property.PropertyType == typeof(Dictionary<string, string>))
                {
                    Dictionary<string, string>? dictionary = (Dictionary<string, string>?)property.GetValue(contentItem);
                    if (dictionary == null) continue;
                    
                    foreach (string subLine in subData)
                    {
                        Match subMatch = DictionaryKeyValueRegex.Match(subLine);
                        if (!subMatch.Success) continue;
                        
                        string subKey = subMatch.Groups[1].Value;
                        string subValue = subMatch.Groups.Count == 3 ? subMatch.Groups[2].Value : "";

                        dictionary[subKey] = subValue;
                    }
                }
                
                object? unknownProperty = property.GetValue(contentItem);
                if (unknownProperty is ISctSerializable sctSerializable)
                {
                    MethodInfo? deserializeMethod = property.PropertyType.GetMethod("Deserialize");
                    if (deserializeMethod == null) continue;
                    
                    deserializeMethod.Invoke(sctSerializable, [ subData.ToArray(), ContentFormatVersion.SatContentV2 ]);
                }
            }
            catch (Exception ex)
            {
                // TODO: cache exceptions to report them back?
                Console.WriteLine(ex);
            }

            // List<string>
            // Dictionary<string, string>

        }

        return contentItem;
    }
    
    private static readonly Regex SctKeyValueRegex = new(@"^@([A-Z_]+)(?:\s+(.*))?");
    private static readonly Regex DictionaryKeyValueRegex = new(@"^\s*-?\s*([a-z_0-9]+)(?:\s+(.*))?");
}