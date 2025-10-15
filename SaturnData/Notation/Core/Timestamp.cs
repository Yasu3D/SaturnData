using System;
using SaturnData.Notation.Events;

namespace SaturnData.Notation.Core;

/// <summary>
/// Contains information about an object's position in time.
/// </summary>
[Serializable]
public class Timestamp : IEquatable<Timestamp>, IComparable
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
    public static Timestamp Zero => new(0)
    {
        Time = 0,
        ScaledTime = 0,
    };

    /// <summary>
    /// The Measure this timestamp is on.
    /// </summary>
    public int Measure
    {
        get => measure;
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
        get => tick;
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
    public static Timestamp Lerp(Timestamp a, Timestamp b, float t)
    {
        int fullTick = (int)((1 - t) * a.FullTick + t * b.FullTick);
        return new(fullTick);
    }

    public bool Equals(Timestamp? other)
    {
        return other is not null && FullTick == other.FullTick && Time == other.Time && ScaledTime == other.ScaledTime;
    }

    public override bool Equals(object? obj)
    {
        return obj is Timestamp timestamp && Equals(timestamp);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Measure, Tick, FullTick);
    }

    public static bool operator ==(Timestamp? a, Timestamp? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(Timestamp? a, Timestamp? b) => !a?.Equals(b) ?? b is not null;
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
    
    public int CompareTo(object? obj)
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
    public static Timestamp TimestampFromTime(Chart chart, float time, int division = 1920)
    {
        TempoChangeEvent? tempo = chart.LastTempoChange(time);
        MetreChangeEvent? metre = chart.LastMetreChange(time);
        if (tempo == null || metre == null) return Zero;
        
        Timestamp last = Max(tempo.Timestamp, metre.Timestamp);

        float timeDifference = time - last.Time;
        int tickDifference = (int)(timeDifference / TimePerTick(tempo.Tempo, metre.Ratio));

        if (division != 1920)
        {
            tickDifference = (int)(Math.Floor(tickDifference / (1920.0f / division)) * (1920.0f / division));
        }
        
        return new(last.FullTick + tickDifference) { Time = time };
    }
    
    /// <summary>
    /// Converts a timestamp struct to millisecond time based on all tempo and metre changes in a chart.
    /// </summary>
    /// <param name="chart">The chart to reference tempo and metre changes off of.</param>
    /// <param name="timestamp">The timestamp struct to convert.</param>
    public static float TimeFromTimestamp(Chart chart, Timestamp timestamp)
    {
        TempoChangeEvent? tempo = chart.LastTempoChange(timestamp);
        MetreChangeEvent? metre = chart.LastMetreChange(timestamp);
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
    public static float ScaledTimeFromTime(Layer layer, float time)
    {
        // Reverse Effects overrule all other speed changes.
        ReverseEffectEvent? reverseEffect = layer.LastReverseEffect(time);
        if (reverseEffect != null && reverseEffect.IsActive(time))
        {
            return reverseEffect.SampleReversedTime(time);
        }
        
        SpeedChangeEvent? speedChange = layer.LastSpeedChange(time);
        StopEffectEvent? stopEffect = layer.LastStopEffect(time);
        
        if (speedChange == null && stopEffect == null) return time; // This is fine, just means there are no speed changes or stops.

        float eventTime;
        float eventScaledTime;
        float scrollSpeed;

        if (speedChange != null && stopEffect == null)
        {
            // Speed change exists - Stop doesn't exist.
            eventTime = speedChange.Timestamp.Time;
            eventScaledTime = speedChange.Timestamp.ScaledTime;
            scrollSpeed = speedChange.Speed;
        }
        else if (speedChange == null && stopEffect != null)
        {
            // Speed change doesn't exist - Stop exists.
            bool stopPassed = time > stopEffect.SubEvents[1].Timestamp.Time;
            
            eventTime = stopPassed 
                ? stopEffect.SubEvents[1].Timestamp.Time 
                : stopEffect.SubEvents[0].Timestamp.Time;
            
            eventScaledTime = stopPassed 
                ? stopEffect.SubEvents[1].Timestamp.ScaledTime 
                : stopEffect.SubEvents[0].Timestamp.ScaledTime;

            scrollSpeed = stopPassed ? 1 : 0;
        }
        else
        {
            // Speed change exists - Stop exists.
            
            if (speedChange!.Timestamp.Time > stopEffect!.SubEvents[1].Timestamp.Time)
            {
                // Speed change comes after the stop.
                // Ignore the stop.
                eventTime = speedChange.Timestamp.Time;
                eventScaledTime = speedChange.Timestamp.ScaledTime;
                scrollSpeed = speedChange.Speed;
            }
            else
            {
                // Speed change comes during or before the stop.
                // It gets overruled by the stop until it has passed.
                bool stopPassed = time > stopEffect.SubEvents[1].Timestamp.Time;
            
                eventTime = stopPassed 
                    ? stopEffect.SubEvents[1].Timestamp.Time 
                    : stopEffect.SubEvents[0].Timestamp.Time;
                
                eventScaledTime = stopPassed 
                    ? stopEffect.SubEvents[1].Timestamp.ScaledTime 
                    : stopEffect.SubEvents[0].Timestamp.ScaledTime;

                scrollSpeed = stopPassed ? speedChange.Speed : 0;
            }
        }
        
        float timeDifference = time - eventTime;
        float scaledTimeDifference = timeDifference * scrollSpeed;
        
        return eventScaledTime + scaledTimeDifference;
    }

    /// <summary>
    /// The length of a tick (1/1920th of a measure) in milliseconds at a specific tempo and metre.
    /// </summary>
    /// <param name="tempo">The tempo to use.</param>
    /// <param name="ratio">The metre ratio use.</param>
    public static float TimePerTick(float tempo, float ratio) => (240.0f / tempo * ratio / 1920.0f) * 1000.0f;
}