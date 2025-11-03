using System.Text;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization.SatV2;

namespace SaturnData.Notation.Serialization.SatV1;

public static class SatV1Writer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="chart">The chart to serialize.</param>
    /// <remarks>
    /// This overload doesn't write any metadata. Certain format specs may not support this.
    /// </remarks>
    public static string ToString(Chart chart, NotationWriteArgs args)
    {
        return ToString(null, chart, args);
    }
    
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry? entry, Chart chart, NotationWriteArgs args)
    {
        return SatV2Writer.ToString(entry, chart, args);
    }

    public static void WriteMetadata(StringBuilder sb, Entry entry, NotationWriteArgs args) => SatV2Writer.WriteMetadata(sb, entry, args);
    public static void WriteBookmarks(StringBuilder sb, Chart chart, NotationWriteArgs args) => SatV2Writer.WriteBookmarks(sb, chart, args);
    public static void WriteEvents(StringBuilder sb, Chart chart, Entry? entry, NotationWriteArgs args) => SatV2Writer.WriteEvents(sb, chart, entry, args);
    public static void WriteNotes(StringBuilder sb, Chart chart, NotationWriteArgs args) => SatV2Writer.WriteNotes(sb, chart, args);
}