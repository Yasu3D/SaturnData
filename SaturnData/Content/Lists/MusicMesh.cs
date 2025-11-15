using System;
using System.Collections.Generic;
using SaturnData.Notation.Core;

namespace SaturnData.Content.Lists;

public class MusicMesh
{
    /// <summary>
    /// Invoked whenever the selected node changes.
    /// </summary>
    public event EventHandler? SelectionChanged;
    
    /// <summary>
    ///  A list of all nodes in the mesh.
    /// </summary>
    public List<MusicMeshNode> Nodes = [];

    /// <summary>
    /// The currently selected node.
    /// </summary>
    public MusicMeshNode? SelectedNode
    {
        get => selectedNode;
        private set
        {
            if (selectedNode == value) return;
            
            selectedNode = value;
            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    private MusicMeshNode? selectedNode = null;

    /// <summary>
    /// Moves the selection point to the left in the mesh.
    /// </summary>
    public void Left()
    {
        if (SelectedNode?.Left == null) return;
        SelectedNode = SelectedNode.Left;
    }

    /// <summary>
    /// Moves the selection point to the right in the mesh.
    /// </summary>
    public void Right()
    {
        if (SelectedNode?.Right == null) return;
        SelectedNode = SelectedNode.Right;
    }

    /// <summary>
    /// Moves the selection point up in the mesh.
    /// </summary>
    public void Up()
    {
        if (SelectedNode?.Top == null) return;
        SelectedNode = SelectedNode.Top;
    }

    /// <summary>
    /// Moves the selection point down in the mesh.
    /// </summary>
    public void Down()
    {
        if (SelectedNode?.Bottom == null) return;
        SelectedNode = SelectedNode.Bottom;
    }

    /// <summary>
    /// Selects a specific entry, if it's contained in the mesh. Otherwise, sets the selection to the first registered node.
    /// </summary>
    /// <param name="entry">The <see cref="Entry"/> the node to select contains.</param>
    public void Select(Entry? entry)
    {
        if (Nodes.Count == 0)
        {
            SelectedNode = null;
            return;
        }

        if (entry == null)
        {
            SelectedNode = Nodes[0];
            return;
        }

        bool entryFound = false;
        foreach (MusicMeshNode node in Nodes)
        {
            if (node.Entry != entry) continue;
            
            SelectedNode = node;
            entryFound = true;
            break;
        }

        if (!entryFound)
        {
            SelectedNode = Nodes[0];
        }
    }
}

public class MusicMeshNode
{
    /// <summary>
    /// The entry associated with this node.
    /// </summary>
    public Entry? Entry = null;

    /// <summary>
    /// The node to the left of the current node, representing a neighboring entry.
    /// </summary>
    public MusicMeshNode? Left = null;

    /// <summary>
    /// The node to the right of the current node, representing a neighboring entry.
    /// </summary>
    public MusicMeshNode? Right = null;

    /// <summary>
    /// The node above the current node, representing a higher difficulty.
    /// </summary>
    public MusicMeshNode? Top = null;

    /// <summary>
    /// The node below the current node, representing a lower difficulty.
    /// </summary>
    public MusicMeshNode? Bottom = null;
}