using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



[RequireComponent(typeof(EventObserver))]
public class HUD : MonoBehaviour
{
    EventObserver eo;
    public PlayerHandler ph;
    
    




    private void Start()
    {
        eo = GetComponent<EventObserver>();



        eo.OnDamage += UpdateHealthMeter;

    }



    #region Health meter
    [Header("Health elements")]
    public GameObject healthDisplay;
    public Text healthCounter;
    public Image healthBar;
    public Color healthNormalColour;
    public Color healthCriticalColour;

    public UnityEvent onHealthDamage;
    public UnityEvent onHealthHeal;
    public UnityEvent onHealthCritical;
    public UnityEvent onHealthFine;

    int prevHealthValue;

    public void UpdateHealthMeter(DamageMessage dm)
    {
        // Check if the player had their health changed. If not, end function without doing anything.
        if (ph != dm.victim)
        {
            return;
        }
        
        int current = ph.ph.health.current;
        int max = ph.ph.health.max;

        healthCounter.text = current.ToString();
        FillMeter(healthBar, current, max);

        if (current < prevHealthValue)
        {
            onHealthDamage.Invoke();
        }
        else if (current > prevHealthValue)
        {
            onHealthHeal.Invoke();
        }
        prevHealthValue = current;

        if (ph.ph.health.IsCritical())
        {
            onHealthCritical.Invoke();
        }
        else
        {
            onHealthFine.Invoke();
        }

        

        
    }
    #endregion



    public UnityEvent onEnemyDamage;
    public UnityEvent onCriticalHit;
    public UnityEvent onKill;


    public void PlayHitMarkerEffects(DamageMessage dm)
    {

    }








    void FillMeter(Image i, float current, float max)
    {
        i.fillAmount = current / max;
    }


    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
