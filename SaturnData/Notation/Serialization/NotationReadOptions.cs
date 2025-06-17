namespace SaturnData.Notation.Serialization;

public struct NotationReadOptions
{
    public NotationReadOptions()
    {
        SortCollections = true;
        OptimizeHoldNotes = true;
        InferClearThresholdFromDifficulty = true;
    }
    
    /// <summary>
    /// Determines if all collections should be sorted by timestamp after reading a file.
    /// </summary>
    public bool SortCollections { get; set; }
    
    /// <summary>
    /// Determines if no-render segments are removed when importing.
    /// </summary>
    public bool OptimizeHoldNotes { get; set; }
    
    /// <summary>
    /// If the clear threshold should be inferred from the chosen Difficulty instead of the @CLEAR value.
    /// </summary>
    /// <remarks>
    /// This exists because the default value was falsely set to <c>0.83</c> for previous SAT format versions, no matter the difficulty.<br/>
    /// These errors should be fixed automatically when importing SatV1/SatV2 files.
    /// </remarks>
    public bool InferClearThresholdFromDifficulty { get; set; }
}