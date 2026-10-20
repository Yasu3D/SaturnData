using System;
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

        PropertyInfo[] propertyList = contentItem.GetType().GetProperties();
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

            // Insert key and value.
            SctKeyAttribute? sctKey = (SctKeyAttribute?)attributes.FirstOrDefault(x => x is SctKeyAttribute);
            if (sctKey == null || sctKey.Aliases.Length == 0)
            {
                sctKey = new(property.Name.PascalCaseToMacroCase(), 30);
            }

            sb.AppendLine($"@{sctKey.Aliases[0].PadRight(sctKey.Padding)}  {ValueToString(property, contentItem)}");
        }

        return sb.ToString();
    }

    public static string ValueToString(PropertyInfo property, ContentItem contentItem)
    {
        return ValueToString(property.GetValue(contentItem));
    }

    public static string ValueToString(object? value)
    {
        if (value is bool b)
        {
            return b ? "TRUE" : "FALSE";
        }

        if (value is int i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }

        if (value is uint u)
        {
            return u.ToString("X8", CultureInfo.InvariantCulture);
        }

        if (value is float f)
        {
            return f.ToString("F6", CultureInfo.InvariantCulture);
        }

        if (value is string s)
        {
            return s;
        }

        if (value is Enum e)
        {
            return e.ToString();
        }

        if (value is List<string> list)
        {
            if (list.Count == 0) return "";
            
            StringBuilder sb = new();
            sb.AppendLine();
            
            for (int index = 0; index < list.Count; index++)
            {
                string item = list[index];

                if (index < list.Count - 1)
                {
                    sb.AppendLine($"{ContentSerializationHelpers.Indent}{item}");
                }
                else
                {
                    sb.Append($"    {item}");
                }
            }

            return sb.ToString();
        }

        if (value is Dictionary<string, string> dictionary)
        {
            if (dictionary.Count == 0) return "";
            
            StringBuilder sb = new();
            sb.AppendLine();

            int maxKeyLength = dictionary.Count == 0 ? 0 : dictionary.Keys.Max(x => x.Length);
            
            int count = 0;
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (count < dictionary.Count - 1)
                {
                    sb.AppendLine($"{ContentSerializationHelpers.Indent}{pair.Key.PadRight(maxKeyLength)}  {pair.Value}");
                }
                else
                {
                    sb.Append($"{ContentSerializationHelpers.Indent}{pair.Key.PadRight(maxKeyLength)}  {pair.Value}");
                }

                count++;
            }
            
            return sb.ToString();
        }
        
        if (value is ISctSerializable sctSerializable)
        {
            return sctSerializable.Serialize();
        }
        
        return "";
    }
}