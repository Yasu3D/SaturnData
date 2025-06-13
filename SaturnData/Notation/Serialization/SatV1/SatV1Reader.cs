using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization.SatV2;

namespace SaturnData.Notation.Serialization.SatV1;

public static class SatV1Reader
{
    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationReadOptions options)
    {
        // SatV2 is an extension of SatV1 and fully backwards-compatible.
        return SatV2Reader.ToChart(lines, options);
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines, NotationReadOptions options)
    {
        // SatV2 is an extension of SatV1 and fully backwards-compatible.
        return SatV2Reader.ToEntry(lines, options);
    }
}