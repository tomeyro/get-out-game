using Godot;
using System;

public class GenericRoom : Spatial
{
    [Signal]
    public delegate void PlayerEntered(GenericRoom room, Player player);
    [Signal]
    public delegate void PlayerExited(GenericRoom room);
    [Signal]
    public delegate void PlayerHit();

    [Signal]
    public delegate void CardKeyPickedUp();
    [Signal]
    public delegate void UsbDrivePickedUp();
    [Signal]
    public delegate void ComputerHacked();
    [Signal]
    public delegate void ExitedMaze();

    public BaseRoom BaseRoom;

    public int Row;
    public int Col;

    PackedScene DoorScene;

    Spatial DoorPositionUp;
    Spatial DoorPositionDown;
    Spatial DoorPositionLeft;
    Spatial DoorPositionRight;

    Player Player;

    public bool DoorUp = false;
    public bool DoorDown = false;
    public bool DoorLeft = false;
    public bool DoorRight = false;

    public void Init(int row, int col, MazeGenerator mazeGenerator, bool hasUsbDrive)
    {
        BaseRoom = GetNode<BaseRoom>("BaseRoom");
        BaseRoom.Connect("PlayerEntered", this, "OnPlayerEntersRoom");
        BaseRoom.Connect("PlayerExited", this, "OnPlayerExitsRoom");

        DoorScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Objects/Door.tscn");

        DoorPositionUp = BaseRoom.GetNode<Spatial>("DoorPositions/Up");
        DoorPositionDown = BaseRoom.GetNode<Spatial>("DoorPositions/Down");
        DoorPositionLeft = BaseRoom.GetNode<Spatial>("DoorPositions/Left");
        DoorPositionRight = BaseRoom.GetNode<Spatial>("DoorPositions/Right");

        Row = row; Col = col;
        Translation = new Vector3(18 * col, 0, 18 * row);
        var connections = mazeGenerator.GetConnections(row, col);
        if (connections.up) {
            var door = DoorScene.Instance<Door>();
            AddChild(door);
            door.Init(Door.DoorTarget.up);
            door.Translation = DoorPositionUp.Translation;
            DoorUp = true;
        }
        if (connections.down) {
            var door = DoorScene.Instance<Door>();
            AddChild(door);
            door.Init(Door.DoorTarget.down);
            door.Translation = DoorPositionDown.Translation;
            DoorDown = true;
        }
        if (connections.left) {
            var door = DoorScene.Instance<Door>();
            AddChild(door);
            door.Init(Door.DoorTarget.left);
            door.Translation = DoorPositionLeft.Translation;
            door.Rotate(Vector3.Up, Mathf.Deg2Rad(90));
            DoorLeft = true;
        }
        if (connections.right) {
            var door = DoorScene.Instance<Door>();
            AddChild(door);
            door.Init(Door.DoorTarget.right);
            door.Translation = DoorPositionRight.Translation;
            door.Rotate(Vector3.Up, Mathf.Deg2Rad(90));
            DoorRight = true;
        }

        Spatial UsbDrive = null;

        if (HasNode("Rotate")) {
            var rotate = GetNode<Spatial>("Rotate");
            var random = new RandomNumberGenerator();
            random.Randomize();
            rotate.Rotate(Vector3.Up, Mathf.Deg2Rad(90 * random.RandiRange(0, 3)));
            if (rotate.HasNode("UsbDrive")) {
                UsbDrive = rotate.GetNode<Spatial>("UsbDrive");
            }
        }

        if (UsbDrive == null && HasNode("UsbDrive")) {
            UsbDrive = GetNode<Spatial>("UsbDrive");
        }
        if (UsbDrive != null) {
            UsbDrive.Visible = hasUsbDrive;
            if (!hasUsbDrive) {
                UsbDrive.QueueFree();
            }
        }
    }

    public override void _Ready()
    {
        Player = GetParent<Spatial>().GetParent<Main>().Player;
    }

    void OnPlayerEntersRoom(Player player)
    {
        EmitSignal(nameof(PlayerEntered), this, player);
    }

    void OnPlayerExitsRoom()
    {
        EmitSignal(nameof(PlayerExited), this);
    }

    void OnPlayerHit()
    {
        EmitSignal(nameof(PlayerHit));
    }

    void OnUsbDrivePickedUp()
    {
        EmitSignal(nameof(UsbDrivePickedUp));
    }

    void OnCardKeyPickedUp()
    {
        EmitSignal(nameof(CardKeyPickedUp));
    }

    void OnComputerHacked()
    {
        EmitSignal(nameof(ComputerHacked));
    }

    void OnExitedMaze()
    {
        EmitSignal(nameof(ExitedMaze));
    }
}
