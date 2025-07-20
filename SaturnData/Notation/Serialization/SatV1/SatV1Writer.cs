using System.Text;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization.SatV2;

namespace SaturnData.Notation.Serialization.SatV1;

public static class SatV1Writer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationWriteOptions options)
    {
        return SatV2Writer.ToString(entry, chart, options);
    }

    public static void WriteMetadata(StringBuilder sb, Entry entry, NotationWriteOptions options) => SatV2Writer.WriteMetadata(sb, entry, options);
    public static void WriteBookmarks(StringBuilder sb, Chart chart, NotationWriteOptions options) => SatV2Writer.WriteBookmarks(sb, chart, options);
    public static void WriteEvents(StringBuilder sb, Chart chart, Entry entry, NotationWriteOptions options) => SatV2Writer.WriteEvents(sb, chart, entry, options);
    public static void WriteNotes(StringBuilder sb, Chart chart, NotationWriteOptions options) => SatV2Writer.WriteNotes(sb, chart, options);
}