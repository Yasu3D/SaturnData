using System;

namespace SaturnData.Content.Serialization;

public interface ISctSerializable
{
    public string Serialize();

    public static ISctSerializable Deserialize(string[] data) => throw new NotImplementedException();
}