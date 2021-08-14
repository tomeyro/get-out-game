using Godot;
using System;

public class Main : Spatial
{
    int MazeSize = 4;
    MazeGenerator mg;

    PackedScene[] RoomScenes;
    PackedScene StartRoomScene;
    PackedScene EndRoomScene;
    PackedScene KeyRoomScene;
    PackedScene ComputerRoomScene;
    GenericRoom[,] InstancedRooms;
    Spatial MazeContainer;

    CameraController Camera;
    public Player Player;

    SaveManager SaveManager;

    RandomNumberGenerator Random;

    AnimationPlayer AnimationPlayer;

    Control UI;

    public int CardKeys = 0;
    Label CardKeyLabel;
    public int UsbDrives = 0;
    Label UsbDrivesLabel;

    bool ComputerHacked = false;
    Label ComputerHackedLabel;

    Label GetOutLabel;
    bool GotOut = false;

    float Timer = 0;
    Label TimerLabel;

    Control MenuScreen;
    Control MapScreen;
    Control Instructions;
    Control ExitedScreen;
    Control CaughtScreen;

    Control Map;
    ColorRect MapRoom;
    ColorRect[,] MapRooms;

    Label CurrentRoomLabel;

    public override void _Ready()
    {
        SaveManager = GetTree().Root.GetNode<SaveManager>("SaveManager");
        MazeSize = SaveManager.GetValueInt("MazeSize", 5);
        mg = new MazeGenerator();
        RoomScenes = new PackedScene[] {
            ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/Room1.tscn"),
            ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/Room2.tscn"),
            ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/Room3.tscn"),
            ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/Room4.tscn"),
            ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/Room5.tscn"),
        };
        StartRoomScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/StartRoom.tscn");
        EndRoomScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/EndRoom.tscn");
        KeyRoomScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/KeyRoom.tscn");
        ComputerRoomScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rooms/ComputerRoom.tscn");
        Player = GetNode<Player>("Player");
        Player.Active = false;
        Camera = GetNode<CameraController>("Camera");
        Camera.Player = Player;
        Random = new RandomNumberGenerator();
        Map = GetNode<Control>("Map");
        MapRoom = Map.GetNode<ColorRect>("Room");
        MapRoom.Visible = false;
        MapRooms = new ColorRect[MazeSize, MazeSize];
        GenerateMaze();
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        UI = GetNode<Control>("UI");
        CardKeyLabel = UI.GetNode<Label>("CardKeyCounter");
        UsbDrivesLabel = UI.GetNode<Label>("UsbDrivesCounter");
        ComputerHackedLabel = UI.GetNode<Label>("ComputerHackedCheck");
        GetOutLabel = UI.GetNode<Label>("GetOutCheck");
        TimerLabel = UI.GetNode<Label>("Timer");
        Instructions = GetNode<Control>("Instructions");
        Instructions.Visible = true;
        ExitedScreen = GetNode<Control>("ExitedScreen");
        ExitedScreen.Visible = false;
        CaughtScreen = GetNode<Control>("CaughtScreen");
        CaughtScreen.Visible = false;
        MenuScreen = GetNode<Control>("Menu");
        MenuScreen.Visible = false;
        MapScreen = GetNode<Control>("Map");
        MapScreen.Visible = false;
        CurrentRoomLabel = UI.GetNode<Label>("RoomLabel");
        CurrentRoomLabel.Text = "Room: 0-0";
    }

    void GenerateMaze()
    {
        Random.Randomize();
        Player.Translation = Vector3.Zero;
        if (MazeContainer != null) {
            MazeContainer.QueueFree();
        }
        MazeContainer = new Spatial();
        InstancedRooms = new GenericRoom[MazeSize, MazeSize];
        mg.Generate(MazeSize);

        var UsbDriveRooms = "";
        var corners = $"/0-0/0-{MazeSize-1}/{MazeSize-1}-{MazeSize-1}/{MazeSize-1}-0/";
        if (MazeSize > 2) {
            var foundRooms = 0;
            while (true) {
                var row = Random.RandiRange(0, MazeSize - 1);
                var col = Random.RandiRange(0, MazeSize - 1);
                var roomkey = $"/{row}-{col}/";
                if (!corners.Contains(roomkey) && !UsbDriveRooms.Contains(roomkey)) {
                    UsbDriveRooms += roomkey;
                    foundRooms += 1;
                    if (foundRooms == 3) {
                        break;
                    }
                }
            }
        } else {
            UsbDriveRooms = $"{corners}";
        }

        float mapRoomSize = 30f;
        float startingMapPosition = 400f - (mapRoomSize * ((float)MazeSize / 2f));
        float maxMapPosition = startingMapPosition + (mapRoomSize * MazeSize);
        var usbDrivesLabel = Map.GetNode<Label>("UsbDrivesLabel");
        usbDrivesLabel.RectPosition = new Vector2(0, startingMapPosition - (usbDrivesLabel.RectSize.y * 2));
        var PlayerRoomLabel = Map.GetNode<Label>("PlayerRoom");
        PlayerRoomLabel.RectPosition = new Vector2(
            startingMapPosition - (MazeSize < 12 ? PlayerRoomLabel.RectSize.x : 0),
            startingMapPosition - PlayerRoomLabel.RectSize.y);
        var ComputerRoomLabel = Map.GetNode<Label>("ComputerRoom");
        ComputerRoomLabel.RectPosition = new Vector2(
            maxMapPosition - (MazeSize < 12 ? 0 : ComputerRoomLabel.RectSize.x),
            startingMapPosition - ComputerRoomLabel.RectSize.y);
        var CardKeyRoomLabel = Map.GetNode<Label>("CardKeyRoom");
        CardKeyRoomLabel.RectPosition = new Vector2(
            startingMapPosition - (MazeSize < 12 ? CardKeyRoomLabel.RectSize.x : 0),
            maxMapPosition);
        var ExitRoomLabel = Map.GetNode<Label>("ExitRoom");
        ExitRoomLabel.RectPosition = new Vector2(
            maxMapPosition - (MazeSize < 12 ? 0 : ExitRoomLabel.RectSize.x),
            maxMapPosition);

        for (int row = 0; row < MazeSize; row ++) {
            for (int col = 0; col < MazeSize; col ++) {
                PackedScene roomScene = RoomScenes[Random.RandiRange(0, RoomScenes.Length - 1)];
                bool hasUsbDrive = UsbDriveRooms.Contains($"/{row}-{col}/");
                if (row == 0 && col == 0) {
                    roomScene = StartRoomScene;
                }
                else if (row == MazeSize - 1 && col == MazeSize - 1) {
                    roomScene = EndRoomScene;
                }
                else if (row == 0 && col == MazeSize - 1) {
                    roomScene = ComputerRoomScene;
                }
                else if (row == MazeSize - 1 && col == 0) {
                    roomScene = KeyRoomScene;
                }

                var room = roomScene.Instance<GenericRoom>();
                room.Init(row, col, mg, hasUsbDrive);
                room.Connect("PlayerEntered", this, "OnPlayerEnteredRoom");
                room.Connect("PlayerExited", this, "OnPlayerExitedRoom");
                room.Connect("PlayerHit", this, "OnPlayerHit");
                room.Connect("UsbDrivePickedUp", this, "OnUsbDrivePickedUp");
                room.Connect("CardKeyPickedUp", this, "OnCardKeyPickedUp");
                room.Connect("ComputerHacked", this, "OnComputerHacked");
                room.Connect("ExitedMaze", this, "OnExitedMaze");
                InstancedRooms[row, col] = room;
                if (row == 0 && col == 0) {
                    MazeContainer.AddChild(room);
                }

                var mapRoom = (ColorRect)MapRoom.Duplicate();
                mapRoom.Visible = hasUsbDrive || corners.Contains($"/{row}-{col}/");
                mapRoom.RectPosition = new Vector2(
                    startingMapPosition + (mapRoomSize * col), startingMapPosition + (mapRoomSize * row));
                mapRoom.GetNode<ColorRect>("Up").Visible = room.DoorUp;
                mapRoom.GetNode<ColorRect>("Down").Visible = room.DoorDown;
                mapRoom.GetNode<ColorRect>("Left").Visible = room.DoorLeft;
                mapRoom.GetNode<ColorRect>("Right").Visible = room.DoorRight;
                mapRoom.GetNode<ColorRect>("Player").Visible = false;
                mapRoom.GetNode<ColorRect>("UsbDrive").Visible = hasUsbDrive;
                Map.AddChild(mapRoom);
                MapRooms[row, col] = mapRoom;
            }
        }
        CallDeferred("add_child", MazeContainer);
    }

    void OnPlayerEnteredRoom(GenericRoom room, Player player)
    {
        foreach (GenericRoom addedRoom in MazeContainer.GetChildren()) {
            if (Mathf.Abs(addedRoom.Row - room.Row) + Mathf.Abs(addedRoom.Col - room.Col) > 1) {
                MazeContainer.RemoveChild(addedRoom);
            }
        }
        if (room.Row > 0) {
            var otherRoom = InstancedRooms[room.Row - 1, room.Col];
            if (otherRoom.GetParent() == null) {
                MazeContainer.AddChild(otherRoom);
            }
        }
        if (room.Row < MazeSize - 1) {
            var otherRoom = InstancedRooms[room.Row + 1, room.Col];
            if (otherRoom.GetParent() == null) {
                MazeContainer.AddChild(otherRoom);
            }
        }
        if (room.Col > 0) {
            var otherRoom = InstancedRooms[room.Row, room.Col - 1];
            if (otherRoom.GetParent() == null) {
                MazeContainer.AddChild(otherRoom);
            }
        }
        if (room.Col < MazeSize - 1) {
            var otherRoom = InstancedRooms[room.Row, room.Col + 1];
            if (otherRoom.GetParent() == null) {
                MazeContainer.AddChild(otherRoom);
            }
        }
        MapRooms[room.Row, room.Col].Visible = true;
        MapRooms[room.Row, room.Col].GetNode<ColorRect>("Player").Visible = true;
        CurrentRoomLabel.Text = $"Room: {room.Row}-{room.Col}";
    }

    void OnPlayerExitedRoom(GenericRoom room)
    {
        MapRooms[room.Row, room.Col].GetNode<ColorRect>("Player").Visible = false;
    }

    void OnPlayerHit()
    {
        Player.Active = false;
        AnimationPlayer.Play("Alarm");
        _UpdateGameOverScreen(CaughtScreen);
    }

    void OnUsbDrivePickedUp()
    {
        UsbDrives += 1;
        UsbDrivesLabel.Text = $"{UsbDrives}";
    }

    void OnCardKeyPickedUp()
    {
        CardKeys += 1;
        CardKeyLabel.Text = $"{CardKeys}";
    }

    void OnComputerHacked()
    {
        ComputerHackedLabel.Visible = true;
        ComputerHacked = true;
    }

    void OnExitedMaze()
    {
        GetOutLabel.Visible = true;
        GotOut = true;
        Player.Active = false;
        _UpdateGameOverScreen(ExitedScreen);
    }

    string _GetStringTimer()
    {
        return $"{Timer.ToString("0.00")}";
    }

    void _UpdateGameOverScreen(Control screen)
    {
        UI.Visible = false;
        screen.Visible = true;
        screen.GetNode<Label>("Timer").Text = _GetStringTimer();
        screen.GetNode<Label>("MazeSize").Text = $"{MazeSize}";
        var objectives = screen.GetNode<Label>("Objectives");
        objectives.GetNode<Label>("HackedPC").Visible = ComputerHacked;
        objectives.GetNode<Label>("GotOut").Visible = GotOut;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("toggle_menu") && (Player.Active || MenuScreen.Visible)) {
            MenuScreen.Visible = !MenuScreen.Visible;
            Player.Active = !MenuScreen.Visible;
        }
        if (Input.IsActionJustPressed("toggle_map") && (Player.Active || MapScreen.Visible)) {
            MapScreen.Visible = !MapScreen.Visible;
            Player.Active = !MapScreen.Visible;
        }
        if (Player.Active) {
            Timer += (float)Math.Round((double)delta, 2);
            UpdateTimerLabel();
        }
    }

    void UpdateTimerLabel()
    {
        TimerLabel.Text = _GetStringTimer();
    }

    void _on_CloseLetterButton_pressed()
    {
        Instructions.Visible = false;
        Player.Active = true;
    }

    void _on_TitleScreenButton_pressed()
    {
        Player.Active = false;
        GetTree().ChangeScene("res://Scenes/Title.tscn");
    }

    void _on_EndingButton_pressed()
    {
        Player.Active = false;
        SaveManager.SetTemp<bool>("EndingHacked", ComputerHacked);
        GetTree().ChangeScene("res://Scenes/Ending.tscn");
    }
}
