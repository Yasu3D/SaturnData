using System.Collections.Generic;

namespace SaturnData.Notation.Core;

public class Layer(string name)
{
    public string Name { get; set; } = name;
    public List<Event> Events { get; set; } = [];
    public List<Note> Notes { get; set; } = [];
    public List<Note> GeneratedNotes { get; set; } = [];
}