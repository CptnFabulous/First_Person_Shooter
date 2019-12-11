using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGate : MonoBehaviour
{
    public List<Health> enemiesRemaining;
    public UnityEvent onDefeat;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Health h in enemiesRemaining)
        {
            if (h.IsAlive() == false)
            {
                enemiesRemaining.Remove(h);
            }
        }

        if (enemiesRemaining.Count <= 0)
        {
            print("All enemies defeated");
            onDefeat.Invoke();
        }
    }
}
