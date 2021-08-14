using Godot;
using System;

public class CameraController : Godot.Camera
{
    public Player Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var RoomSize = 18;
        if (Player != null) {
            var TargetTranslation = new Vector3(Player.Translation.x, Translation.y, Player.Translation.z);
            TargetTranslation.x = Mathf.Floor((TargetTranslation.x + (RoomSize / 2)) / RoomSize) * RoomSize;
            TargetTranslation.z = Mathf.Floor((TargetTranslation.z + (RoomSize / 2)) / RoomSize) * RoomSize;
            if (1/delta >= 30) {
                var distance = Translation.DistanceTo(TargetTranslation);
                Translation = Translation.LinearInterpolate(TargetTranslation, Mathf.Min(20 * delta, distance));
            } else {
                Translation = TargetTranslation;
            }
        }
    }
}
