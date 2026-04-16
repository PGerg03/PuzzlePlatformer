using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StarController : MonoBehaviour
{
    private int ID = -1;
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
