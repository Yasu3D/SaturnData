namespace SaturnData.Notation.Core;

/// <summary>
/// Contains information about an object's position in time.
/// </summary>
[Serializable]
public struct Timestamp : IEquatable<Timestamp>
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
            
        Time = -1;
        ScaledTime = -1;
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
            
        Time = -1;
        ScaledTime = -1;
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
    public int Measure;
        
    /// <summary>
    /// The exact subdivision of a measure this timestamp is on.
    /// </summary>
    public int Tick;

    /// <summary>
    /// Total Ticks up to this timestamp.
    /// </summary>
    public int FullTick;
        
    /// <summary>
    /// Timestamp in milliseconds.
    /// </summary>
    public float Time;

    /// <summary>
    /// Pseudo-Timestamp in milliseconds, scaled by scroll speed events.
    /// </summary>
    public float ScaledTime;
        
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

    public bool Equals(Timestamp other) => FullTick == other.FullTick;
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
    public static Timestamp operator *(Timestamp a, float b) => new((int)(a.FullTick * b));
    public static Timestamp operator /(Timestamp a, float b)
    {
        if (b == 0) throw new DivideByZeroException();
        return new((int)(a.FullTick / b));
    }
}