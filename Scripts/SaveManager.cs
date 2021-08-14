using Godot;
using System;

public class SaveManager : Node
{

    Godot.Collections.Dictionary data;

    const string SaveFileName = "user://getout.save";

    Godot.Collections.Dictionary temp;

    public override void _Ready()
    {
        Load();
        temp = new Godot.Collections.Dictionary();
    }

    public void Load()
    {
        var file = new File();
        if (!file.FileExists(SaveFileName)) {
            data = (Godot.Collections.Dictionary)JSON.Parse("{}").Result;
            return;
        }
        file.Open(SaveFileName, File.ModeFlags.Read);
        var contents = file.GetAsText();
        data = (Godot.Collections.Dictionary)JSON.Parse(contents).Result;
        file.Close();
    }

    public void Save()
    {
        var file = new File();
        file.Open(SaveFileName, File.ModeFlags.Write);
        file.StoreLine(JSON.Print(data));
        file.Close();
    }

    public T GetValue<T>(string key, T defaultValue, Func<object, T> converter=null)
    {
        if (!data.Contains(key)) {
            return defaultValue;
        }
        if (converter != null) {
            return converter(data[key]);
        }
        return (T)data[key];
    }

    public int GetValueInt(string key, int defaultValue)
    {
        var value = GetValue<int>(key, defaultValue, Convert.ToInt32);
        return Mathf.RoundToInt(value);
    }

    public void SetValue<T>(string key, T value)
    {
        data[key] = value;
        Save();
    }

    public T GetTemp<T>(string key, T defaultValue, Func<object, T> converter=null)
    {
        if (!temp.Contains(key)) {
            return defaultValue;
        }
        if (converter != null) {
            return converter(temp[key]);
        }
        return (T)temp[key];
    }

    public int GetTempInt(string key, int defaultValue)
    {
        var value = GetTemp<int>(key, defaultValue, Convert.ToInt32);
        return Mathf.RoundToInt(value);
    }

    public void SetTemp<T>(string key, T value)
    {
        temp[key] = value;
    }
}
