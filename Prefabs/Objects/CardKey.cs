using Godot;
using System;

public class CardKey : BaseObject
{
    public override void _Ready()
    {
        base._Ready();
        Connect(nameof(PickedUp), Room, "OnCardKeyPickedUp");
    }

    protected override void Interact()
    {
        PickUp();
    }
}
