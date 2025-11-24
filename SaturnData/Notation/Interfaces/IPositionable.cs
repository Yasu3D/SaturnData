using System;

namespace SaturnData.Notation.Interfaces;

/// <summary>
/// Implements position and size attributes.
/// </summary>
public interface IPositionable
{
    public enum OverlapResult
    {
        None = 0,
        Body = 1,
        LeftEdge = 2,
        RightEdge = 3,
    }
    
    /// <summary>
    /// The position of the IPositionable.<br/>
    /// Position starts at 3 o'clock/East, and steps Counterclockwise in 6° increments.
    /// </summary>
    /// <remarks>
    /// Range [0 - 59]
    /// </remarks>
    public int Position { get; set; }

    /// <summary>
    /// The size of the IPositionable.<br/>
    /// Increasing values makes a Note grow Counterclockwise.
    /// </summary>
    /// <remarks>
    /// Range [1 - 60]
    /// </remarks>
    public int Size { get; set; }
    
    /// <summary>
    /// Returns <c>true</c> if two <c>IPositionable</c>s overlap on any lane.
    /// </summary>
    /// <param name="a">The first positionable to compare.</param>
    /// <param name="b">The second positionable to compare.</param>
    public static bool IsAnyOverlap(IPositionable a, IPositionable b)
    {
        return IsAnyOverlap(a.Position, a.Size, b.Position, b.Size);
    }

    /// <summary>
    /// Returns <c>true</c> if the defined shapes overlap on any lane.
    /// </summary>
    /// <param name="posA">The first position to compare.</param>
    /// <param name="sizeA">The first size to compare.</param>
    /// <param name="posB">The second position to compare.</param>
    /// <param name="sizeB">The second size to compare.</param>
    public static bool IsAnyOverlap(int posA, int sizeA, int posB, int sizeB)
    {
        if (IsIdenticalOverlap(posA, sizeA, posB, sizeB)) return true;
        if (sizeA == 60 || sizeB == 60) return true;
        
        bool[] occupiedLanes = new bool[60];

        for (int i = posA; i < posA + sizeA; i++)
        {
            occupiedLanes[i % 60] = true;
        }

        for (int i = posB; i < posB + sizeB; i++)
        {
            if (occupiedLanes[i % 60]) return true;
        }
        
        return false;
    }

    /// <summary>
    /// Returns <c>true</c> if one <c>IPositionable</c> fully overlaps another.
    /// </summary>
    /// <param name="a">The first positionable to compare.</param>
    /// <param name="b">The second positionable to compare.</param>
    public static bool IsFullOverlap(IPositionable a, IPositionable b)
    {
        return IsFullOverlap(a.Position, a.Size, b.Position, b.Size);
    }

    /// <summary>
    /// Returns <c>true</c> if one defined shape fully overlaps another.
    /// </summary>
    /// <param name="posA">The first position to compare.</param>
    /// <param name="sizeA">The first size to compare.</param>
    /// <param name="posB">The second position to compare.</param>
    /// <param name="sizeB">The second size to compare.</param>
    public static bool IsFullOverlap(int posA, int sizeA, int posB, int sizeB)
    {
        if (IsIdenticalOverlap(posA, sizeA, posB, sizeB)) return true;
        if (sizeA == 60 || sizeB == 60) return true;

        // IsIdenticalOverlap() already checked if the positions are equal.
        // If the size is the same here, then the position must be different -> no enclosing overlap.
        if (sizeA == sizeB) return false;
        
        int largerPos = sizeA > sizeB ? posA : posB;
        int largerSize = Math.Max(sizeA, sizeB);
        
        int smallerPos = sizeA > sizeB ? posB : posA;
        int smallerSize = Math.Min(sizeA, sizeB);

        bool[] occupiedLanes = new bool[60];
        
        for (int i = largerPos; i < largerPos + largerSize; i++)
        {
            occupiedLanes[i % 60] = true;
        }

        for (int i = smallerPos; i < smallerPos + smallerSize; i++)
        {
            if (!occupiedLanes[i % 60]) return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Returns <c>true</c> if two <c>IPositionable</c>s have identical shapes.
    /// </summary>
    /// <param name="a">The first positionable to compare.</param>
    /// <param name="b">The second positionable to compare.</param>
    public static bool IsIdenticalOverlap(IPositionable a, IPositionable b)
    {
        return IsIdenticalOverlap(a.Position, a.Size, b.Position, b.Size);
    }

    /// <summary>
    /// Returns <c>true</c> if two defined shapes are identical.
    /// </summary>
    /// <param name="posA">The first position to compare.</param>
    /// <param name="sizeA">The first size to compare.</param>
    /// <param name="posB">The second position to compare.</param>
    /// <param name="sizeB">The second size to compare.</param>
    public static bool IsIdenticalOverlap(int posA, int sizeA, int posB, int sizeB)
    {
        return posA == posB && sizeA == sizeB;
    }

    /// <summary>
    /// Returns the
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="lane"></param>
    /// <returns></returns>
    public static OverlapResult HitTestResult(int pos, int size, int lane)
    {
        if (lane == pos) return OverlapResult.LeftEdge;
        if (lane == (pos + size - 1) % 60) return OverlapResult.RightEdge;
        if (IsAnyOverlap(pos, size, lane, 1)) return OverlapResult.Body;
        
        return OverlapResult.None;
    }
    
    public static int LimitPosition(int value)
    {
        int r = value % 60;
        return r < 0 ? r + 60 : r;
    }

    public static int LimitSize(int value)
    {
        return Math.Clamp(value, 1, 60);
    }
}