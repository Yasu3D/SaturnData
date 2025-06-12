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
        // SatV2 is an extension of SatV1 and fully backwards-compatible.
        return SatV2Writer.ToString(entry, chart, options);
    }
}