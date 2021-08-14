using Godot;
using System;

public class UsbDrive : BaseObject
{
    public override void _Ready()
    {
        base._Ready();
        Connect(nameof(PickedUp), Room, "OnUsbDrivePickedUp");
    }

    protected override void Interact()
    {
        PickUp();
    }
}
