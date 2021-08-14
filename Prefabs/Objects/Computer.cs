using Godot;
using System;

public class Computer : BaseObject
{
    [Signal]
    public delegate void Hacked();

    AudioStreamPlayer SfxError;
    AnimationPlayer AnimationPlayer;

    bool IsHacked = false;

    public override void _Ready()
    {
        base._Ready();
        SfxError = GetNode<AudioStreamPlayer>("Sfx/Error");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Connect(nameof(Hacked), Room, "OnComputerHacked");
    }

    protected override void Interact()
    {
        if (!IsHacked) {
            if (CanHack()) {
                AnimationPlayer.Play("Hack");
                IsHacked = true;
                EmitSignal(nameof(Hacked));
            } else {
                SfxError.Play();
            }
        }
    }

    bool CanHack()
    {
        var main = GetTree().Root.GetNode<Main>("Main");
        return main.UsbDrives == 3;
    }

}
