using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //TODO: Find error for sprite changing view directions

    public Transform target;

    private Animator anim;
    public int waitTimeForAnim = 2;

    public float walkSpeed = 200f;
    public float nextWaypointDistance = 3f;
    public float ShootAttackRange = 8f;
    public float meleeAttackRange = 2.5f;
    public float meleeAttackRangeDelta = 1.1f;

    public Transform enemyGFX;

    //the calculated path
    Path path;

    //waypoint that we are currently moving towards
    int currentWaypoint = 0;

    bool reachedEndOfPath = false;

    //caching
    Seeker seeker;
    Rigidbody2D rb;
    WeaponController weapon;

    bool searchingForPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<WeaponController>();
        anim = GetComponentInChildren<Animator>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

            return;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

            return;
        }

        Movement();
    }

    void Movement()
    {
        transform.position = this.transform.position;
        //Pathfinding
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        else
        {
            reachedEndOfPath = false;
        }

        //face and move to the found pathway
        //TODO: fix -> direction causes the character to lag and flip
        //Vector2 directionToNextWaypoint = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 directionToNextWaypoint = ((Vector2)path.vectorPath[1] - (Vector2)path.vectorPath[0]).normalized;
        Debug.Log("directionToNextWaypoint: " + directionToNextWaypoint);
        Debug.Log("path.vectorPath[0]: " + path.vectorPath[0]);
        Debug.Log("path.vectorPath[currentWaypoint]: " + path.vectorPath[currentWaypoint]); //w tym jest problem z kalkulacją. Czasami ta liczba jest taka sama, jak rb.position i wtedy wychodzi wartość, która odwraca postać.
        Debug.Log("rb.position: " + rb.position);
        Debug.Log("path.vectorPath[currentWaypoint] - rb.position: " + ((Vector2)path.vectorPath[currentWaypoint] - rb.position));

        //first movement solution
        //Vector2 force = directionToNextWaypoint * walkSpeed * Time.fixedDeltaTime;
        //rb.AddForce(force);

        //second movement solution
        //rb.velocity = new Vector2(directionToNextWaypoint.x * walkSpeed * Time.fixedDeltaTime, rb.velocity.y);

        if (directionToNextWaypoint.x > 0.01f)
        {
            enemyGFX.localScale = new Vector3(1.5f, 1.5f, 0);
        }

        else if (directionToNextWaypoint.x <= 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1.5f, 1.5f, 0);
        }
        
        float playerDist = Vector2.Distance(transform.position, target.transform.position);
        float distanceToCurrentWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distanceToCurrentWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (playerDist > ShootAttackRange)
        {
            rb.velocity = Vector2.zero;

            //TODO: Reload only once, need to find soution for checking the boolean of the Can.Shoot() function
            if (!weapon.CanShoot())
            {
                weapon.Reload();
                Debug.LogWarning("Enemy Reloads!");
            }
        }

        else if (playerDist <= ShootAttackRange && weapon.CanShoot())
        {
            anim.SetBool("Aim", true);

            //makes the enemy not move anymore
            //if (aimAnimation == true)
            //    transform.position = this.transform.position;

            //else
            //{
                anim.SetTrigger("Shoot");

                weapon.GunFire();
                weapon.WeaponFXEffects();
            //}

            anim.SetBool("Aim", false);
        }

        else// if (playerDist <= AttackRange && (anim.GetBool("Aim") == false)) // && playerDist < meleeAttackRange)
        {
            if (path.vectorPath.Count == 0)
                return;

            rb.velocity = new Vector2(directionToNextWaypoint.x * walkSpeed * Time.fixedDeltaTime, rb.velocity.y);

            if (Vector2.Distance(rb.position, target.transform.position) <= meleeAttackRange)
            {
                //rb.velocity = Vector2.zero;
                rb.velocity = Vector2.MoveTowards(rb.position, target.transform.position, meleeAttackRangeDelta);
            }

            MeleeAttack();
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void MeleeAttack()
    {
        //TODO: Melee Attack including distance and so
        //if playerDistance(from Movement()) is lesser then 1.1f, then stop and hit with sword
        Debug.LogWarning("Melee ATTACK!");
    }

    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

            yield return null;
        }

        else
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(UpdatePath());
        }
    }

    void OnPathComplete(Path p)
    {
        //was our path succesfull?
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator SearchForPlayer()
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");

        if (sResult == null)
        {
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(SearchForPlayer());
        }

        else
        {
            target = sResult.transform;
            searchingForPlayer = false;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }
}