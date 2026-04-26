using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditTests
{
    private GameObject gameObject;
    private Game game;

    [SetUp]
    public void SetUp()
    {
        gameObject = new();
        game = gameObject.AddComponent<Game>();
    }

    [Test]
    public void GameClassEditTests()
    {
        // Awake
        game.Awake();
        Assert.IsFalse(game.inGame);

        // Save options
        if (!File.Exists("Options.json"))
            game.SaveOptions();
        Assert.IsTrue(File.Exists("Options.json"));
    }

    [Test]
    public void StarClassEditTests()
    {
        StarController star = gameObject.AddComponent<StarController>();
        // Default ID
        Assert.AreEqual(star.ID, -1);

        // Setup
        star.Setup(1, Vector3.zero, 0, game);
        Assert.AreEqual(star.ID, 1);
        Assert.IsFalse(star.collected);
    }
}
