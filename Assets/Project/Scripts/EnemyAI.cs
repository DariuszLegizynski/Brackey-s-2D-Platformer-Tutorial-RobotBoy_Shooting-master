using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
//B using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //TODO: Find error for sprite changing view directions

    public Transform target;
    public Transform enemyGFX;

    List<Vector3> path;

    private Animator anim;
    bool shoot = true;

    [Header("Movement")]
    public float moveSpeed;
    public float attackRange;
    public float yPathOffset;

    //Brackeys Solution
    //public float walkSpeed = 200f;
    //public float nextWaypointDistance = 3f;
    //public float attackRange;

    //the calculated path
    //B Path path;

    //waypoint we are currently moving towards
    //B int currentWaypoint = 0;

    //B bool reachedEndOfPath = false;

    //caching
    //B Seeker seeker;
    Rigidbody2D rb;
    WeaponController weapon;

    bool searchingForPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        //B seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<WeaponController>();

        //B
        /*
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

            return;
        }*/

        // Start a new path to the target position, return the result to the OnPathComplete method
        //B seeker.StartPath(transform.position, target.position, OnPathComplete);

        //B StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //B
        /*
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

        return;
        }*/

        Movement();
    }

    void Movement()
    {
        float playerDist = Vector2.Distance(transform.position, target.transform.position);

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
                //ChaseTarget();
            
            //here, after shooting will be a function to chase the target and engage it in melee
            /* Not sure if wanna use. If the enemy cant shoot (becouse he shot already, than he either will reload or go into melee
             * else
            {
                //TODO: Reload animation
                Debug.Log("Reloading");
            }*/
        }

        //B
        //Pathfinding
        /* if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        else
        {
            reachedEndOfPath = false;
        }*/

        //B
        /*
        //Add force to the found pathway
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * walkSpeed * Time.fixedDeltaTime;

        rb.AddForce(force);

        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }*/

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

        //look at target
        Vector3 dir = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.up * angle;
    }

    void ChaseTarget()
    {
        if (path.Count == 0)
            return;

        //move towards the closes path
        transform.position = Vector3.MoveTowards(transform.position, path[0] + new Vector3(0, yPathOffset, 0), moveSpeed * Time.deltaTime);

        if (transform.position == path[0] + new Vector3(0, yPathOffset, 0))
            path.RemoveAt(0);
    }

    void UpdatePath()
    {
        // calculate a path to our target
        NavMeshPath navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, navMeshPath);

        // save that as a list
        path = navMeshPath.corners.ToList();
    }

    //B
    /*
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
            //B StartCoroutine(UpdatePath());
            UpdatePath();
            yield return false;
        }
    }*/
}
