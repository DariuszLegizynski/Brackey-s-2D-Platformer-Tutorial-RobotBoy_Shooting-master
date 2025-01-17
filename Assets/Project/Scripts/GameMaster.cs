﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //TODO: Death Animation
    public GameObject deathEffectPrefab;

    public static GameMaster gm;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 1;
    public GameObject spawnPrefab;

    //public AudioClip respawnAudio;
    public AudioSource audioSource;

    void Start()
    {
        //audioSource.clip = respawnAudio;

        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);
        audioSource.Play();

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject cloneSpawnParticle = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(cloneSpawnParticle, 3f);
    }

    public static void KillPlayer(Player player)
    {
        GameObject clonedeathEffectPrefab = Instantiate(gm.deathEffectPrefab, gm.transform.position, gm.transform.rotation);
        Destroy(clonedeathEffectPrefab.gameObject, 3f);
        Destroy(player.gameObject);
        gm.StartCoroutine (gm.RespawnPlayer());
    }

    public static void KillEnemy(EnemyBehaviour enemy)
    {
        GameObject clonedeathEffectPrefab = Instantiate(gm.deathEffectPrefab, gm.transform.position, gm.transform.rotation);
        Destroy(clonedeathEffectPrefab.gameObject, 3f);
        Destroy(enemy.gameObject);
    }
}
