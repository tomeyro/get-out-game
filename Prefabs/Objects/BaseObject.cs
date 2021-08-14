using Godot;
using System;

public class BaseObject : Spatial
{
    [Signal]
    public delegate void PickedUp();

    protected Player Player;

    virtual protected void Interact() {
        // Override me
    }

    protected GenericRoom Room;

    public override void _Ready()
    {
        Room = GetParentRoom();
    }

    GenericRoom GetParentRoom()
    {
        var parent = GetParent();
        while (parent != null) {
            if (typeof(GenericRoom).IsInstanceOfType(parent)) {
                return (GenericRoom) parent;
            }
            parent = parent.GetParent();
        }
        return null;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Player != null && Input.IsActionJustPressed("interact")) {
            Interact();
        }
    }

    void _on_PlayerDetector_body_entered(Player player)
    {
        Player = player;
    }

    void _on_PlayerDetector_body_exited(Player player)
    {
        Player = null;
    }

    protected void PickUp()
    {
        EmitSignal(nameof(PickedUp));
        Visible = false;
        if (HasNode("Sfx")) {
            var sfx = GetNode<AudioStreamPlayer>("Sfx");
            sfx.Play();
            sfx.Connect("finished", this, "queue_free");
        } else {
            QueueFree();
        }
    }
}
