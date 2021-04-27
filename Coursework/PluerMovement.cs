using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PluerMovement : MonoBehaviour
{
    [SerializeField] private float JumpForce = 400f;
    [Range(0, 0.3f)] [SerializeField] private float MovementSmoothing = 0.05f;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private Transform FloorCheck;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private Transform Camera;
    [SerializeField] private GameObject LeftShooter;
    [SerializeField] private GameObject RightShooter;
    [SerializeField] private GameObject uiend;
    [SerializeField] private GameObject uistart;
    [SerializeField] private Text counter;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    private bool endgame = false;
    private bool started = true;
    private bool dead = false;
    private Animator anime;
    const float FloorCheckRadius = 0.2f;
    private bool OnFloor;
    private bool jump = false;
    private bool doubleJump = true;
    private Rigidbody2D myRigidbody2D;
    private bool diRight = true;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private int coinCount = 0;
    private float horMove = 0f;
    [SerializeField] private float runSpeed = 40f;

    private enum State { idle, run, jump, fall, deth, spawn, end }
    private State curState = State.idle;

    // Start is called before the first frame update
    void Start()
    {
		//initialize rigidbody, animator and turn off ending screen 
        myRigidbody2D = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        uiend.SetActive(false);
        endScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (endgame)
        {//if ending screen quit
            if (Input.anyKeyDown) Application.Quit();
        }
        if (started)
        {//if starting screen turn him off and turn controls on
            if (Input.anyKeyDown)
            {
                started = false;
                uistart.SetActive(false);
                startScreen.SetActive(false);
            }
        }
        else
        {//if not dead move pluer
            if (!dead)
            {
                horMove = Input.GetAxis("Horizontal") * runSpeed;
                if (Input.GetButtonDown("Jump"))
                {
                    curState = State.jump;
                    jump = true;
                }
                if (Input.GetButtonDown("Cancel"))
                {
                    Application.Quit();
                }
            }
        }
    }

    private void FixedUpdate()
    {//if not dead move pluer
        if (!dead)
        {
            floorCheck();
            Move(horMove * Time.fixedDeltaTime, jump);
            jump = false;
            floorCheck();
            StateChanger();
            anime.SetInteger("AnimeState", (int)curState);
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (!dead)
            switch (collision.gameObject.tag)
            {//if not dead and collision with DETH go to death animation
                case "DETH":
                    {
                        dead = true;
                        curState = State.deth;
                        myRigidbody2D.velocity = new Vector3(0, 0, 0);
                        anime.SetInteger("AnimeState", (int)curState);
                    }break;
                default:
                    {

                    }break;
            }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead)
            switch (collision.gameObject.tag)
            {
                case "DETH":
                    {//if not dead and collision with DETH go to death animation
                        dead = true;
                        curState = State.deth;
                        myRigidbody2D.velocity = new Vector3(0, 0, 0);
                        anime.SetInteger("AnimeState", (int)curState);
                    }break;
                case "Coin":
                    {//if not dead and collision with Coin increase coinCount
                        counter.text = coinCount.ToString();
                        Animator animeCoin = collision.gameObject.GetComponent<Animator>();
                        if(!animeCoin.GetBool("Collected")) coinCount++;
                        animeCoin.SetBool("Collected", true);
                    }break;
                case "Exit":
                    {//if not dead and collision with Exit show ending screen
                        endgame = true;
                        uiend.SetActive(true);
                        endScreen.SetActive(true);
                        curState = State.idle;
                        myRigidbody2D.velocity = Vector3.zero;
                        transform.position = new Vector3(39.02f, 23.25f, 0f);
                        anime.SetInteger("AnimeState", (int)curState);
                        dead = true;
                    }break;
                default:
                    {

                    }break;
            }
    }

    private void floorCheck()
    {//Chech if floor down
        bool onfloor = OnFloor;
        OnFloor = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(FloorCheck.position, FloorCheckRadius, Ground);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                OnFloor = true;
                doubleJump = true;
            }
        }
    }

    private void Move(float move, bool jump)
    {
		//Add speed
        Vector3 targetVelocity = new Vector2(move * 10f, myRigidbody2D.velocity.y);
        myRigidbody2D.velocity = Vector3.SmoothDamp(myRigidbody2D.velocity, targetVelocity, ref velocity, MovementSmoothing);
		//Flip for needed direction
        if (move > 0 && !diRight)
        {
            Flip();
        }
        else if (move < 0 && diRight)
        {
            Flip();
        }
		//Jump
        if ((OnFloor && jump) || (doubleJump && curState == State.jump && jump) || (doubleJump && curState == State.fall && jump))
        {
            if (!OnFloor)
            {
                doubleJump = false;
            }
            OnFloor = false;
            if (myRigidbody2D.velocity.y < 0.1f) myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 0);
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, JumpForce);
            jump = false;
        }

    }

    private void Flip()
    {
        diRight = !diRight;

        transform.Rotate(0f, 180f, 0f);
    }

    private void StateChanger()
    {
        switch (curState)
        {
            case State.jump:
                {//Negative vertical speed == falling
                    if (myRigidbody2D.velocity.y < -0.001f) curState = State.fall;
                }
                break;

            case State.fall:
                {//OnFloor == idle
                    if (OnFloor) curState = State.idle;
                }
                break;

            default:
                {//Horizontal speed > 1f == run
                    if (Mathf.Abs(myRigidbody2D.velocity.x) > 1f)
                    {
                        curState = State.run;
                    }
                    else
                    {
                        curState = State.idle;
                    }
					//Negative vertical speed == falling
                    if (myRigidbody2D.velocity.y < -0.001f && !OnFloor) curState = State.fall;
                }
                break;
        }
        
    }

    private void SpawnStart()
    {//Set position for spawn
        curState = State.spawn;
        Camera.position = new Vector3(13.63f, -0.4199999f, -10f);
        transform.position = SpawnPoint.position;
        myRigidbody2D.velocity = Vector3.zero;
        Animator disabler = LeftShooter.GetComponent<Animator>();
        disabler.SetBool("Enabled", false);
        disabler = RightShooter.GetComponent<Animator>();
        disabler.SetBool("Enabled", false);
        anime.SetInteger("AnimeState", (int)curState);
    }

    private void SpawnEnd()
    {//Give controls to player
        dead = false;
        Camera.position = new Vector3(13.63f, -0.4199999f, -10f);
        curState = State.idle;
        anime.SetInteger("AnimeState", (int)curState);
    }
}
