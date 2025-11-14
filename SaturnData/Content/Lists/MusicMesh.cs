using SaturnData.Notation.Core;

namespace SaturnData.Content.Lists;

public class MusicMesh
{
    
}

public class MusicMeshNode
{
    /// <summary>
    /// The 
    /// </summary>
    public Entry? Entry = null;

    /// <summary>
    /// 
    /// </summary>
    public MusicMeshNode? Left = null;

    /// <summary>
    /// 
    /// </summary>
    public MusicMeshNode? Right = null;

    /// <summary>
    /// 
    /// </summary>
    public MusicMeshNode? HigherDifficulty = null;

    /// <summary>
    /// 
    /// </summary>
    public MusicMeshNode? LowerDifficulty = null;
}