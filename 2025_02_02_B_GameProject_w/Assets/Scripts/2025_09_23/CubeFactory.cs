using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class CubeFactory : MonoBehaviour
{

    [Header("�����հ� ��ġ")]
    public GameObject cubePrefab;
    public Transform queuePoint; 
    public Transform woodStorage;
    public Transform metalStorage;
    public Transform assemblyArea;

    private Queue<GameObject>meterialQueue = new Queue<GameObject>();
    private Stack<GameObject> woodWarehouse= new Stack<GameObject>();
    private Stack<GameObject>metalWarehouse = new Stack<GameObject>();
    private Stack<string> assemblyStack = new Stack<string>();
    private List<WorkRequest> requestList = new List<WorkRequest>();
    private Dictionary<ProductType , int >products = new Dictionary<ProductType , int>();



    public int money = 500;
    public int score = 0;

    private float lastMaterialTime;
    private float lastOrderTime;


    // Start is called before the first frame update
    void Start()
    {
        products[ProductType.Chair] = 0;
        assemblyStack.Push("����");
        assemblyStack.Push("����");
        assemblyStack.Push("�غ�");
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateVisual();
        AutoEvent();
    }
    void AddMaterial()
    {
        ResourcesType randomType = (Random.value > 0.5f) ? ResourcesType.Wood : ResourcesType.Metal;

        GameObject newCube = Instantiate(cubePrefab);
        ResourcesCube cubeComponent = newCube .AddComponent<ResourcesCube>();
        cubeComponent.initalize(randomType);

        meterialQueue.Enqueue(newCube);



        Debug.Log($"{randomType} ���� ���� ,ť ��� :{meterialQueue.Count}");




    }

    void ProcessQueue()
    {
        if(meterialQueue.Count ==0)
        {
            Debug.Log("ť�� ����ֽ��ϴ�");

            return; 
        }

        //ť���� ���Ḧ ������ (���Լ���)

        GameObject cube = meterialQueue.Dequeue();
        ResourcesCube resource = cube.GetComponent<ResourcesCube>();

        if(resource.Type == ResourcesType.Wood)
        {
            woodWarehouse.Push(cube);
            Debug.Log($"���� â�� �԰� ! â�� : {woodWarehouse.Count}��");
        }
        else if(resource.Type == ResourcesType.Metal)
        {
            metalWarehouse.Push(cube);
            Debug.Log($"�ݼ� â�� �԰� ! â�� : {metalWarehouse.Count}��");
        }

    }


    void ProcessAssmebly()
    {
        if(woodWarehouse.Count == 0 || metalWarehouse.Count == 0)
        {

            Debug.Log("������ ��ᰡ �����մϴ�");
            return;


        }


        if(assemblyStack.Count == 0)
        {
            Debug.Log("���� �۾��� �����");

            return;
        }
        // ���ÿ��� �۾��� ������
        string work = assemblyStack.Pop();

        GameObject wood = woodWarehouse.Pop();
        GameObject metal = metalWarehouse.Pop();
        Destroy(wood);
        Destroy(metal);

        // ��� �۾� �Ϸ�� ��ǰ ����

        if(assemblyStack.Count == 0)
        {
            products[ProductType.Chair]++;
            score += 100;

            assemblyStack.Push("����");
            assemblyStack.Push("����");
            assemblyStack.Push("�غ�");

            Debug.Log($"���� �ϼ��ߴ粲!! ������ : {products[ProductType.Chair]}��");
        }


    }
    void AddRequest()
    {
        int quantity = Random.Range(1, 4);
        int reward = quantity * 200;

        WorkRequest newRequest = new WorkRequest(ProductType.Chair, quantity, reward);

        requestList.Add(newRequest);

        Debug.Log("���Բ� �� �ֹ� ���Դ� ���ض� �뿹���");
    }
    void ProcessRequest()
    {
        if(requestList.Count == 0)
        {
            Debug.Log("ó���� ��û���� ����.. �츮 ������");
            return;
        }

        WorkRequest firestRequest = requestList[0];

        if (products[firestRequest.ProductType] >= firestRequest.quantily) 

        {
            products[firestRequest.ProductType] -= firestRequest.quantily;
            money += firestRequest.reward;
            score += firestRequest.reward;

            requestList.RemoveAt(0);
        }

        else
        {
            int available = products[firestRequest.ProductType];
            int needed = firestRequest.quantily - available;
            Debug.Log($"��� ���� ! {needed} ��  ���ʿ� �� || ����: {available}");
        }
    }

    void UpdateVisual()
    {
        UpdateQueueVisual();
        UpdateWarehouseVisual(); 
    }

    void UpdateWarehouseVisual()
    {
        UpdateStackVisual(woodWarehouse.ToArray(), woodStorage);
        UpdateStackVisual(metalWarehouse.ToArray(), metalStorage);


    }

    void UpdateQueueVisual()
    {
        if (queuePoint == null) return;

        GameObject[] queueArray = meterialQueue.ToArray();
        for (int i = 0; i < queueArray.Length; i++)
        {
            Vector3 position = queuePoint.position + Vector3.right * (i * 1.2f);
            queueArray[i].transform.position = position;    
        }


        

    }

    
    void UpdateStackVisual(GameObject[] stackArray, Transform basePoint)
    {
        if (basePoint == null) return;
        for (int i = 0; i < stackArray.Length; i++)
        {
            Vector3 position = basePoint.position + Vector3.up * (i * 1.1f);
            stackArray[stackArray.Length - 1 - i ] . transform.position = position;
        }
    }

    private void OnGUI()
    {
        // ���� ����

        GUI.Label(new Rect(10, 10, 200, 20), $"�� : {money}�� | ���� : {score}��");

        GUI.Label(new Rect(10, 40, 250, 20), $"���� ť (Queue): {meterialQueue.Count}�� ��� ");
        GUI.Label(new Rect(10, 60, 250, 20), $"���� â�� (Stack): {woodWarehouse.Count}�� ��� ");
        GUI.Label(new Rect(10, 80, 250, 20), $"�ݼ� â�� (Stack): {metalWarehouse.Count}�� ��� ");
        GUI.Label(new Rect(10, 100, 250, 20), $"���� ���� (Stack): {assemblyStack.Count}�� ��� ");
        GUI.Label(new Rect(10, 120, 250, 20), $"����ǰ (Dict): {products[ProductType.Chair]}�� ��� ");
        GUI.Label(new Rect(10, 140, 250, 20), $"��û�� (List): {requestList.Count}�� ��� ");

        GUI.Label(new Rect(10, 170, 200, 20), "===��û�� ��� ===");
        for (int i = 0; i < requestList.Count && i < 3; i++)
        {
            WorkRequest request = requestList[i];
            GUI.Label(new Rect(10, 190 + i * 20, 300, 20),
                $"{i}���� {request.quantily}�� -> {request.reward}��");
        }

        GUI.Label(new Rect(300, 40, 150, 20), " ===���۹�===");
        GUI.Label(new Rect(300, 60, 150, 20), " 1Ű : ���� ť �߰�");
        GUI.Label(new Rect(300, 80, 150, 20), " QŰ : ť -> â��");
        GUI.Label(new Rect(300, 100, 150, 20), " AŰ : ���� (����)");
        GUI.Label(new Rect(300, 120, 150, 20), " SŰ : ��û ó��");
        GUI.Label(new Rect(300, 140, 150, 20), " RŰ : ��û�� �߰�");

    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddMaterial();
        if (Input.GetKeyDown(KeyCode.Q)) ProcessQueue();
        if (Input.GetKeyDown(KeyCode.A)) ProcessAssmebly();
        if (Input.GetKeyDown(KeyCode.S  )) ProcessRequest();
        if (Input.GetKeyDown(KeyCode.R)) AddRequest();
    }

    void AutoEvent()
    {
        if(Time.time - lastMaterialTime > 3f)
        {
            AddMaterial();
            lastMaterialTime = Time.time;   

              
        }

        if(Time.time - lastMaterialTime > 10f)
        {
            AddRequest(); 
            lastOrderTime = Time.time;
        }
    }
}
