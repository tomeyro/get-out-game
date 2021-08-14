using Godot;
using System;

public class Text3D : Spatial
{

    [Export]
    string Text = "text";

    public override void _Ready()
    {
        GetNode<Viewport>("Viewport").GetNode<Control>("Control").GetNode<Label>("Label").Text = Text;
    }

}
