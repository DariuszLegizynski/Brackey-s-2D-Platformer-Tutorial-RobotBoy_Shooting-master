using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCamera : MonoBehaviour
{
    public Transform target;

    float nextTimeToSearch = 0;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector2(
                target.transform.position.x,
                0f);
        }

        else
        {
            FindPlayer();
            return;
        }
    }

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
            if (searchResult != null)
                target = searchResult.transform;
            nextTimeToSearch = Time.time + 0.5f;
        }
    }
}
