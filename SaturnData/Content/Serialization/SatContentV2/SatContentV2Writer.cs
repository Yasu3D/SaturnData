using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Serialization;

public static class SatContentV2Writer
{
    public static string ToString(ContentItem contentItem)
    {
        StringBuilder sb = new();

        sb.AppendLine("@SCT_VERSION  2");
        sb.AppendLine($"@TYPE         {contentItem.GetType().Name}");
        sb.AppendLine();
        
        write(contentItem);

        return sb.ToString();

        void write(object? objectToWrite)
        {
            if (objectToWrite == null) return;
            
            PropertyInfo[] propertyList = objectToWrite.GetType().GetProperties();
            propertyList = propertyList
            .OrderByDescending(x => x.DeclaringType == typeof(ContentItem))
            .ThenByDescending(x => x.DeclaringType == typeof(CosmeticItem))
            .ToArray();
            
            foreach (PropertyInfo property in propertyList)
            {
                Attribute[] attributes = property.GetCustomAttributes().ToArray();

                // Don't write ignored properties.
                if (attributes.Any(x => x is SctIgnoreAttribute)) continue;

                // Insert optional line break.
                if (attributes.Any(x => x is SctLineBreakAttribute))
                {
                    sb.AppendLine();
                }

                // Insert optional comment.
                SctCommentAttribute? sctComment = (SctCommentAttribute?)attributes.FirstOrDefault(x => x is SctCommentAttribute);
                if (sctComment != null)
                {
                    sb.AppendLine($"# {sctComment.Message}");
                }

                // Write data from properties of a property.
                if (attributes.Any(x => x is SctWriteProperties))
                {
                    object? value = property.GetValue(objectToWrite);
                    write(value);
                    continue;
                }
                
                // Insert key and value.
                SctKeyAttribute? sctKey = (SctKeyAttribute?)attributes.FirstOrDefault(x => x is SctKeyAttribute);
                if (sctKey == null || sctKey.Aliases.Length == 0)
                {
                    sctKey = new(property.Name.PascalCaseToMacroCase(), 30);
                }

                sb.AppendLine($"@{sctKey.Aliases[0].PadRight(sctKey.Padding)}  {ValueToString(property, objectToWrite)}");
            }
        }
    }

    public static string ValueToString(PropertyInfo property, object objectToWrite)
    {
        return ValueToString(property.GetValue(objectToWrite));
    }

    public static string ValueToString(object? value)
    {
        if (value == null) return "";
        
        if (value is bool boolValue)
        {
            return boolValue ? "TRUE" : "FALSE";
        }

        if (value is int intValue)
        {
            return intValue.ToString(CultureInfo.InvariantCulture);
        }

        if (value is uint uintValue)
        {
            return uintValue.ToString("X8", CultureInfo.InvariantCulture);
        }

        if (value is float floatValue)
        {
            return floatValue.ToString("F6", CultureInfo.InvariantCulture);
        }

        if (value is string stringValue)
        {
            return stringValue;
        }

        if (value is Enum enumValue)
        {
            return enumValue.ToString();
        }

        if (value is List<string> listValue)
        {
            if (listValue.Count == 0) return "";
            
            StringBuilder sb = new();
            sb.AppendLine();
            
            for (int i = 0; i < listValue.Count; i++)
            {
                if (i < listValue.Count - 1)
                {
                    sb.AppendLine(listValue[i].Indent());
                }
                else
                {
                    sb.Append(listValue[i].Indent());
                }
            }

            return sb.ToString();
        }

        if (value is Dictionary<string, string> dictionaryValue)
        {
            if (dictionaryValue.Count == 0) return "";
            
            StringBuilder sb = new();
            sb.AppendLine();

            int maxKeyLength = dictionaryValue.Count == 0 ? 0 : dictionaryValue.Keys.Max(x => x.Length);
            
            int count = 0;
            foreach (KeyValuePair<string, string> pair in dictionaryValue)
            {
                if (count < dictionaryValue.Count - 1)
                {
                    sb.AppendLine($"{pair.Key.PadRight(maxKeyLength)}  {pair.Value}".Indent());
                }
                else
                {
                    sb.Append($"{pair.Key.PadRight(maxKeyLength)}  {pair.Value}".Indent());
                }

                count++;
            }
            
            return sb.ToString();
        }
        
        if (value is ISctSerializable sctSerializable)
        {
            return sctSerializable.Serialize(ContentFormatVersion.SatContentV2);
        }

        Type type = value.GetType();
        Type genericDefinition = type.GetGenericTypeDefinition();
        Type[] genericArguments = type.GetGenericArguments();

        bool isDictionary = genericDefinition == typeof(Dictionary<,>);
        bool keyIsString = genericArguments.Length == 2 && genericArguments[0] == typeof(string);
        bool valueIsISctSerializable = genericArguments.Length == 2 && genericArguments[1].GetInterfaces().Contains(typeof(ISctSerializable));
        if (isDictionary && keyIsString && valueIsISctSerializable)
        {
            Dictionary<string, string> stringDictionary = new();
            
            foreach (object item in (IEnumerable)value)
            {
                if (item == null) continue;
                Type itemType = item.GetType();

                string dictKey = (string)itemType.GetProperty("Key")!.GetValue(item)!;
                ISctSerializable dictValue = (ISctSerializable)itemType.GetProperty("Value")!.GetValue(item)!;

                stringDictionary[dictKey] = dictValue.Serialize(ContentFormatVersion.SatContentV2);
            }

            return ValueToString(stringDictionary);
        }
        
        return "";
    }
}