using System;

namespace SaturnData.Utilities;

public class SaturnMath
{
    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c>, controlled by <c>t</c>
    /// </summary>
    public static float Lerp(float a, float b, float t) => a + t * (b - a);
    
    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c>, controlled by <c>t</c>, cycled by <c>m</c>
    /// </summary>
    public static float LerpCyclic(float a, float b, float t, float m)
    {
        if (Math.Abs(a - b) <= m * 0.5f) return a + t * (b - a);
        
        if (a > b) b += m;
        else a += m;

        return a + t * (b - a);
    }
    
    /// <summary>
    /// Returns where <c>x</c> lies between <c>a</c> and <c>b</c>.
    /// </summary>
    public static float InverseLerp(float a, float b, float x) => a == b ? 0 : (x - a) / (b - a);
}