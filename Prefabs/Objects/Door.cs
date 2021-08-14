using Godot;
using System;

public class Door : BaseObject
{
    public enum DoorTarget {
        up,
        down,
        left,
        right
    };

    DoorTarget Target;

    AudioStreamPlayer DoorHandle;

    public override void _Ready()
    {
        DoorHandle = GetNode<AudioStreamPlayer>("DoorHandle");
    }

    public void Init(DoorTarget target)
    {
        Target = target;
    }

    protected override void Interact()
    {
        DoorHandle.Play(0.4f);
        var direction = Vector3.Zero;
        if (Target == DoorTarget.up) {
            direction.z -= 1;
        }
        if (Target == DoorTarget.down) {
            direction.z += 1;
        }
        if (Target == DoorTarget.right) {
            direction.x += 1;
        }
        if (Target == DoorTarget.left) {
            direction.x -= 1;
        }
        Player.Translation = Player.Translation + (direction * 3);
    }

}
