using System;

namespace SaturnData.Notation.Interfaces;

public enum LaneSweepDirection
{
    Counterclockwise = 0,
    Clockwise = 1,
    CenterOutward = 2,
    CenterInward = 3,
    Instant = 4,
}

public interface ILaneToggle
{
    /// <summary>
    /// The direction a lane toggle sweep animation takes.
    /// </summary>
    public LaneSweepDirection Direction { get; set; }
    
    /// <summary>
    /// Returns the duration of a lane toggle sweep animation in milliseconds.
    /// </summary>
    public static float SweepDuration(LaneSweepDirection direction, int size)
    {
        return direction switch
        {
            LaneSweepDirection.Counterclockwise => size * 8.3333333f,
            LaneSweepDirection.Clockwise => size * 8.3333333f,
            LaneSweepDirection.CenterOutward => size * 4.1666666f,
            LaneSweepDirection.CenterInward => size * 4.1666666f,
            LaneSweepDirection.Instant => 0,
            _ => 0,
        };
    }

    /// <summary>
    /// Calculates the sweep of a Lane Toggle.
    /// </summary>
    /// <param name="state">The existing state of lanes.</param>
    /// <param name="visualTime">The current visual time.</param>
    /// <param name="isStartupSequence">Is this the startup sequence?</param>
    public void SetState(bool[] state, float visualTime, bool isStartupSequence);

    /// <summary>
    /// Calculates the sweep of a Lane Toggle.
    /// </summary>
    /// <param name="state">The existing state of lanes.</param>
    /// <param name="show">Should the lane be shown or hidden?</param>
    /// <param name="objectTime">The time of the Lane Toggle.</param>
    /// <param name="visualTime">The current visual time.</param>
    /// <param name="isStartupSequence">Is this the startup sequence?</param>
    /// <param name="position">The position of the Lane Toggle.</param>
    /// <param name="size">The size of the Lane Toggle.</param>
    /// <param name="direction">The sweep direction of the Lane Toggle.</param>
    internal static void SetState(bool[] state, bool show, float objectTime, float visualTime, bool isStartupSequence, int position, int size, LaneSweepDirection direction)
    {
        float delta = visualTime - objectTime;
        float duration = objectTime == 0 && !isStartupSequence ? 0 : SweepDuration(direction, size);
        
        // Instant or after the sweep. Set lanes without animating.
        if (duration == 0 || delta > duration)
        {
            for (int j = position; j < position + size; j++)
            {
                state[j % 60] = show;
            }

            return;
        }

        float progress = delta / duration;

        // In range for a sweep animation. Set lanes based on animation.
        if (direction is LaneSweepDirection.CenterOutward)
        {
            float halfSize = size * 0.5f;
            int floor = (int)halfSize;
            int steps = (int)Math.Ceiling(halfSize);
            int centerClockwise = position + floor;
            int centerCounterclockwise = size % 2 != 0 ? centerClockwise : centerClockwise + 1;
            int offset = size % 2 != 0 ? 60 : 59;

            for (int j = 0; j < (int)(steps * progress); j++)
            {
                state[(centerClockwise - j + offset) % 60] = show;
                state[(centerCounterclockwise + j + offset) % 60] = show;
            }
        }
        else if (direction is LaneSweepDirection.CenterInward)
        {
            float halfSize = size * 0.5f;
            int steps = (int)Math.Ceiling(halfSize);

            for (int j = 0; j < (int)(steps * progress); j++)
            {
                state[(position + j + 60) % 60] = show;
                state[(position + size - j + 59) % 60] = show;
            }
        }
        else if (direction is LaneSweepDirection.Clockwise)
        {
            for (int j = 0; j < (int)(size * progress); j++)
            {
                state[(position + size - j + 59) % 60] = show;
            }
        }
        else if (direction is LaneSweepDirection.Counterclockwise)
        {
            for (int j = 0; j < (int)(size * progress); j++)
            {
                state[(j + position + 60) % 60] = show;
            }
        }
    }
}