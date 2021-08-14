using Godot;
using System;

public class EndingScene : Spatial
{
    SaveManager SaveManager;
    AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        SaveManager = GetTree().Root.GetNode<SaveManager>("SaveManager");
        var hacked = SaveManager.GetTemp<bool>("EndingHacked", false);
        GetNode<Spatial>("EndingNotHacked").Visible = !hacked;
        GetNode<Spatial>("EndingHacked").Visible = hacked;

        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        AnimationPlayer.Connect("animation_finished", this, "OnAnimationFinished");

        var exit = GetNode<Spatial>("EndingRoom").GetNode<ExitDoor>("ExitDoorRestart");
        exit.Connect("ExitedMaze", this, "OnExited");
    }

    void OnExited()
    {
        AnimationPlayer.Play("Ending");
    }

    void OnAnimationFinished(string animationName)
    {
        if (animationName == "Ending") {
            GetTree().ChangeScene("res://Scenes/Main.tscn");
        }
    }
}
