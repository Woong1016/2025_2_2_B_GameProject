using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryOrderSystem : MonoBehaviour
{

    [Header("주문설정")]
    public float ordergenrateInterval = 15f;
    public int maxActiveOrders = 8;

    [Header("게임 상태")]

    public int totalOrdersGenerated = 0;
    public int completedOrders = 0;
    public int expiredOrders = 0;


    private List<DeliveryOreder> currentOrders = new List<DeliveryOreder>();

    private List<Building> restaruants = new List<Building>();
    private List<Building> customers = new List<Building>();

    [System.Serializable]
    public class OrderSystemEvents
    {
        public UnityEvent<DeliveryOreder> OnNewOrderAdded;
        public UnityEvent<DeliveryOreder> OnOrderPickedUp;
        public UnityEvent<DeliveryOreder> OnOrderCompleted;
        public UnityEvent<DeliveryOreder> OnOrderExpired;

    }

    public OrderSystemEvents orderEvents;
    private DeliveryDriver driver;


    void Start()
    {

        driver = FindObjectOfType<DeliveryDriver>();
        FindAllBuilding();

        StartCoroutine(GenerateInialOrders());

        StartCoroutine(orderGenerator());

        StartCoroutine(ExpiredOrderChecker());

    }
    void FindAllBuilding()
    {
        Building[] allBuildings = FindObjectsOfType<Building>();

        foreach (Building building in allBuildings)
        {
            if (building.BuildingType == BuildingType.Restaurant)
            {
                restaruants.Add(building);

            }
            else if (building.BuildingType == BuildingType.Coustomer)
            {
                customers.Add(building);
            }
        }
        Debug.Log($"음식점 {restaruants.Count}개 , 고객 {customers.Count}개 발견");
    }


    void CreatNewOrder()
    {
        if (restaruants.Count == 0 || customers.Count == 0) return;

        Building randomRestaurant = restaruants[Random.Range(0, restaruants.Count)];
        Building randomCustomer = customers[Random.Range(0, customers.Count)];


        if (randomRestaurant == randomCustomer)
        {
            randomCustomer = customers[Random.Range(0, customers.Count)];
        }

        float reward = Random.Range(3000f, 8000f);

        DeliveryOreder newOrder = new DeliveryOreder(++totalOrdersGenerated, randomRestaurant, randomCustomer, reward);

        currentOrders.Add(newOrder);
        orderEvents.OnNewOrderAdded?.Invoke(newOrder);



    }

    void PickupOrder(DeliveryOreder order)
    {
        order.state = OrederState.PickedUp;
        orderEvents.OnOrderPickedUp?.Invoke(order);

    }

    void CompleteOrder(DeliveryOreder order)
    {
        order.state = OrederState.Completed;
        completedOrders++;

        if (driver != null)
        {
            driver.AddMoney(order.reward);
        }

        currentOrders.Remove(order);
        orderEvents.OnOrderCompleted?.Invoke(order);
    }

    void ExpiredOrder(DeliveryOreder order)
    {

        order.state = OrederState.Expired;
        expiredOrders++;

        currentOrders.Remove(order);
        orderEvents.OnOrderExpired?.Invoke(order);
    }



    public List<DeliveryOreder> GetDeliveryOreders()
    {
        return new List<DeliveryOreder>(currentOrders);
    }


    public int GetPickWaitingCount()
    {
        int count = 0;

        foreach (DeliveryOreder order in currentOrders)
        {
            if (order.state == OrederState.WaitingPickup) count++;
        }
        return count;
    }


    public int GetDeliveryWaitingCount()
    {
        int count = 0;
        foreach (DeliveryOreder order in currentOrders)
        {
            if (order.state == OrederState.PickedUp) count++;
        }
        return count;
    }

    DeliveryOreder FindOrderForPickup(Building restaurant)
    {
        foreach (DeliveryOreder order in currentOrders)
        {
            if (order.restaurantBuilding == restaurant && order.state == OrederState.WaitingPickup)
            {
                return order;
            }
        }
        return null;
    }

    DeliveryOreder FindOrderForDelivery(Building customer)
    {
        foreach (DeliveryOreder order in currentOrders)
        {
            if (order.customerBuilding == customer && order.state == OrederState.PickedUp)
            {
                return order;
            }
        }
        return null;
    }

    public void OnDriverEnteredRestaurant(Building restaurant)
    {
        DeliveryOreder orderToPickup = FindOrderForPickup(restaurant);

        if (orderToPickup != null)
        {
            PickupOrder(orderToPickup);
        }

    }

    public void OnDriverEnteredCustomer(Building customer)
    {

        DeliveryOreder orderToDeliver = FindOrderForDelivery(customer);

         if(orderToDeliver != null)
        {
            CompleteOrder(orderToDeliver);
        }
    }
    IEnumerator GenerateInialOrders()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            CreatNewOrder();
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator orderGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(ordergenrateInterval);

            if(currentOrders.Count < maxActiveOrders)
            {
                CreatNewOrder();
            }
        }
    }

    IEnumerator ExpiredOrderChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            List<DeliveryOreder> expiredOrders = new List<DeliveryOreder>();

            foreach (DeliveryOreder order in currentOrders)
            {
                if(order.IsExpired()&& order.state != OrederState.Completed )

                {
                    expiredOrders.Add(order);
                }
            }
            foreach(DeliveryOreder expired in expiredOrders)
            {
                ExpiredOrder(expired);
            }

        }
            
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 ,10 , 400 , 1300));

        GUILayout.Label("===배달 주문 ===");
        GUILayout.Label($"활성 주문 : {currentOrders.Count}개");
        GUILayout.Label($"픽업대기 : {GetPickWaitingCount()}개");
        GUILayout.Label($"배달 대기: {GetDeliveryWaitingCount()}개");
        GUILayout.Label($"완료 : {completedOrders}개 | 만료 : {expiredOrders}");

        GUILayout.Space(10);

        foreach(DeliveryOreder order in currentOrders)
        {

            string status = order.state == OrederState.WaitingPickup ? "픽업 대기 " : "배달대기";
            float timeLeft = order .GetRemainingTime();

            GUILayout.Label($"#{order.orderld}:{order.restaurantName} -> {order.customerName}");
            GUILayout.Label($"{status}| {timeLeft:F0} 초 남음");


        }
        GUILayout.EndArea();




    }



}


