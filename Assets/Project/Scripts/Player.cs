using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private Rigidbody2D myRigidbody2D;
    public BoxCollider2D boxColliderStand;
    public CapsuleCollider2D capsuleColliderCrouch;

    public float moveSpeed;
    public float jumpSpeed;

    private Animator anim;

    private bool grounded;
    private bool crouch = false;
    private bool shoot = false;
    private bool aim = false;

    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance;

    [System.Serializable]
    public class PlayerStats
    {
        public int Health = 100;
    }

    public PlayerStats playerStats = new PlayerStats();

    public int fallDistance = -20;

	// Use this for initialization
	void Start ()
    {
        //boxColliderStand = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        //capsuleColliderCrouch = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();

        myRigidbody2D = GetComponentInChildren<Rigidbody2D>();

        anim = GetComponentInChildren<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        Movement();

        Fall();
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            myRigidbody2D.velocity = new Vector2(-moveSpeed, myRigidbody2D.velocity.y);
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myRigidbody2D.velocity = new Vector2(moveSpeed, myRigidbody2D.velocity.y);
        }

        else
        {
            myRigidbody2D.velocity = new Vector2(0f, myRigidbody2D.velocity.y);
        }

        anim.SetFloat("Speed", Mathf.Abs(myRigidbody2D.velocity.x));

        if (myRigidbody2D.velocity.x < 0)
        {
            //transform.Rotate(0f, 180f, 0f);
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        else if (myRigidbody2D.velocity.x > 0)
        {
            //transform.Rotate(0f, 180f, 0f);
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpSpeed);
        }

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, whatIsGround);
        anim.SetBool("Jump", !grounded);

        if (Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("Crouch", crouch);

            //boxColliderComponent.offset = new Vector2(0f, 0.5f);
            //boxColliderComponent.size = new Vector2(1f, 2f);
            capsuleColliderCrouch.enabled = true;
            boxColliderStand.enabled = false;
        }

        else
        {
            anim.SetBool("Crouch", !crouch);
            //boxColliderComponent.size = new Vector2 (1f, 3.17f);
            boxColliderStand.enabled = true;
            capsuleColliderCrouch.enabled = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("Aim", aim);
        }

        else
        {
            anim.SetBool("Aim", !aim);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetBool("Shoot", shoot);
        }

        else
        {
            anim.SetBool("Shoot", !shoot);
        }
    }

    public void DamagePlayer(int damage)
    {
        playerStats.Health -= damage;

        if(playerStats.Health <= 0)
        {
            Debug.Log("PLAYER KILLED!");
            GameMaster.KillPlayer(this);
        }
    }

    void Fall()
    {
        if (transform.position.y <= fallDistance)
        {
            DamagePlayer(playerStats.Health);
        }
    }
}
