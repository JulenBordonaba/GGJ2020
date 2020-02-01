using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public float populationChange;
    public float resourcesChange;
    public int happinessChange;
    public float economyIndexChange;
    public float locuraChange;

    public void ExecuteOrder()
    {
        GameManager.current.resourceGrowth += economyIndexChange;
        GameManager.current.happiness += happinessChange;
        GameManager.current.population += populationChange;
        GameManager.current.resources += resourcesChange;
        GameManager.current.locura += locuraChange;
        GameManager.current.ordersToNewspaper -= 1;
        if(GameManager.current.ordersToNewspaper<=0)
        {
            GameManager.current.ThrowEvent();
            GameManager.current.ordersToNewspaper = Random.Range(2, 4);
        }
    }
}
