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
            // This could be fewer steps if we add a preload script to hold context.
            // For now, we pass the player color in manually.
            var color = GetNode<ColorPickerButton>("PlayerColor").Color;
            var game = _gameScene.Instantiate();
            var worm = game.FindChild("Worm") as Worm;
            worm.PlayerColor = color;
            var tree = GetTree();
            var root = tree.GetRoot();
            root.AddChild(game);
            root.RemoveChild(tree.GetCurrentScene());
        };
    }
}