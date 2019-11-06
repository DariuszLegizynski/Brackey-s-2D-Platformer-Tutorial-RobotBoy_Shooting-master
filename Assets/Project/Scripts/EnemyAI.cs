using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //TODO: Find error for sprite changing view directions

    public Transform target;

    private Animator anim;
    bool shoot = true;

    public float walkSpeed = 200f;
    public float nextWaypointDistance = 3f;
    public float attackRange;

    public Transform enemyGFX;

    //the calculated path
    Path path;

    //waypoint we are currently moving towards
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

        //Add force to the found pathway
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * walkSpeed * Time.fixedDeltaTime;

        rb.AddForce(force);

        float playerDist = Vector2.Distance(transform.position, target.transform.position);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;

            Debug.LogWarning("velocity is: " + rb.velocity.x);

            if (playerDist <= attackRange)
            {
                //anim.SetBool("Shoot", shoot = true);

                if (weapon.CanShoot())
                {
                    weapon.GunFire();
                    weapon.WeaponFXEffects();
                }

                else
                    weapon.Reload();
                //here, after shooting will be a function to chase the target and engage it in melee
                /* Not sure if wanna use. If the enemy cant shoot (becouse he shot already, than he either will reload or go into melee
                 * else
                {
                    //TODO: Reload animation
                    Debug.Log("Reloading");
                }*/
            }
        }

        if (rb.velocity.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(1.5f, 1.5f, 1f);
        }

        else if (rb.velocity.x < 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1.5f, 1.5f, 1f);
        }

        //anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        //Debug.LogWarning("velocity Mathf is: " + rb.velocity.x);
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

        if(sResult == null)
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
