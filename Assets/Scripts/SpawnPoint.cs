using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public PlayerHandler playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        if (ph == null)
        {
            ph = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }


        //ph.pc.SetToTransform(transform);


    }

    // Update is called once per frame
    void Update()
    {

    }
}