using Godot;
using System;

public class CameraFollow : Camera
{
    Player Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetParent().GetNode<Player>("Player");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Translation = new Vector3(Player.Translation.x, Translation.y, Player.Translation.z);
    }
}
