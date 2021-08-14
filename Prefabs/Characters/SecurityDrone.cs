using Godot;
using System;

public class SecurityDrone : Spatial
{
    [Signal]
    public delegate void PlayerHit();

    bool Active = false;

    Spatial PlayerDetector;
    SpotLight SpotLight;

    AnimationPlayer animationPlayer;

    [Export]
    Vector3[] movements;

    int currentTargetIndex = 0;

    [Export]
    float speed = 3;

    Spatial movementTarget;

    [Export]
    bool titleScreenMode = false;

    GenericRoom Room;

    Spatial Rotator;

    AudioStreamPlayer3D Sfx;

    float CollidingCounter = 0;
    float MaxCollidingCounter = 0.2f;

    Player Player;
    Area AreaPlayerDetector;
    RayCast RayPlayerDetector;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerDetector = GetNode<Spatial>("Body/PlayerDetector");
        SpotLight = GetNode<SpotLight>("Body/SpotLight");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        if (movements == null || movements.Length == 0) {
            movements = new Vector3[] { Vector3.Zero };
        }
        movementTarget = new Spatial();
        movementTarget.Translation = Translation + movements[0];
        GetParent<Spatial>().CallDeferred("add_child", movementTarget);
        Room = GetParentRoom();
        if (Room != null) {
            Room.BaseRoom.Connect("PlayerEntered", this, "OnPlayerEntersRoom");
            Room.BaseRoom.Connect("PlayerExited", this, "OnPlayerExitsRoom");
            Connect(nameof(PlayerHit), Room, "OnPlayerHit");
        }
        if (!titleScreenMode) {
            Deactivate();
        }
        Rotator = new Spatial();
        Rotator.Translation = Translation;
        GetParent<Spatial>().CallDeferred("add_child", Rotator);
        Sfx = GetNode<AudioStreamPlayer3D>("Sfx");

        AreaPlayerDetector = GetNode<Area>("Body/AreaPlayerDetector");
        RayPlayerDetector = AreaPlayerDetector.GetNode<RayCast>("RayCast");
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

    void OnPlayerEntersRoom(Player player)
    {
        Activate();
    }

    void Activate()
    {
        Active = true;
        animationPlayer.Play("Drone");
        SpotLight.ShadowEnabled = true;
        SpotLight.Visible = true;
        foreach (RayCast ray in PlayerDetector.GetChildren()) {
            ray.Enabled = true;
        }
    }

    void OnPlayerExitsRoom()
    {
        Deactivate();
    }

    void Deactivate()
    {
        Active = false;
        animationPlayer.Stop();
        SpotLight.ShadowEnabled = false;
        SpotLight.Visible = false;
        foreach (RayCast ray in PlayerDetector.GetChildren()) {
            ray.Enabled = false;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        Rotator.Translation = Translation;
        if (titleScreenMode) {
            GlobalTranslate(new Vector3(2f, 0, 0) * delta);
            if (Mathf.Abs(5 - Translation.x) <= 0.2f) {
                Translation = new Vector3(-5, Translation.y, Translation.z);
            }
            SpotLight.ShadowEnabled = false;
            if (!Sfx.Playing) {
                Sfx.Play();
            }
        } else {
            if (Active) {
                if (!Sfx.Playing) {
                    Sfx.Play();
                }
                SpotLight.ShadowEnabled = true;
                var colliding = false;
                if (Player != null) {
                    var RayTarget = new Vector3(
                        Player.GlobalTransform.origin.x,
                        RayPlayerDetector.GlobalTransform.origin.y,
                        Player.GlobalTransform.origin.z);
                    RayPlayerDetector.LookAt(RayTarget, Vector3.Up);
                    RayPlayerDetector.ForceRaycastUpdate();
                    colliding = RayPlayerDetector.IsColliding() && RayPlayerDetector.GetCollider() == Player;
                }
                if (colliding) {
                    CollidingCounter += delta;
                } else {
                    CollidingCounter = 0;
                }
                if (CollidingCounter >= MaxCollidingCounter) {
                    animationPlayer.Stop();
                    var rotatorTarget = new Vector3(
                        Player.GlobalTransform.origin.x,
                        Rotator.GlobalTransform.origin.y,
                        Player.GlobalTransform.origin.z);
                    Rotator.LookAt(rotatorTarget, Vector3.Up);
                    EmitSignal(nameof(PlayerHit));
                } else {
                    animationPlayer.Play("Drone");
                    var direction = (movementTarget.GlobalTransform.origin - GlobalTransform.origin).Normalized();
                    direction.y = 0;
                    if (!colliding) {
                        Rotator.LookAt(movementTarget.GlobalTransform.origin, Vector3.Up);
                    }
                    GlobalTranslate(direction * speed * delta);
                    if (GlobalTransform.origin.DistanceTo(movementTarget.GlobalTransform.origin) <= 0.2f) {
                        currentTargetIndex = currentTargetIndex + 1 >= movements.Length ? 0 : currentTargetIndex + 1;
                        movementTarget.Translation += movements[currentTargetIndex];
                    }
                }
            }
            var RotationY = Mathf.LerpAngle(Rotation.y, Rotator.Rotation.y, 10 * delta);
            var newRotation = new Vector3(Rotation.x, RotationY, Rotation.z);
            Rotation = newRotation;
        }
    }

    void _on_AreaPlayerDetector_body_entered(Player player)
    {
        Player = player;
    }

    void _on_AreaPlayerDetector_body_exited(Player player)
    {
        Player = null;
    }
}
