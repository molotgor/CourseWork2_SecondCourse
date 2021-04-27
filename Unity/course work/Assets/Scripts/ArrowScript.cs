using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private bool isRight;
    [SerializeField] private float speed;
    private Rigidbody2D myrb;
    private Animator anime;
    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponent<Animator>();
        myrb = GetComponent<Rigidbody2D>();
        if (isRight) myrb.velocity = new Vector2(speed, 0);
        else myrb.velocity = new Vector2(-speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                {
                    myrb.velocity = new Vector2(0, -0.5f);
                    myrb.gravityScale = 1;
                    anime.SetBool("Broken", true);
                }
                break;
            case "Player":
                {
                    myrb.velocity = new Vector2(0, -0.5f);
                    myrb.gravityScale = 1;
                    anime.SetBool("Broken", true);
                }
                break;
        }
    }

    private void EndArrow()
    {
        Destroy(gameObject);
    }
}
