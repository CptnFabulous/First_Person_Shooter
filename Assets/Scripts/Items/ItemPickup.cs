using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemPickup : MonoBehaviour
{
    public int value;
    public bool consumeAll = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnTriggerEnter(Collider c)
    {
        // Do item pickup stuff

        if (value <= 0)
        {
            Destroy(gameObject);
        }
    }





    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Blue" && this.gameObject.tag != "redweapon")
        {
            Blue.gameObject.tag = "blueownwep";
            Destroy(rb);
            Destroy(bc);
            this.gameObject.tag = "blueweapon";
            this.gameObject.transform.parent = BlueWepHolder.transform;
            if (refblue.playerfacingleft == true)
            {
                transform.localScale = new Vector2(2, 2);
                this.gameObject.transform.position = BlueWepHolder.gameObject.transform.position;
            }
            else if (refblue.playerfacingleft == false)
            {
                transform.localScale = new Vector2(2, 2);
                this.gameObject.transform.position = BlueWepHolder.gameObject.transform.position;
            }
            DestroythisweaponBlue();
        }
    }
    */
}
