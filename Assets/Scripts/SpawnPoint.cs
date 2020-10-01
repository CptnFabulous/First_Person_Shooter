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
        if (ph != null)
        {
            ph.transform.position = transform.position;
        }
        else
        {
            ph = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }


        ph.pc.SetToTransform(transform);


        //Vector3 rotationValues = transform.rotation.eulerAngles;
        //Vector3 rotationValues = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        //print(rotationValues);

        //ph.transform.rotation = Quaternion.Euler(0, rotationValues.y, 0);
        //ph.pc.head.transform.localRotation = Quaternion.Euler(rotationValues.x, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}