using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizzBuzzVariant
{
    public string name;
    public int factor;
}

public class FizzBuzz : MonoBehaviour
{
    public FizzBuzzVariant[] variants;

    float counter;

    public float delay = 0.5f;
    float timer;


    

    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            counter += 1;

            string message = "";
            foreach(FizzBuzzVariant f in variants)
            {
                if (counter % f.factor == 0)
                {
                    message += f.name;
                }
            }

            print(message);
        }
    }

    void FizzBuzzCommandLoop(int count)
    {
        for (int counter = 0; counter < count; counter++)
        {
            FizzBuzzCheck(counter);
        }
    }

    void FizzBuzzCheck(int counter)
    {
        string message = "";
        foreach (FizzBuzzVariant f in variants)
        {
            if (counter % f.factor == 0)
            {
                message += f.name;
            }
        }

        print(message);
    }

    void FizzBuzzAction(int i)
    {

    }

    
}
