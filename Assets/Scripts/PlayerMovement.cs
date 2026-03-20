using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rg;

    public string PlayerAxes;
    public KeyCode JumpCode;
    public float MovePower;
    public float JumpPower = 35;

    public bool Jump = false;
    public bool Move = false;

    [Header("BoxCast")]
    public Vector2 Ground_BoxSize;
    public float Ground_CastDistance;
    public LayerMask LayerGround;
    public Vector2 Wall_BoxSize;
    public float Wall_CastDistance;

    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKey(JumpCode) && isGrounded())
        {
            Jump = true;
        }

        MovePower = Input.GetAxisRaw(PlayerAxes);
        if(isLeftWall() && MovePower < 0) MovePower = 0;
        if(isRightWall() && MovePower > 0) MovePower = 0;
        if(MovePower != 0) Move = true;
    }

    void FixedUpdate()
    {
        if(Jump) rg.AddForce(new Vector2(0, JumpPower * 10));
        Jump = false;

        if(Move) rg.velocity = new Vector2(MovePower * 5, rg.velocity.y);
        Move = false;

    }

    public bool isGrounded()
    {
        if(Physics2D.BoxCast(transform.position, Ground_BoxSize, 0, -transform.up, Ground_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }

    public bool isLeftWall()
    {
        if(Physics2D.BoxCast(transform.position - transform.up * 0.142f, Wall_BoxSize, 0, -transform.right, Wall_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }
    
    public bool isRightWall()
    {
        if(Physics2D.BoxCast(transform.position - transform.up * 0.142f, Wall_BoxSize, 0, transform.right, Wall_CastDistance, LayerGround))
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * Ground_CastDistance, Ground_BoxSize);
        // Recalculate for Player Model when have
        Gizmos.DrawWireCube((transform.position - transform.up * 0.142f) - transform.right * Wall_CastDistance, Wall_BoxSize);
        Gizmos.DrawWireCube((transform.position - transform.up * 0.142f) + transform.right * Wall_CastDistance, Wall_BoxSize);
    }
}
