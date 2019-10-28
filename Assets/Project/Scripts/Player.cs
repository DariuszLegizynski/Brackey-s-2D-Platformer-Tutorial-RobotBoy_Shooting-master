using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private Rigidbody2D myRigidbody2D;
    public BoxCollider2D boxColliderStand;
    public CapsuleCollider2D capsuleColliderCrouch;

    public float moveSpeed;
    public float jumpSpeed;

    private Animator anim;
    WeaponController weapon;

    bool ifReloaded = true;
    float timeToFire = 0;
    public float fireRate = 0f;

    private bool grounded;
    private bool crouch = false;
    private bool shoot = false;
    private bool aim = false;

    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance;

    public int fallDistance = -20;

    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        //sets our current health (or whatever stats are used) to the max health value at the start of the game
        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public PlayerStats playerStats = new PlayerStats();

    [Header("To acces health Bar (from MainCamera)")]
    [SerializeField]
    StatusIndicator statusIndicator;

	// Use this for initialization
	void Start ()
    {
        //boxColliderStand = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        //capsuleColliderCrouch = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();

        myRigidbody2D = GetComponentInChildren<Rigidbody2D>();

        anim = GetComponentInChildren<Animator>();

        playerStats.Init();

        if(statusIndicator == null)
        {
            Debug.LogError("PLAYER: no status indicator reference found");
        }

        else
        {
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }
	}

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.D))
        {
            GunFire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

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
    }

    void GunFire()
    {
        if (fireRate == 0)
        {
            Debug.Log("Am in fire rate loop");

            //if (Input.GetKey(KeyCode.D))
            //{
                anim.SetBool("Aim", aim);
                //weapon.GunAim();
                Debug.Log("ifReloaded is " + ifReloaded);
            //}

            if (Input.GetKeyUp(KeyCode.D) && ifReloaded == true)
            {
                StartCoroutine(weapon.Shoot());
                Debug.Log("Am in Start Coroutine shoot");
                weapon.WeaponFXEffects();
                anim.SetBool("Shoot", shoot);
                ifReloaded = false;
            }

            else if ((Input.GetKeyUp(KeyCode.D) && ifReloaded == false))
            {
                //TODO: PlaySound.EmptyMagazine("Click");
                anim.SetBool("Shoot", !shoot);
                Debug.Log("Reload needed!");
            }
            
            /*else //if ((Input.GetKeyUp(KeyCode.D) && ifReloaded == false))
            {
                //TODO: PlaySound.EmptyMagazine("Click");
                anim.SetBool("Shoot", !shoot);
                Debug.Log("Reload needed!");
            }*/
        }

        else
        {
            if (Input.GetKey(KeyCode.D) && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                StartCoroutine(weapon.Shoot());
                weapon.WeaponFXEffects();
                ifReloaded = false;
            }
        }
    }

    void Reload()
    {
        ifReloaded = true;
        Debug.Log("Reloaded");
    }

    public void DamagePlayer(int damage)
    {
        playerStats.currentHealth -= damage;

        if(playerStats.currentHealth <= 0)
        {
            Debug.Log("PLAYER KILLED!");
            GameMaster.KillPlayer(this);
        }

        statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }

    void Fall()
    {
        if (transform.position.y <= fallDistance)
        {
            DamagePlayer(playerStats.maxHealth);
        }
    }
}
