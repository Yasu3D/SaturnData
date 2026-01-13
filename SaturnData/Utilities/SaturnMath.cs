using System;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Utilities;

public static class SaturnMath
{
    /// <summary>
    /// Returns the Euclidean remainder of <c>x / m</c>.
    /// </summary>
    /// <returns></returns>
    public static int Modulo(int x, int m)
    {
        if (m <= 0) return 0;
        
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    
    public static float Modulo(float x, int m)
    {
        if (m <= 0) return 0;
        
        float r = x % m;
        return r < 0 ? r + m : r;
    }
    
    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c>, controlled by <c>t</c>
    /// </summary>
    public static float Lerp(float a, float b, float t) => a + t * (b - a);
    
    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c>, controlled by <c>t</c>, cycled by <c>m</c>
    /// </summary>
    public static float LerpCyclic(float a, float b, float t, float m)
    {
        return LerpCyclicManual(a, b, t, m, Math.Abs(a - b) <= m * 0.5f);
    }
    
    /// <summary>
    /// Linearly interpolates between <c>a</c> and <c>b</c>, controlled by <c>t</c>, cycled by <c>m</c>.<br/>
    /// Shortcutting is controlled by <c>shortcut</c>
    /// </summary>
    public static float LerpCyclicManual(float a, float b, float t, float m, bool shortcut)
    {
        if (!shortcut) return a + t * (b - a);
        
        if (a > b) b += m;
        else a += m;

        return a + t * (b - a);
    }
    
    /// <summary>
    /// Determines if the direction a Hold Note is interpolated in should be reversed.
    /// </summary>
    /// <param name="start">The start positionable.</param>
    /// <param name="end">The end positionable.</param>
    /// <returns></returns>
    public static bool FlipHoldInterpolation(IPositionable start, IPositionable end)
    {
        float startCenter = (start.Position + start.Size * 0.5f) * -6;
        float endCenter   = (end.Position   + end.Size   * 0.5f) * -6;

        return FlipHoldInterpolation(startCenter, endCenter);
    }

    /// <summary>
    /// Determines if the direction a Hold Note is interpolated in should be reversed.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool FlipHoldInterpolation(float start, float end) => Math.Abs(start - end) > 180;
    
    /// <summary>
    /// Returns where <c>x</c> lies between <c>a</c> and <c>b</c>.
    /// </summary>
    public static float InverseLerp(float a, float b, float x) => a == b ? 0 : (x - a) / (b - a);
}