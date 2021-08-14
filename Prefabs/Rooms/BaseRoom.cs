using Godot;
using System;

public class BaseRoom : Spatial
{
    public Player Player;
    Spatial Lamps;

    [Signal]
    public delegate void PlayerEntered(Player player);

    [Signal]
    public delegate void PlayerExited();

    [Signal]
    public delegate void PlayerHit();

    public override void _Ready()
    {
        Lamps = GetNode<Spatial>("Objects").GetNode<Spatial>("Lamps");
    }

    void ToggleLamps(bool on)
    {
        foreach (Spatial lamp in Lamps.GetChildren()) {
            var omni = lamp.GetNode<OmniLight>("OmniLight");
            omni.ShadowEnabled = on;
            omni.Visible = on;
        }
    }

    void _on_PlayerDetector_body_entered(Player player)
    {
        EmitSignal(nameof(PlayerEntered), player);
        Player = player;
    }

    void _on_PlayerDetector_body_exited(Player player)
    {
        EmitSignal(nameof(PlayerExited));
        Player = null;
    }

}
