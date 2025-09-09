using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{


    [Header("�ǹ�")]

    public BuildingType BuildingType;
    public string buildingname = "�ǹ�";

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
                Debug.Log($"{buildingname}���� ������ �Ⱦ��߽��ϴ�");
                break;

            case BuildingType.Coustomer:
                Debug.Log($"{buildingname}���� ��� �Ϸ�");
                driver.CompleteDelivery();
                break;
            case BuildingType.ChargingStation:
                Debug.Log($"{buildingname}���� ���͸��� �����߽��ϴ�");
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
            Debug.Log($"{buildingname}�� �������ϴ�");
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
                    buildingname = "������";
                    break;

                case BuildingType.Coustomer:
                    mat .color = Color.green;
                    buildingname = "�� ��";
                    break;

                    case BuildingType.ChargingStation:  
                    mat .color = Color.yellow   ;
                    buildingname = "������";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }
    }
}
