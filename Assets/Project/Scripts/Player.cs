using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Rigidbody2D myRigidbody2D;
    public BoxCollider2D boxColliderStand;
    public CapsuleCollider2D capsuleColliderCrouch;

    public float moveSpeed;
    public float jumpSpeed;

    private Animator anim;

    WeaponController weapon;
    bool ifReloaded = true;
    private float timeToFire = 0;
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
    void Start()
    {
        //boxColliderStand = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        //capsuleColliderCrouch = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();

        myRigidbody2D = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        weapon = GetComponentInChildren <WeaponController>();

        playerStats.Init();

        if (statusIndicator == null)
        {
            statusIndicator = GameObject.FindGameObjectWithTag("StatusIndicator").GetComponent<StatusIndicator>();
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }

        else if (statusIndicator != null)
        {
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }

        else
            Debug.LogError("PLAYER: no status indicator reference found");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Fall();

        GunFire();

        Reload();
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

    void GunFire()
    {
        if (fireRate == 0)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("ifReloaded is " + ifReloaded);
                //TODO: Play sound "Gun Aim"
            }

            else if (Input.GetKeyUp(KeyCode.D) && ifReloaded == true)
            {
                weapon.GunFire();
                weapon.WeaponFXEffects();
                ifReloaded = false;
            }

            else if ((Input.GetKeyUp(KeyCode.D) && ifReloaded == false))
            {
                //TODO: PlaySound.EmptyMagazine("Click");
                Debug.Log("Reload needed!");
            }

            /*
            else
            {
                //TODO: Play sound. no more aiming
                Debug.Log("Not aiming anymore");
            }
            */
        }

        else
        {
            if (Input.GetKey(KeyCode.D) && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                weapon.GunFire();
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ifReloaded = true;

            //TODO: Reload animation
            Debug.Log("Reloading");
        }
    }

    public void DamagePlayer(int damage)
    {
        playerStats.currentHealth -= damage;

        if (playerStats.currentHealth <= 0)
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