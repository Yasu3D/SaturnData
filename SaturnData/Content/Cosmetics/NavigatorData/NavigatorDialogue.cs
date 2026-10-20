using System;
using System.Collections.Generic;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary> 
/// A collection of <see cref="NavigatorLine"/>s to randomly choose from.
/// </summary>
public class NavigatorDialogue
{
    /// <summary>
    /// The collection of dialogue variants.
    /// </summary>
    public List<NavigatorLine> Lines { get; set; } = [];

    /// <summary>
    /// A random dialogue variant chosen from <see cref="Lines"/>
    /// </summary>
    public NavigatorLine RandomLine
    {
        get
        {
            if (Lines.Count == 1) return Lines[0];

            if (Lines.Count > 1)
            {
                Random random = new();
                int index = random.Next(0, Lines.Count);

                return Lines[index];
            }
            
            return NavigatorLine.NoMessageFound();
        }
    }
}
