Godot provides high-level and packet-level networking APIs. We'll try sticking to the highest-level abstractions possible, until such time as we need to dig deeper.

# Basics

Every Node has a `Multiplayer` field that gives access to the `MultiplayerAPI`.

For now, all we really know is that you can set `GetTree().Root.Multiplayer.MultiplayerPeer` to either client or server, and that will establish a connection (I think? we don't actually do anything with connections atm)