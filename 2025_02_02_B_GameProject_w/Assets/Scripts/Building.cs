using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{


    [Header("건물")]

    public BuildingType BuildingType;
    public string buildingname = "건물";

    [System.Serializable]

    public class BuildingEvents
    {

        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;
    }

    public BuildingEvents buildingEvents;

    void HandleDriverService (DeliveryDriver driver)
    {
        switch(BuildingType)
        {
            case BuildingType.Restaurant:
                Debug.Log($"{buildingname}에서 음식을 픽업했습니다");
                break;

            case BuildingType.Coustomer:
                Debug.Log($"{buildingname}에서 배달 완료");
                driver.CompleteDelivery();
                break;
            case BuildingType.ChargingStation:
                Debug.Log($"{buildingname}에서 배터리를 충전했습니다");
                driver.ChargeBattery();
                break;

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if(driver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(buildingname);
            HandleDriverService(driver);
        }
    }
    public void OnTriggerExit(Collider other)

    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(buildingname);
            Debug.Log($"{buildingname}을 떠났습니다");
        }

       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();

        if(renderer != null)
        {
            Material mat = renderer.material;
            switch (BuildingType)
            {

                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    buildingname = "음식점";
                    break;

                case BuildingType.Coustomer:
                    mat .color = Color.green;
                    buildingname = "고객 집";
                    break;

                    case BuildingType.ChargingStation:  
                    mat .color = Color.yellow   ;
                    buildingname = "충전소";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }
    }
}
