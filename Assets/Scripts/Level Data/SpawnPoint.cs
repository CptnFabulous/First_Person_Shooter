using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public PlayerHandler playerPrefab;

    public void SpawnInPlayerForFirstTime()
    {
        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        if (ph == null)
        {
            ph = Instantiate(playerPrefab);
        }

        Spawn(playerPrefab);
    }

    public void Spawn(PlayerHandler playerToSpawn)
    {
        
    }
}