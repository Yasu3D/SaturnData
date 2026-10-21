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
        T contentItem = new();
        Dictionary<string, (object, PropertyInfo)> propertyAliases = [];

        getPropertyAliases(contentItem);

        for (int i = 0; i < data.Length; i++)
        {
            string line = data[i];
            if (line.StartsWith('#')) continue;
            
            Match match = SctKeyValueRegex.Match(line);
            if (!match.Success) continue;

            string key = match.Groups[1].Value;
            string value = match.Groups.Count == 3 ? match.Groups[2].Value : "";
            
            if (!propertyAliases.TryGetValue(key, out (object, PropertyInfo) propertyAlias)) continue;

            try
            {
                object source = propertyAlias.Item1;
                PropertyInfo property = propertyAlias.Item2;
                
                // Single Line Data
                if (property.PropertyType == typeof(bool))
                {
                    bool convertedValue = value == "TRUE";
                    property.SetValue(source, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(int))
                {
                    int convertedValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    property.SetValue(source, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(uint))
                {
                    uint convertedValue = Convert.ToUInt32(value, 16);
                    property.SetValue(source, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(float))
                {
                    float convertedValue = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    property.SetValue(source, convertedValue);
                    continue;
                }

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(source, value);
                    continue;
                }

                if (property.PropertyType.BaseType == typeof(Enum))
                {
                    object convertedValue = Enum.Parse(property.PropertyType, value, false);
                    property.SetValue(source, convertedValue);
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
                    List<string>? list = (List<string>?)property.GetValue(source);
                    if (list == null) continue;
                    
                    foreach (string subLine in subData)
                    {
                        if (string.IsNullOrWhiteSpace(subLine)) continue;
                        list.Add(subLine.TrimStart());
                    }

                    continue;
                }

                if (property.PropertyType == typeof(Dictionary<string, string>))
                {
                    Dictionary<string, string>? dictionary = (Dictionary<string, string>?)property.GetValue(source);
                    if (dictionary == null) continue;
                    
                    foreach (string subLine in subData)
                    {
                        Match subMatch = DictionaryKeyValueRegex.Match(subLine);
                        if (!subMatch.Success) continue;
                        
                        string subKey = subMatch.Groups[1].Value;
                        string subValue = subMatch.Groups.Count == 3 ? subMatch.Groups[2].Value : "";

                        dictionary[subKey] = subValue;
                    }

                    continue;
                }
                
                object? unknownProperty = property.GetValue(source);
                if (unknownProperty is ISctSerializable sctSerializable)
                {
                    MethodInfo? deserializeMethod = property.PropertyType.GetMethod("Deserialize");
                    if (deserializeMethod == null) continue;
                    
                    deserializeMethod.Invoke(sctSerializable, [ subData.ToArray(), ContentFormatVersion.SatContentV2 ]);

                    continue;
                }

                if (property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] genericArguments = property.PropertyType.GetGenericArguments();
                    if (genericArguments.Length != 2) continue;
                    
                    bool keyIsString = genericArguments[0] == typeof(string);
                    bool valueIsISctSerializable = genericArguments[1].GetInterfaces().Contains(typeof(ISctSerializable));

                    if (!keyIsString || !valueIsISctSerializable) continue;
                    
                    object? dictionary = property.GetValue(source);
                    PropertyInfo? indexerProperty = property.PropertyType.GetProperty("Item");
                    MethodInfo? tryGetValueMethod = property.PropertyType.GetMethod("TryGetValue");
                    MethodInfo? deserializeMethod = genericArguments[1].GetMethod("Deserialize");
                    
                    if (dictionary == null || indexerProperty == null || tryGetValueMethod == null || deserializeMethod == null) continue;

                    foreach (string subLine in subData)
                    {
                        Match subMatch = DictionaryKeyValueRegex.Match(subLine);
                        if (!subMatch.Success) continue;

                        string subKey = subMatch.Groups[1].Value;
                        string subValue = subMatch.Groups.Count == 3 ? subMatch.Groups[2].Value : "";

                        object?[] parameters = new object?[] { subKey, null };
                        tryGetValueMethod.Invoke(dictionary, parameters);

                        object? convertedValue = parameters[1] ?? Activator.CreateInstance(genericArguments[1]);
                        deserializeMethod.Invoke(convertedValue, [ new[] { subValue }, ContentFormatVersion.SatContentV2 ]);
                        indexerProperty.SetValue(dictionary, convertedValue, [ subKey ]);
                    }

                    continue;
                }
            }
            catch (Exception ex)
            {
                // TODO: cache exceptions to report them back?
                Console.WriteLine(ex);
            }
        }

        return contentItem;
        
        void getPropertyAliases(object source)
        {
            PropertyInfo[] propertyList = source.GetType().GetProperties();
            
            foreach (PropertyInfo property in propertyList)
            {
                Attribute[] attributes = property.GetCustomAttributes().ToArray();
                bool writeProperties = attributes.Any(x => x is SctWriteProperties);

                if (writeProperties)
                {
                    object? value = property.GetValue(source);
                    if (value == null) continue;

                    getPropertyAliases(value);
                    continue;
                }
                
                SctKeyAttribute? sctKey = (SctKeyAttribute?)attributes.FirstOrDefault(x => x is SctKeyAttribute);
                if (sctKey == null || sctKey.Aliases.Length == 0)
                {
                    string aliasName = property.Name.PascalCaseToMacroCase();
                    propertyAliases[aliasName] = (source, property);
                    continue;
                }

                foreach (string alias in sctKey.Aliases)
                {
                    propertyAliases[alias] = (source, property);
                }
            }
        }
    }
    
    private static readonly Regex SctKeyValueRegex = new(@"^@([A-Z_]+)(?:\s+(.*))?");
    private static readonly Regex DictionaryKeyValueRegex = new(@"^\s*-?\s*([A-Za-z_0-9.]+)(?:\s+(.*))?");
}