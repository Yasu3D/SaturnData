using System;

namespace SaturnData.Content.Items.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SctKeyAttribute : Attribute
{
    public SctKeyAttribute(string[] aliases, int padding)
    {
        Aliases = aliases;
        Padding = padding;
    }

    public SctKeyAttribute(string sctAlias, int padding)
    {
        Aliases = [sctAlias];
        Padding = padding;
    }

    public string[] Aliases { get; }
    public int Padding { get; }
}