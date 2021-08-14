using Godot;
using System;

public class Player : KinematicBody
{
    [Export]
    bool titleScreenMode = false;

    AnimationPlayer animationPlayer;

    Spatial Rotator;

    public bool Active = true;

    AudioStreamPlayer Footsteps;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        Rotator = new Spatial();
        Rotator.Translation = Translation;
        GetParent<Spatial>().CallDeferred("add_child", Rotator);

        Footsteps = GetNode<AudioStreamPlayer>("Footsteps");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (titleScreenMode) {
            GlobalTranslate(new Vector3(2f, 0, 0) * delta);
            if (Mathf.Abs(5 - Translation.x) <= 0.2f) {
                Translation = new Vector3(-5, Translation.y, Translation.z);
            }
            Footsteps.VolumeDb = -100;
            animationPlayer.Play("Walking");
        } else {
            if (Active) {
                _HandleMovement(delta);
            } else {
                animationPlayer.Play("Lost");
            }
        }
    }

    public void _HandleMovement(float delta)
    {
        Rotator.Translation = Translation;

        var direction = Vector3.Zero;
        if (Input.IsActionPressed("move_up")) {
            direction.z -= 1;
        }
        if (Input.IsActionPressed("move_down")) {
            direction.z += 1;
        }
        if (Input.IsActionPressed("move_right")) {
            direction.x += 1;
        }
        if (Input.IsActionPressed("move_left")) {
            direction.x -= 1;
        }

        if (direction != Vector3.Zero) {
            Rotator.LookAt(GlobalTransform.origin + direction, Vector3.Up);
            animationPlayer.Play("Walking");
        } else {
            animationPlayer.Play("Idle");
        }

        MoveAndSlide(direction * 300 * delta);

        var RotationY = Mathf.LerpAngle(Rotation.y, Rotator.Rotation.y, 10 * delta);
        var newRotation = new Vector3(Rotation.x, RotationY, Rotation.z);
        Rotation = newRotation;
    }
}
