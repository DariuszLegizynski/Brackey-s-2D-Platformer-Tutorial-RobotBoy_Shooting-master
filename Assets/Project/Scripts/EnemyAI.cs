﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //TODO: Find error for sprite changing view directions

    public Transform target;

    private Animator anim;

    private bool shoot = false;
    bool ifReloaded = true;
    public float speed = 200f;
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
        /*
        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist <= attackRange)
        {
            if (weapon.CanShoot())
                weapon.GunFire();
        }

        else
        {
            ChaseTarget();
        }
        */

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
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        rb.AddForce(force);

        float playerDist = Vector2.Distance(transform.position, target.transform.position);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;

            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            if (playerDist <= attackRange && ifReloaded == true)
            {
                weapon.GunFire();
                weapon.WeaponFXEffects();
                anim.SetBool("Shoot", shoot);
                ifReloaded = false;
            }

            else if (ifReloaded == false)
            {
                //TODO: Reload animation
                Debug.Log("Reloading");
                ifReloaded = true;
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
