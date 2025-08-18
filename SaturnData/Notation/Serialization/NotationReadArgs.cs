namespace SaturnData.Notation.Serialization;

public class NotationReadArgs
{
    /// <summary>
    /// Should all objects in collections be sorted by their timestamp?
    /// </summary>
    public bool SortCollections { get; set; } = true;

    /// <summary>
    /// Should hidden hold segments be removed?
    /// </summary>
    public bool OptimizeHoldNotes { get; set; } = true;

    /// <summary>
    /// Should the clear threshold be inferred from the chosen Difficulty instead of the @CLEAR value?
    /// </summary>
    /// <remarks>
    /// This exists because the default value was falsely set to <c>0.83</c> for previous SAT format versions, no matter the difficulty.<br/>
    /// These errors should be fixed automatically when importing SatV1/SatV2 files.
    /// </remarks>
    public bool InferClearThresholdFromDifficulty { get; set; } = true;
}