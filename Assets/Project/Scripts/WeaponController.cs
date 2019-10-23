using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float fireRate = 0f;
    //for future use
    //TODO: public int ammoRate = 0;
    public int damage = 10;
    public LayerMask whatToHit;
    public LineRenderer lineRenderer;

    bool ifReloaded = true;

    private float timeToFire = 0;

    public Transform muzzle;
    public GameObject hitEffect;
    public Transform muzzleFlashPrefab;
    public GameObject weaponSmokePrefab;
    public GameObject muzzleSmokePrefab;

    // Update is called once per frame
    void Update()
    {
        GunFire();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
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
                StartCoroutine(Shoot());
                WeaponFXEffects();
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
                StartCoroutine(Shoot());
            }
        }

    }

    IEnumerator Shoot()
    {
        Vector2 muzzlePos = new Vector2(muzzle.transform.position.x, muzzle.transform.position.y);

        RaycastHit2D hitInfo = Physics2D.Raycast(muzzlePos, muzzle.transform.up, 100f, whatToHit);
        Debug.LogError("PifPaf!");

        if (hitInfo)
        {
            Debug.Log("We hit " + hitInfo.collider.name + " and did " + damage + " damage.");

            EnemyBehaviour enemy = hitInfo.transform.GetComponent<EnemyBehaviour>();

            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            GameObject cloneHitEffect = Instantiate(hitEffect, hitInfo.point, Quaternion.identity);

            lineRenderer.SetPosition(0, muzzle.position);
            lineRenderer.SetPosition(1, hitInfo.point);

            Destroy(cloneHitEffect.gameObject, 1.5f);
        }

        else
        {
            Debug.DrawRay(muzzlePos, muzzle.transform.up * 100f, Color.yellow);
            lineRenderer.SetPosition(0, muzzle.position);
            lineRenderer.SetPosition(1, muzzle.transform.up * 100f);
        }

        lineRenderer.enabled = true;

        //wait for one frame
        yield return new WaitForSeconds(0.02f);

        lineRenderer.enabled = false;
    }

    void Reload()
    {
        ifReloaded = true;

        //TODO: Reload animation
        Debug.Log("Reloading");
    }

    void WeaponFXEffects()
    {
        Vector2 muzzleRot = muzzle.rotation.eulerAngles;
        muzzleRot = new Vector2(muzzle.rotation.x, muzzle.rotation.y + 90);
        Transform cloneMuzleFlash = (Transform)Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation); //Quaternion.Euler(muzleRot));

        float size = Random.Range(1.6f, 1.9f);
        cloneMuzleFlash.transform.localScale = new Vector3(size, size / 2, size);
        Destroy(cloneMuzleFlash.gameObject, 0.02f);

        GameObject cloneWeaponSmokePrefab = Instantiate(weaponSmokePrefab, transform.position, transform.rotation);
        Destroy(cloneWeaponSmokePrefab.gameObject, 5f);

        GameObject cloneMuzzleSmokePrefab = Instantiate(muzzleSmokePrefab, muzzle.position, muzzle.rotation);
        Destroy(cloneMuzzleSmokePrefab.gameObject, 5f);
    }
}
