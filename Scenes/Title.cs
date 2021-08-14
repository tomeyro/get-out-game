using Godot;
using System;

public class Title : Spatial
{
    SaveManager SaveManager;

    int MazeSize;
    Label MazeSizeLabel;
    string MazeSizeLabelText;

    Control Credits;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SaveManager = GetTree().Root.GetNode<SaveManager>("SaveManager");
        var control = GetNode<Control>("Control");
        MazeSize = SaveManager.GetValueInt("MazeSize", 5);
        MazeSizeLabel = control.GetNode<Label>("MazeSizeLabel");
        MazeSizeLabelText = MazeSizeLabel.Text;
        UpdateMazeSizeLabel();
        control.GetNode<HSlider>("MazeSize").Value = MazeSize;
        Credits = GetNode<Control>("Credits");
    }

    void _on_MazeSize_value_changed(float value)
    {
        MazeSize = Mathf.RoundToInt(value);
        SaveManager.SetValue<int>("MazeSize", MazeSize);
        UpdateMazeSizeLabel();
    }

    void UpdateMazeSizeLabel()
    {
        var difficulties = new string[] {
            "", "",
            "Baby Mode",
            "Very Easy",
            "Easy",
            "Normal",
            "Kinda Hard",
            "Hard",
            "Very Hard",
            "Mega Hard",
            "Ultra Hard",
            "Super Duper Hard",
            "Chariz Hard",
            "Impossible",
            "Stop",
            "Go back",
            "This is too much",
            "STOP!",
            "You'll never finish this",
            "Are you crazy?",
            "R.I.P.",
        };
        MazeSizeLabel.Text = $"{MazeSizeLabelText} {MazeSize} ({difficulties[MazeSize]})";
    }

    void _on_Start_pressed()
    {
        GetTree().ChangeScene("res://Scenes/Main.tscn");
    }

    void _on_CreditsBtn_pressed()
    {
        Credits.Visible = true;
    }

    void _on_CloseCreditsBtn_pressed()
    {
        Credits.Visible = false;
    }
}
