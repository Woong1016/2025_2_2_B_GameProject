using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{

    [Header("�ǹ� ����")]

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
    private DeliveryOrderSystem orderSystem;

    void Start()
    {
        SetupBuilding();
        orderSystem = FindAnyObjectByType<DeliveryOrderSystem>();
        CreateNameTag();

    }
    void HandleDriverService (DeliveryDriver driver)
    {
        switch (BuildingType)
        {

            case BuildingType.Restaurant:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                    //Debug.Log($"{buildingname}���� ������ �Ⱦ��߽��ϴ�");
                }
                break;

            case BuildingType.Coustomer:
                if (orderSystem != null)
                {

                    orderSystem.OnDriverEnteredCustomer(this);
                   //Debug.Log($"{buildingname}���� ��� �Ϸ�");
                   //driver.CompleteDelivery();
                }
                else
                {
                    driver.CompleteDelivery();  
                }

                    break;

            case BuildingType.ChargingStation:
               //Debug.Log($"{buildingname}���� ���͸��� �����߽��ϴ�");
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

    void CreateNameTag()
    {
        GameObject nameTag = new GameObject("NameTag");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;


        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingname;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<BildBoard>();
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();

        if(renderer != null)
        {
            Material mat = renderer.material;
            switch (BuildingType)
            {

                case BuildingType.Restaurant:
                    mat.color = Color.red;
                   // buildingname = "������";
                    break;

                case BuildingType.Coustomer:
                    mat .color = Color.green;
                   // buildingname = "�� ��";
                    break;

                case BuildingType.ChargingStation:  
                    mat .color = Color.yellow   ;
                   // buildingname = "������";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }  
    }
}
