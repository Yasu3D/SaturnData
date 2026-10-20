namespace SaturnData.Content.Serialization;

public interface ISctSerializable
{
    /// <summary>
    /// Writes the current object in the <c>.sct</c> format.
    /// </summary>
    /// <param name="formatVersion">The format version to write in.</param>
    /// <returns></returns>
    public string Serialize(ContentFormatVersion formatVersion);

    /// <summary>
    /// Reads the provided data and applies its values to the object this method is called on.
    /// </summary>
    /// <param name="data">The lines of data to deserialize.</param>
    /// <param name="formatVersion">The format version of the provided data.</param>
    public void Deserialize(string[] data, ContentFormatVersion formatVersion);
}