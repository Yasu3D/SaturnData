using System;
using System.Collections.Generic;

namespace SaturnData.Utilities;

internal static class BinarySearch
{
    internal static T? Run<T>(List<T> collection, Func<T, bool> predicate) where T : class
    {
        int minimum = 0;
        int maximum = collection.Count - 1;

        T? result = null;

        while (minimum <= maximum)
        {
            int center = minimum + (maximum - minimum) / 2;

            if (!predicate.Invoke(collection[center]))
            {
                maximum = center - 1;
            }
            else
            {
                result = collection[center];
                minimum = center + 1;
            }
        }

        return result;
    }

    internal static T? First<T>(List<T> collection, Func<T, bool> predicate) where T : class
    {
        int minimum = 0;
        int maximum = collection.Count - 1;
        int center = minimum + (maximum - minimum) / 2;
        
        T? result = null;

        while (minimum <= maximum)
        {
            center = minimum + (maximum - minimum) / 2;

            if (!predicate.Invoke(collection[center]))
            {
                maximum = center - 1;
            }
            else
            {
                result = collection[center];
                minimum = center + 1;
            }
        }

        if (result == null) return result;

        while (center > 0)
        {
            center--;
            
            T previous = collection[center];
            if (!predicate.Invoke(previous)) break;

            result = previous;
        }
        
        return result;
    }
    
    internal static T? Last<T>(List<T> collection, Func<T, bool> predicate) where T : class
    {
        int minimum = 0;
        int maximum = collection.Count - 1;
        int center = minimum + (maximum - minimum) / 2;
        
        T? result = null;

        while (minimum <= maximum)
        {
            center = minimum + (maximum - minimum) / 2;

            if (!predicate.Invoke(collection[center]))
            {
                maximum = center - 1;
            }
            else
            {
                result = collection[center];
                minimum = center + 1;
            }
        }

        if (result == null) return result;

        while (center < collection.Count - 1)
        {
            center++;
            
            T next = collection[center];
            if (!predicate.Invoke(next)) break;

            result = next;
        }
        
        return result;
    }
}