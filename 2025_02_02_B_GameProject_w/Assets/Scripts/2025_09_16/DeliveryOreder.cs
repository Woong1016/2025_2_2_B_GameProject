using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

// 간단한 배달 주문
[System.Serializable]

public class DeliveryOreder 
{
    public int orderld;
    public string restaurantName;
    public string customerName;
    public Building restaurantBuilding;
    public Building customerBuilding;

    public float orderTime;
    public float timeLimit;
    public float reward;
    public OrederState state;

    public DeliveryOreder(int id , Building restaurant , Building customer , float rewardAmount)

    {
        orderld = id;
        restaurantBuilding = restaurant;
        customerBuilding = customer;
        restaurantName = restaurant.buildingname;
        customerName = customer.buildingname;

        orderTime = Time.time;
        timeLimit = Random.Range(60f, 120f);
        reward = rewardAmount;
        state = OrederState.WaitingPickup;

        
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, timeLimit - (Time.time - orderTime));
    }

    public bool IsExpired ()
    {
        return GetRemainingTime() <= 0f;
    }


    

}
