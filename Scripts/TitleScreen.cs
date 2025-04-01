using Godot;
using System;
using System.Net;

public partial class TitleScreen : Control
{
    [Export] private Button _localPlayButton;
    [Export] private Button _serverPlayButton;
    [Export] private Button _clientPlayButton;

    [Export] private PackedScene _gameScene;
    [Export] private int _serverPort = 31337;

    public override void _Ready()
    {
        // For local play, just start the game.
        // Distinct from client play because local co-op
        _localPlayButton.Pressed += () =>
        {
            // Local play is a "server of one" (for now, this is hacky)
            PlayerSettings.Instance.IsServerPlayer = true;
            GameStart();
        };

        // For server play, we create a server, and tell 
        _serverPlayButton.Pressed += () =>
        {
            PlayerSettings.Instance.IsServerPlayer = true;

            var peer = new ENetMultiplayerPeer();
            peer.CreateServer(_serverPort);
            GetTree().Root.Multiplayer.MultiplayerPeer = peer;

            GameStart();
        };
        _clientPlayButton.Pressed += () =>
        {
            PlayerSettings.Instance.IsServerPlayer = false;

            // default to joining a localhost server for testing purposes
            var peer = new ENetMultiplayerPeer();
            peer.CreateClient("192.168.1.1", _serverPort);
            GetTree().Root.Multiplayer.MultiplayerPeer = peer;

            GameStart();
        };
    }

    // Unloads current scene and loads game scene
    void GameStart()
    {
        // Pass worm color to game via PlayerSettings
        var color = GetNode<ColorPickerButton>("PlayerColor").Color;
        PlayerSettings.Instance.WormColor = color;

        // Replace title screen with game screen
        GetTree().ChangeSceneToPacked(_gameScene);
    }
}