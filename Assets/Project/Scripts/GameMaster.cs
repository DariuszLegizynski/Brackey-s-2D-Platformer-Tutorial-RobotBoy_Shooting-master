using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 1;
    public Transform spawnPrefab;

    void Start()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public IEnumerator RespawnPlayer()
    {
        Debug.Log("TODO: Add wait for spawn sound");
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine (gm.RespawnPlayer());
    }
}
