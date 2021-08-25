using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof (Health))]
public class DropOnDeath : MonoBehaviour
{
    [System.Serializable]
    public class ItemStack
    {
        public GameObject item;
        public int quantity;
    }

    public List<ItemStack> droppedItems;
    public Vector3 zoneLocalCenter = Vector3.one;
    public Vector3 zoneExtents = new Vector3(0.5f, 1f, 0.5f);

    Health h;

    private void Awake()
    {
        h = GetComponent<Health>();
        h.onDeath.AddListener(DropItems);
    }

    void DropItems()
    {
        foreach (ItemStack s in droppedItems)
        {
            for (int i = 0; i < s.quantity; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-zoneExtents.x, zoneExtents.x), Random.Range(-zoneExtents.y, zoneExtents.y), Random.Range(-zoneExtents.z, zoneExtents.z));
                Vector3 relativeToTransform = transform.rotation * (zoneLocalCenter + randomPosition);
                Instantiate(s.item, transform.position + relativeToTransform, Quaternion.identity);
            }
        }
        //enabled = false;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (h.IsDead == false && droppedItems.Count > 0)
        {
            foreach(ItemStack s in droppedItems)
            {
                for(int i = 0; i < s.quantity; i++)
                {
                    Vector3 randomPosition = new Vector3(Random.Range(-zoneExtents.x, zoneExtents.x), Random.Range(-zoneExtents.y, zoneExtents.y), Random.Range(-zoneExtents.z, zoneExtents.z));
                    Vector3 relativeToTransform = transform.rotation * (zoneLocalCenter + randomPosition);
                    Instantiate(s.item, transform.position + relativeToTransform, Quaternion.identity);
                }
            }

            
        }
    }
    */
}
