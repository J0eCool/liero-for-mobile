using Godot;
using System;

public partial class PlayerSettings : Node
{
    public static PlayerSettings Instance { get; private set; }

    public Color WormColor { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }
}