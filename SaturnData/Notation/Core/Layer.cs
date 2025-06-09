using System.Collections.Generic;

namespace SaturnData.Notation.Core;

public class Layer<T>(string name)
{
    public string Name { get; set; } = name;
    public List<T> Items { get; }= [];
}