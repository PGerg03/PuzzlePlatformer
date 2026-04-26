using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

[RequireComponent(typeof(SpriteRenderer))]
public class StarController : MonoBehaviour
{
    public int ID = -1;
    public Game game;
    public bool collected = false;

    public void Setup(int id, Vector3 pos, int isCollected, Game g)
    {
        ID = id;
        transform.localPosition = pos;
        collected = isCollected > 0;
        game = g;
        if (collected)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Color originalColor = renderer.color;
            originalColor.a = 0.5f;
            renderer.color = originalColor;
        }
    }

    public void End()
    {
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 7) return;
        if (!collected)
        {
            game.Collect(ID);
        }

        gameObject.SetActive(false);
    }
}
