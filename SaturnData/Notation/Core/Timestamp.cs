using System;
using System.Linq;
using SaturnData.Notation.Events;

namespace SaturnData.Notation.Core;

/// <summary>
/// Contains information about an object's position in time.
/// </summary>
[Serializable]
public struct Timestamp : IEquatable<Timestamp>, IComparable
{
    /// <summary>
    /// Creates a timestamp from a <c>Measure</c> and <c>Tick</c> value.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <c>measure</c> or <c>tick</c> are negative.</exception>
    public Timestamp(int measure, int tick)
    {
        if (measure < 0) throw new ArgumentException("Measure cannot be negative.");
        if (tick < 0) throw new ArgumentException("Tick cannot be negative.");
            
        Measure = measure;
        Tick = tick;

        FullTick = measure * 1920 + tick;
            
        Time = 0;
        ScaledTime = 0;
    }

    /// <summary>
    /// Creates a timestamp from a <c>FullTick</c> value.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <c>fullTick</c> is negative.</exception>
    public Timestamp(int fullTick)
    {
        if (fullTick < 0) throw new ArgumentException("Timestamp cannot be negative.");
            
        Measure = fullTick / 1920;
        Tick = fullTick - Measure * 1920;
            
        FullTick = fullTick;
            
        Time = 0;
        ScaledTime = 0;
    }
    
    /// <summary>
    /// The earliest possible timestamp.
    /// </summary>
    public static Timestamp Zero = new()
    {
        Measure = 0,
        Tick = 0,
        FullTick = 0,
        Time = 0,
        ScaledTime = 0,
    };

    /// <summary>
    /// The Measure this timestamp is on.
    /// </summary>
    public int Measure
    {
        readonly get => measure;
        set
        {
            if (measure == value) return;

            measure = value;
            fullTick = measure * 1920 + Tick;
        }
    }
    private int measure;

    /// <summary>
    /// The exact subdivision of a measure this timestamp is on.
    /// </summary>
    public int Tick
    {
        readonly get => tick;
        set
        {
            if (tick == value) return;

            tick = value;
            fullTick = measure * 1920 + tick;
        }
    }
    private int tick;

    /// <summary>
    /// Total Ticks up to this timestamp.
    /// </summary>
    public int FullTick
    {
        get => fullTick;
        set
        {
            if (fullTick == value) return;

            fullTick = value;
            measure = fullTick / 1920;
            tick = fullTick - measure * 1920;
        }
    }
    private int fullTick;
        
    /// <summary>
    /// Timestamp in milliseconds.
    /// </summary>
    public float Time { get; set; }

    /// <summary>
    /// Pseudo-Timestamp in milliseconds, scaled by scroll speed events.
    /// </summary>
    public float ScaledTime { get; set; }

    /// <summary>
    /// Returns the larger Timestamp.
    /// </summary>
    public static Timestamp Max(Timestamp a, Timestamp b) => a.FullTick > b.FullTick ? a : b;

    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c> by <c>t</c>.
    /// </summary>
    /// <param name="a">The start Timestamp</param>
    /// <param name="b">The end Timestamp</param>
    /// <param name="t">The interpolation value between the two Timestamps.</param>
    public static Timestamp Lerp(Timestamp a, Timestamp b, float t) => new((int)((1 - t) * a.FullTick + t * b.FullTick));

    public bool Equals(Timestamp other) => FullTick == other.FullTick && Time == other.Time && ScaledTime == other.ScaledTime;
    public override bool Equals(object? obj) => obj is Timestamp timestamp && Equals(timestamp);
    public override int GetHashCode() => HashCode.Combine(Measure, Tick, FullTick, Time, ScaledTime);

    public static bool operator ==(Timestamp a, Timestamp b) => a.Equals(b);
    public static bool operator !=(Timestamp a, Timestamp b) => !a.Equals(b);
    public static bool operator >(Timestamp a, Timestamp b) => a.FullTick > b.FullTick;
    public static bool operator <(Timestamp a, Timestamp b) => a.FullTick < b.FullTick;
    public static bool operator >=(Timestamp a, Timestamp b) => a.FullTick >= b.FullTick;
    public static bool operator <=(Timestamp a, Timestamp b) => a.FullTick <= b.FullTick;
    public static Timestamp operator +(Timestamp a, Timestamp b) => new(a.FullTick + b.FullTick);
    public static Timestamp operator -(Timestamp a, Timestamp b) => new(a.FullTick - b.FullTick);
    public static Timestamp operator +(Timestamp a, int b) => new(a.FullTick + b);
    public static Timestamp operator -(Timestamp a, int b) => new(a.FullTick - b);
    public static Timestamp operator *(Timestamp a, float b) => new((int)(a.FullTick * b));
    public static Timestamp operator /(Timestamp a, float b)
    {
        if (b == 0) throw new DivideByZeroException();
        return new((int)(a.FullTick / b));
    }
    
    public int CompareTo(object obj)
    {
        if (obj is not Timestamp timestamp) return 1;
        return FullTick.CompareTo(timestamp.FullTick);
    }

    /// <summary>
    /// Converts millisecond time to a timestamp struct based on all tempo and metre changes in a chart.
    /// </summary>
    /// <param name="chart">The chart to reference tempo and metre changes off of.</param>
    /// <param name="time">The time to convert in milliseconds.</param>
    /// <param name="division">The beat division to round the result to (optional)</param>
    /// <returns></returns>
    public static Timestamp TimestampFromTime(Chart chart, float time, int division = 1920)
    {
        TempoChangeEvent? tempo = chart.Events.LastOrDefault(x => x is TempoChangeEvent t && t.Timestamp.Time < time) as TempoChangeEvent;
        MetreChangeEvent? metre = chart.Events.LastOrDefault(x => x is MetreChangeEvent m && m.Timestamp.Time < time) as MetreChangeEvent;
        if (tempo == null || metre == null) return Zero;
        
        Timestamp last = Max(tempo.Timestamp, metre.Timestamp);

        float timeDifference = time - last.Time;
        int tickDifference = (int)(timeDifference / TimePerTick(tempo.Tempo, metre.Ratio));

        if (division != 1920)
        {
            tickDifference = (int)(Math.Floor(tickDifference / (1920.0f / division)) * (1920.0f / division));
        }
        
        return new(last.FullTick + tickDifference);
    }
    
    /// <summary>
    /// Converts a timestamp struct to millisecond time based on all tempo and metre changes in a chart.
    /// </summary>
    /// <param name="chart">The chart to reference tempo and metre changes off of.</param>
    /// <param name="timestamp">The timestamp struct to convert.</param>
    /// <returns></returns>
    public static float TimeFromTimestamp(Chart chart, Timestamp timestamp)
    {
        TempoChangeEvent? tempo = chart.Events.LastOrDefault(x => x is TempoChangeEvent t && t.Timestamp < timestamp) as TempoChangeEvent;
        MetreChangeEvent? metre = chart.Events.LastOrDefault(x => x is MetreChangeEvent m && m.Timestamp < timestamp) as MetreChangeEvent;
        if (tempo == null || metre == null) return 0;
        
        Timestamp last = Max(tempo.Timestamp, metre.Timestamp);

        int tickDifference = timestamp.FullTick - last.FullTick;
        float timeDifference = tickDifference * TimePerTick(tempo.Tempo, metre.Ratio);
        return last.Time + timeDifference;
    }

    /// <summary>
    /// Converts millisecond time into scaled time based on all speed changes in a layer.
    /// </summary>
    /// <param name="layer">The layer to reference speed changes off of.</param>
    /// <param name="time">The time to convert in milliseconds.</param>
    /// <returns></returns>
    public static float ScaledTimeFromTime(Layer layer, float time)
    {
        SpeedChangeEvent? speed = layer.Events.LastOrDefault(x => x is SpeedChangeEvent s && s.Timestamp.Time < time) as SpeedChangeEvent;
        if (speed == null) return time; // This is fine, just means there are no speed changes.

        float timeDifference = time - speed.Timestamp.Time;
        float scaledTimeDifference = timeDifference * speed.HiSpeed;
        
        return speed.Timestamp.ScaledTime + scaledTimeDifference;
    }

    /// <summary>
    /// The length of a tick (1/1920th of a measure) in milliseconds at a specific tempo and metre.
    /// </summary>
    /// <param name="tempo">The tempo to use.</param>
    /// <param name="ratio">The metre ratio use.</param>
    /// <returns></returns>
    public static float TimePerTick(float tempo, float ratio) => (240.0f / tempo * ratio / 1920.0f) * 1000.0f;
}