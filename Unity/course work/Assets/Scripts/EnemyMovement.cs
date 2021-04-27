using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform FloorPoint;
    [SerializeField] private bool isRight;
    [SerializeField] private float speed;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask Ground;
    private Rigidbody2D myrb;

    // Start is called before the first frame update
    void Start()
    {
        myrb = GetComponent<Rigidbody2D>();
        if (isRight) myrb.velocity = new Vector2(speed, 0);
        else myrb.velocity = new Vector2(-speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRight) myrb.velocity = new Vector2(speed, 0);
        else myrb.velocity = new Vector2(-speed, 0);
    }

    private void FixedUpdate()
    {
        if(isRight)
        {
            isRight = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(FloorPoint.position, checkRadius, Ground); 
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    isRight = true;
                }
            }
            if (!isRight)
            {
                transform.Rotate(0f, 180f, 0f);
            }
        }
        else
        {
            isRight = true;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(FloorPoint.position, checkRadius, Ground);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    isRight = false;
                }
            }
            if(isRight)
            {
                transform.Rotate(0f, 180f, 0f);
            }
        }

    }
}
