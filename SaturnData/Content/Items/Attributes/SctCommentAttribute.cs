using System;

namespace SaturnData.Content.Items.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SctCommentAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}