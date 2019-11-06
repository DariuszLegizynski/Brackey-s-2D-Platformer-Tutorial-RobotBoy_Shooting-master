using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //for future use
    //TODO: public int ammoRate = 0;
    public int damage = 10;
    public LayerMask whatToHit;
    public LineRenderer lineRenderer;

    public Transform muzzle;
    public GameObject hitEffect;
    public GameObject bloodHitEffectPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject weaponSmokePrefab;
    public GameObject muzzleSmokePrefab;

    public float shootRate;
    float lastShootTime;
    bool isReloaded = true;

    public void GunFire()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        lastShootTime = Time.time;
        isReloaded = false;

        Vector2 muzzlePos = new Vector2(muzzle.transform.position.x, muzzle.transform.position.y);

        RaycastHit2D hitInfo = Physics2D.Raycast(muzzlePos, muzzle.transform.up, 100f, whatToHit);
        Debug.LogError("PifPaf!");

        if (hitInfo)
        {
            Debug.Log("We hit " + hitInfo.collider.name + " and did " + damage + " damage.");

            EnemyBehaviour enemy = hitInfo.transform.GetComponent<EnemyBehaviour>();

            Player player = hitInfo.transform.GetComponent<Player>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                GameObject cloneBloodHitEffect = Instantiate(bloodHitEffectPrefab, hitInfo.point, Quaternion.identity);
                Destroy(cloneBloodHitEffect.gameObject, 1.5f);
            }

            else if (player != null)
            {
                player.DamagePlayer(damage);
                GameObject cloneBloodHitEffect = Instantiate(bloodHitEffectPrefab, hitInfo.point, Quaternion.identity);
                Destroy(cloneBloodHitEffect.gameObject, 1.5f);
            }

            else
            {
                GameObject cloneHitEffect = Instantiate(hitEffect, hitInfo.point, Quaternion.identity);
                Destroy(cloneHitEffect.gameObject, 1.5f);
            }

            lineRenderer.SetPosition(0, muzzle.position);
            lineRenderer.SetPosition(1, hitInfo.point);
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

    public void WeaponFXEffects()
    {
        Vector2 muzzleRot = muzzle.rotation.eulerAngles;
        muzzleRot = new Vector2(muzzle.rotation.x, muzzle.rotation.y);
        GameObject cloneMuzleFlash = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation);  //Quaternion.Euler(muzzleRot));

        float size = Random.Range(1.6f, 1.9f);
        cloneMuzleFlash.transform.localScale = new Vector3(size, size / 2, size);
        Destroy(cloneMuzleFlash.gameObject, 0.02f);

        GameObject cloneWeaponSmokePrefab = Instantiate(weaponSmokePrefab, transform.position, transform.rotation);
        Destroy(cloneWeaponSmokePrefab.gameObject, 5f);

        GameObject cloneMuzzleSmokePrefab = Instantiate(muzzleSmokePrefab, muzzle.position, muzzle.rotation);
        Destroy(cloneMuzzleSmokePrefab.gameObject, 5f);
    }

    public bool CanShoot()
    {
        Debug.Log("isReloaded2: " + shootRate);

        if (Time.time - lastShootTime >= shootRate)
        {
            if (isReloaded == true)
                return true;
        }

        return false;
    }

    public void Reload()
    {
        isReloaded = true;
    }
}