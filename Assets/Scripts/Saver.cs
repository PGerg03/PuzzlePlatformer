using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#region Option POCOs
[System.Serializable]
struct OptionSave
{
    public float Volume;
    public int Resolution, WindowMode, StoryMode;
    public int Single, Multi;
    public int SingleStoryC, MultiStoryC;
    public List<StarCollection> SinglePStars;
    public List<StarCollection> MultiPStars;
};

[System.Serializable]
public class StarCollection
{
    public List<int> stars = new() { 0, 0, 0 };

    public int this[int x]
    {
        get { return stars[x]; }
        set { stars[x] = value; }
    }
    public int Count
    {
        get { return stars.Count; }
    }
    public int Collected
    {
        get { return stars.Sum(); }
    }

    public void Clear()
    {
        stars.Clear();
        stars.AddRange(new List<int>() { 0, 0, 0 });
    }

}

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
        options.WindowMode = (FullScreenMode)soption.WindowMode;
        options.MainVolume = soption.Volume;
        options.Story = soption.StoryMode;
        options.SingleUnlockedMaps = soption.Single;
        options.MultiUnlockedMaps = soption.Multi;
        options.SingleStoryCount = soption.SingleStoryC;
        options.MultiStoryCount = soption.MultiStoryC;

        options.SinglePlayerStars.AddRange(soption.SinglePStars ?? new List<StarCollection>());
        while (options.SinglePlayerStars.Count < 15)
        {
            options.SinglePlayerStars.Add(new StarCollection());
        }
        options.MultiPlayerStars.AddRange(soption.MultiPStars ?? new List<StarCollection>());
        while (options.MultiPlayerStars.Count < 15)
        {
            options.MultiPlayerStars.Add(new StarCollection());
        }
    }

    public static void SaveOptions(string path, Game options)
    {
        OptionSave optionSave = new()
        {
            Resolution = options.Resolution,
            WindowMode = (int)options.WindowMode,
            Volume = options.MainVolume,
            StoryMode = options.Story,
            Single = options.SingleUnlockedMaps,
            Multi = options.MultiUnlockedMaps,
            SingleStoryC = options.SingleStoryCount,
            MultiStoryC = options.MultiStoryCount,
            SinglePStars = new(),
            MultiPStars = new()
        };

        options.SinglePlayerStars.ForEach(s => optionSave.SinglePStars.Add(s));
        options.MultiPlayerStars.ForEach(s => optionSave.MultiPStars.Add(s));

        string json = JsonUtility.ToJson(optionSave, true);
        File.WriteAllText(path, json);
    }
    #endregion
}