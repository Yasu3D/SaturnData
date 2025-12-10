using System;
using System.Collections.Generic;
using SaturnData.Notation.Events;
using SaturnData.Utilities;

namespace SaturnData.Notation.Core;

/// <summary>
/// Contains information about an object's position in time.
/// </summary>
[Serializable]
public class Timestamp : IEquatable<Timestamp>, IComparable
{
    /// <summary>
    /// Creates a timestamp from another timestamp.
    /// </summary>
    public Timestamp(Timestamp cloneSource)
    {
        Measure = cloneSource.measure;
        Tick = cloneSource.Tick;
        FullTick = cloneSource.FullTick;
        Time = cloneSource.Time;
        ScaledTime = cloneSource.ScaledTime;
    }
    
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

            measure = Math.Max(0, value);
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

            tick = Math.Clamp(value, 0, 1919);
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
    /// Returns the larger timestamp.
    /// </summary>
    public static Timestamp Max(Timestamp a, Timestamp b) => a.FullTick > b.FullTick ? a : b;

    /// <summary>
    /// Returns the smaller timestamp.
    /// </summary>
    public static Timestamp Min(Timestamp a, Timestamp b) => a.FullTick < b.FullTick ? a : b;
    
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
    /// Converts millisecond time to a timestamp based on all tempo and metre changes in a chart.
    /// </summary>
    /// <param name="chart">The chart to reference tempo and metre changes off of.</param>
    /// <param name="time">The time in milliseconds to convert to a timestamp.</param>
    /// <param name="division">The beat division to round the result to. (optional)</param>
    public static Timestamp TimestampFromTime(Chart chart, float time, int division = 1920)
    {
        TempoChangeEvent? lastTempoChange = chart.LastTempoChange(time);
        MetreChangeEvent? lastMetreChange = chart.LastMetreChange(time);
        if (lastTempoChange == null || lastMetreChange == null) return Zero;

        return TimestampFromTime(lastTempoChange, lastMetreChange, time, division);
    }

    /// <summary>
    /// Converts millisecond time to a timestamp based on all provided tempo and metre changes.
    /// </summary>
    /// <param name="tempoChanges">The collection of tempo changes.</param>
    /// <param name="metreChanges">The collection of metre changes.</param>
    /// <param name="time">The time in milliseconds to convert to a timestamp.</param>
    /// <param name="division">The beat division to round the result to. (optional)</param>
    /// <returns></returns>
    public static Timestamp TimestampFromTime(List<TempoChangeEvent> tempoChanges, List<MetreChangeEvent> metreChanges, float time, int division = 1920)
    {
        TempoChangeEvent? lastTempoChange = BinarySearch.Last(tempoChanges, x => x.Timestamp.Time < time);
        MetreChangeEvent? lastMetreChange = BinarySearch.Last(metreChanges, x => x.Timestamp.Time < time);
        if (lastTempoChange == null || lastMetreChange == null) return Zero;
        
        return TimestampFromTime(lastTempoChange, lastMetreChange, time, division);
    }
    
    /// <summary>
    /// Converts millisecond time to a timestamp based on the provided tempo and metre changes.
    /// </summary>
    /// <param name="lastTempoChange">The last tempo change.</param>
    /// <param name="lastMetreChange">The last metre change.</param>
    /// <param name="time">The time in milliseconds to convert to a timestamp.</param>
    /// <param name="division">The beat division to round the result to. (optional)</param>
    /// <returns></returns>
    private static Timestamp TimestampFromTime(TempoChangeEvent lastTempoChange, MetreChangeEvent lastMetreChange, float time, int division = 1920)
    {
        Timestamp last = Max(lastTempoChange.Timestamp, lastMetreChange.Timestamp);

        float timeDifference = time - last.Time;
        int tickDifference = (int)(timeDifference / TimePerTick(lastTempoChange.Tempo, lastMetreChange.Ratio));

        int fullTick = last.FullTick + tickDifference;
        
        if (division != 1920)
        {
            float m = 1920.0f / division;
            fullTick = (int)(Math.Floor(fullTick / m) * m);
        }

        fullTick = Math.Max(last.FullTick, fullTick);
        
        return new(fullTick) { Time = time };
    }
    
    /// <summary>
    /// Converts a timestamp struct to millisecond time based on all tempo and metre changes in a chart.
    /// </summary>
    /// <param name="chart">The chart to reference tempo and metre changes off of.</param>
    /// <param name="timestamp">The timestamp to convert to milliseconds.</param>
    public static float TimeFromTimestamp(Chart chart, Timestamp timestamp)
    {
        TempoChangeEvent? lastTempoChange = chart.LastTempoChange(timestamp);
        MetreChangeEvent? lastMetreChange = chart.LastMetreChange(timestamp);
        if (lastTempoChange == null || lastMetreChange == null) return 0;

        return TimeFromTimestamp(lastTempoChange, lastMetreChange, timestamp);
    }

    /// <summary>
    /// Converts a timestamp struct to millisecond time based on all provided tempo and metre changes.
    /// </summary>
    /// <param name="tempoChangeEvents">The collection of tempo changes.</param>
    /// <param name="metreChangeEvents">The collection of metre changes.</param>
    /// <param name="timestamp">The timestamp to convert to milliseconds.</param>
    /// <returns></returns>
    public static float TimeFromTimestamp(List<TempoChangeEvent> tempoChanges, List<MetreChangeEvent> metreChanges, Timestamp timestamp)
    {
        TempoChangeEvent? lastTempoChange = BinarySearch.Last(tempoChanges, x => x.Timestamp.FullTick < timestamp.FullTick);
        MetreChangeEvent? lastMetreChange = BinarySearch.Last(metreChanges, x => x.Timestamp.FullTick < timestamp.FullTick);
        if (lastTempoChange == null || lastMetreChange == null) return 0;

        return TimeFromTimestamp(lastTempoChange, lastMetreChange, timestamp);
    }
    
    /// <summary>
    /// Converts a timestamp struct to millisecond time based on the provided tempo and metre changes.
    /// </summary>
    /// <param name="lastTempoChange">The last tempo change.</param>
    /// <param name="lastMetreChange">The last metre change.</param>
    /// <param name="timestamp">The timestamp to convert to milliseconds.</param>
    /// <returns></returns>
    private static float TimeFromTimestamp(TempoChangeEvent lastTempoChange, MetreChangeEvent lastMetreChange, Timestamp timestamp)
    {
        Timestamp last = Max(lastTempoChange.Timestamp, lastMetreChange.Timestamp);

        int tickDifference = timestamp.FullTick - last.FullTick;
        float timeDifference = tickDifference * TimePerTick(lastTempoChange.Tempo, lastMetreChange.Ratio);
        return last.Time + timeDifference;
    }
    
    /// <summary>
    /// Converts millisecond time into scaled time based on all speed changes in a layer.
    /// </summary>
    /// <param name="layer">The layer to reference speed changes off of.</param>
    /// <param name="time">The time in milliseconds to convert.</param>
    public static float ScaledTimeFromTime(Layer layer, float time, bool ignoreReverse = false)
    {
        // Reverse Effects overrule all other speed changes.
        ReverseEffectEvent? lastReverseEffect = layer.LastReverseEffect(time);
        
        if (!ignoreReverse && lastReverseEffect != null && lastReverseEffect.IsActive(time))
        {
            return lastReverseEffect.SampleReversedTime(time);
        }
        
        SpeedChangeEvent? speedChange = layer.LastSpeedChange(time);
        StopEffectEvent? stopEffect = layer.LastStopEffect(time);

        return ScaledTimeFromTime(speedChange, stopEffect, time);
    }

    /// <summary>
    /// Converts millisecond time into scaled time based on all provided speed changes and effects.
    /// </summary>
    /// <param name="speedChanges">The collection of speed changes.</param>
    /// <param name="stopEffects">The collection of stop effects.</param>
    /// <param name="reverseEffects">The collection of reverse effects.</param>
    /// <param name="time">The time in milliseconds to convert.</param>
    /// <param name="ignoreReverse"></param>
    /// <returns></returns>
    public static float ScaledTimeFromTime(List<SpeedChangeEvent> speedChanges, List<StopEffectEvent> stopEffects, List<ReverseEffectEvent> reverseEffects, float time, bool ignoreReverse = false)
    {
        ReverseEffectEvent? lastReverseEffect = time == 0 
        ? BinarySearch.Last(reverseEffects, x => x.Timestamp.Time == 0)
        : BinarySearch.Last(reverseEffects, x => x.Timestamp.Time < time);

        if (!ignoreReverse && lastReverseEffect != null && lastReverseEffect.IsActive(time))
        {
            return lastReverseEffect.SampleReversedTime(time);
        }

        SpeedChangeEvent? lastSpeedChange = time == 0
            ? BinarySearch.Last(speedChanges, x => x.Timestamp.Time == 0)
            : BinarySearch.Last(speedChanges, x => x.Timestamp.Time < time);
        
        StopEffectEvent? lastStopEffect = time == 0
            ? BinarySearch.Last(stopEffects, x => x.Timestamp.Time == 0)
            : BinarySearch.Last(stopEffects, x => x.Timestamp.Time < time);

        return ScaledTimeFromTime(lastSpeedChange, lastStopEffect, time);
    }

    /// <summary>
    /// Converts millisecond time into scaled time based on the provided speed change and stop effect.
    /// </summary>
    /// <param name="lastSpeedChange">The last speed change.</param>
    /// <param name="lastStopEffect">The last stop effect.</param>
    /// <param name="time">The time in milliseconds to convert.</param>
    /// <returns></returns>
    private static float ScaledTimeFromTime(SpeedChangeEvent? lastSpeedChange, StopEffectEvent? lastStopEffect, float time)
    {
        if (lastSpeedChange == null && lastStopEffect == null) return time;

        float eventTime;
        float eventScaledTime;
        float scrollSpeed;

        if (lastSpeedChange != null && lastStopEffect == null)
        {
            // Speed change exists - Stop doesn't exist.
            eventTime = lastSpeedChange.Timestamp.Time;
            eventScaledTime = lastSpeedChange.Timestamp.ScaledTime;
            scrollSpeed = lastSpeedChange.Speed;
        }
        else if (lastSpeedChange == null && lastStopEffect != null)
        {
            // Speed change doesn't exist - Stop exists.
            bool stopPassed = time > lastStopEffect.SubEvents[1].Timestamp.Time;
            
            eventTime = stopPassed 
                ? lastStopEffect.SubEvents[1].Timestamp.Time 
                : lastStopEffect.SubEvents[0].Timestamp.Time;
            
            eventScaledTime = stopPassed 
                ? lastStopEffect.SubEvents[1].Timestamp.ScaledTime 
                : lastStopEffect.SubEvents[0].Timestamp.ScaledTime;

            scrollSpeed = stopPassed ? 1 : 0;
        }
        else
        {
            // Speed change exists - Stop exists.
            
            if (lastSpeedChange!.Timestamp.Time > lastStopEffect!.SubEvents[1].Timestamp.Time)
            {
                // Speed change comes after the stop.
                // Ignore the stop.
                eventTime = lastSpeedChange.Timestamp.Time;
                eventScaledTime = lastSpeedChange.Timestamp.ScaledTime;
                scrollSpeed = lastSpeedChange.Speed;
            }
            else
            {
                // Speed change comes during or before the stop.
                // It gets overruled by the stop until it has passed.
                bool stopPassed = time > lastStopEffect.SubEvents[1].Timestamp.Time;
            
                eventTime = stopPassed 
                    ? lastStopEffect.SubEvents[1].Timestamp.Time 
                    : lastStopEffect.SubEvents[0].Timestamp.Time;
                
                eventScaledTime = stopPassed 
                    ? lastStopEffect.SubEvents[1].Timestamp.ScaledTime 
                    : lastStopEffect.SubEvents[0].Timestamp.ScaledTime;

                scrollSpeed = stopPassed ? lastSpeedChange.Speed : 0;
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tick"></param>
    /// <returns></returns>
    public static int BeatFromTick(int tick, int division) => (int)Math.Round(division * tick / 1920.0);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="beat"></param>
    /// <returns></returns>
    public static int TickFromBeat(int beat, int division) => 1920 * beat / division;
}