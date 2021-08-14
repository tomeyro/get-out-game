using Godot;
using System;

public class ExitDoor : BaseObject
{
    [Signal]
    public delegate void ExitedMaze();

    AudioStreamPlayer SfxError;
    AnimationPlayer AnimationPlayer;

    bool Exited = false;

    [Export]
    bool EndingModeDisabled = false;
    [Export]
    bool EndingModeRestart = false;

    public override void _Ready()
    {
        base._Ready();
        if (Room != null) {
            Connect(nameof(ExitedMaze), Room, "OnExitedMaze");
        }
        SfxError = GetNode<AudioStreamPlayer>("Sfx/Error");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    protected override void Interact()
    {
        if (EndingModeDisabled) {
            SfxError.Play();
        }
        else if (EndingModeRestart) {
            AnimationPlayer.Play("Exit");
            EmitSignal(nameof(ExitedMaze));
        }
        else if (!Exited) {
            if (CanExit()) {
                Exited = true;
                AnimationPlayer.Play("Exit");
                EmitSignal(nameof(ExitedMaze));
            } else {
                SfxError.Play();
            }
        }
    }

    bool CanExit()
    {
        var main = GetTree().Root.GetNode<Main>("Main");
        return main.CardKeys == 1;
    }
}
