using SaturnData.Notation.Core;

namespace SaturnData.Notation.Serialization.SatV1;

public class SatV1Serializer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart)
    {
        return "";
    }

    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines)
    {
        return null;
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines)
    {
        return null;
    }
}