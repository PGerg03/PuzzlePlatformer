using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Nature,
    Desert,
    Ice,
}

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rg;
    private SpriteRenderer sr;
    [SerializeField] private Sprite RightModel;
    [SerializeField] private Sprite LeftModel;
    [SerializeField] private GameObject Pointer;
    public PlayerType Type;
    public bool Active = false;

    public string PlayerAxes;
    public KeyCode JumpCode;
    public float MovePower;
    public float JumpPower = 35;

    public bool Done = false;
    public bool Jump = false;
    public bool UnderWater => inWater();
    public bool Grounded => isGrounded();

    public int LastContact;

    [Header("BoxCast")]
    public Vector2 Ground_BoxSize;
    public float Ground_CastDistance;
    public LayerMask LayerGround;
    public Vector2 Water_BoxSize;
    public float Water_CastDistance;
    public LayerMask LayerWater;
    public Vector2 Wall_BoxSize;
    public float Wall_CastDistance;

    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Active)
        {
            if (Input.GetKeyDown(JumpCode) && Grounded)
            {
                Jump = true;
            }

            MovePower = Input.GetAxisRaw(PlayerAxes);
            if (MovePower > 0) sr.sprite = RightModel;
            if (MovePower < 0) sr.sprite = LeftModel;
            if (isLeftWall() && MovePower < 0) MovePower = 0;
            if (isRightWall() && MovePower > 0) MovePower = 0;
        }
    }

    void FixedUpdate()
    {
        if (Active)
        {
            if (UnderWater)
            {
                if (Jump) rg.AddForce(new Vector2(0, JumpPower * 4));
                rg.velocity = new Vector2(MovePower * 2, rg.velocity.y);
                rg.gravityScale = 0.5f;
            }
            else
            {
                if (Jump) rg.AddForce(new Vector2(0, JumpPower * 10));
                rg.velocity = new Vector2(MovePower * 5, rg.velocity.y);
                rg.gravityScale = 2f;
            }
            Jump = false;
        }
        else
        {
            rg.velocity = new Vector2(0, rg.velocity.y);
        }
    }
    
    public void SetActive(bool active)
    {
        Active = active;
        Pointer.SetActive(!active);
    }

    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, Ground_BoxSize, 0, -transform.up, Ground_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }
    public bool inWater()
    {
        if (Physics2D.BoxCast(transform.position, Water_BoxSize, 0, -transform.up, Water_CastDistance, LayerWater))
        {
            return true;
        }
        return false;
    }
    public bool isLeftWall()
    {
        if (Physics2D.BoxCast(transform.position - transform.up * 0.01f, Wall_BoxSize, 0, -transform.right, Wall_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }
    public bool isRightWall()
    {
        if (Physics2D.BoxCast(transform.position - transform.up * 0.01f, Wall_BoxSize, 0, transform.right, Wall_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * Ground_CastDistance, Ground_BoxSize);
        Gizmos.DrawWireCube(transform.position - transform.up * Water_CastDistance, Water_BoxSize);
        Gizmos.DrawWireCube(transform.position - transform.up * 0.01f - transform.right * Wall_CastDistance, Wall_BoxSize);
        Gizmos.DrawWireCube(transform.position - transform.up * 0.01f + transform.right * Wall_CastDistance, Wall_BoxSize);
    }
}
