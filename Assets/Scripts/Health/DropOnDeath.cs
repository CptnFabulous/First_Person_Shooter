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
    public Vector3 dropZone;

    Health h;

    private void Awake()
    {
        h = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (h.IsAlive() == false && droppedItems.Count > 0)
        {
            foreach(ItemStack s in droppedItems)
            {
                for(int i = 0; i < s.quantity; i++)
                {
                    Vector3 dropArea = new Vector3(Random.Range(-dropZone.x / 2, dropZone.x / 2), Random.Range(-dropZone.y / 2, dropZone.y / 2), Random.Range(-dropZone.y / 2, dropZone.y / 2));
                    Instantiate(s.item, transform.position + dropArea, Quaternion.identity);
                }
            }

            enabled = false;
        }
    }
}
