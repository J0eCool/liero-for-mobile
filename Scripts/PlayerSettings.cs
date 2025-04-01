using Godot;
using System;

// Stores information about the player's preferenes
public partial class PlayerSettings : Node
{
    public static PlayerSettings Instance { get; private set; } // Singleton pattern; probably incorrect

    public Color WormColor { get; set; } // Color of worm to use
    public bool IsServerPlayer { get; set; } // Hacky flag for differentiating client vs server instance 

    public override void _Ready()
    {
        Instance = this;
    }
}