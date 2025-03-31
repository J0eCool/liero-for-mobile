using Godot;
using System;

public partial class TitleScreen : Control
{
    [Export] PackedScene _gameScene;

    public override void _Ready()
    {
        var localPlayButton = GetNode<Button>("LocalPlay");
        localPlayButton.Pressed += () =>
        {
            // Update PlayerSettings
            var color = GetNode<ColorPickerButton>("PlayerColor").Color;
            PlayerSettings.Instance.WormColor = color;

            // Replace title screen with game screen
            GetTree().ChangeSceneToPacked(_gameScene);
        };
    }
}