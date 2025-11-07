using System;
using System.Collections.Generic;
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
    public static Chart ToChart(string[] lines, NotationReadArgs args, out List<Exception> exceptions)
    {
        // SatV2 is an extension of SatV1 and fully backwards-compatible.
        return SatV2Reader.ToChart(lines, args, out exceptions);
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines, NotationReadArgs args, out List<Exception> exceptions, string path = "")
    {
        // SatV2 is an extension of SatV1 and fully backwards-compatible.
        Entry entry = SatV2Reader.ToEntry(lines, args, out exceptions, path);
        entry.FormatVersion = FormatVersion.SatV1;

        return entry;
    }
}