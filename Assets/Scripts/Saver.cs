using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#region Option POCOs
struct OptionSave
{
    public float Volume;
    public int Resolution, WindowMode, StoryMode;
    public int Single, Multi;
    public int SingleStoryC, MultiStoryC;
};
#endregion

public static class Saver
{
    #region Save Options
    public static void ReadOptionsFile(string path, Game options)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("File is empty: " + path);
            return;
        }

        // Deserialize the JSON string into a JObject
        OptionSave soption;
        try
        {
            soption = JsonUtility.FromJson<OptionSave>(json);
        }
        catch
        {
            Debug.LogError("Failed to deserialize JSON: " + path);
            return;
        }

        /* Handle options data */
        options.Resolution = soption.Resolution;
        options.WindowMode = soption.WindowMode;
        options.MainVolume = soption.Volume;
        options.Story = soption.StoryMode;
        options.SingleUnlockedMaps = soption.Single;
        options.MultiUnlockedMaps = soption.Multi;
        options.SingleStoryCount = soption.SingleStoryC;
        options.MultiStoryCount = soption.MultiStoryC;
    }

    public static void SaveOptions(string path, Game options)
    {
        OptionSave optionSave = new()
        {
            Resolution = options.Resolution,
            WindowMode = options.WindowMode,
            Volume = options.MainVolume,
            StoryMode = options.Story,
            Single = options.SingleUnlockedMaps,
            Multi = options.MultiUnlockedMaps,
            SingleStoryC = options.SingleStoryCount,
            MultiStoryC = options.MultiStoryCount
        };

        string json = JsonUtility.ToJson(optionSave);
        File.WriteAllText(path, json);
    }
    #endregion
}