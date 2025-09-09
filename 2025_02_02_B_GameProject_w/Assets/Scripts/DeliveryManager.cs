using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
public class DeliveryManager : MonoBehaviour
{


    [Header("UI��� ")]
    public Text statusText;
    public Text messageText;
    public Slider batterySlider;
    public Image batteryFill;

    [Header("���ӿ�����Ʈ ")]

    public DeliveryDriver driver;
    // Start is called before the first frame update
    void Start()
    {
        if (driver != null)
        {
            driver.driverevent.OnMoneyChaged.AddListener(UpdateMoney);
            driver.driverevent.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverevent.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
            driver.driverevent.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverevent.OnMoveStoped .AddListener(OnMoveStoped);
            driver.driverevent.OnLowBattery.AddListener(OnlowBatteryEmpty);
            driver.driverevent.OnLowBatteryEmpty.AddListener(OnlowBatteryEmpty);
            driver.driverevent.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowMessage(string message, Color color)
    {
        if (message != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAgterDelay(21));
        }


    }

    IEnumerator ClearMessageAgterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    void UpdateMoney(float money)
    {

        ShowMessage($"�� : {money}�� ", Color.green);

    }

    void UpdateBattery(float battery)
    {
        if (batterySlider != null)
        {
            batterySlider.value = battery / 100f;

        }

        if (battery != null)
        {
            if (battery > 50f)
            {
                batteryFill.color = Color.green;

            }
            else if (battery < 20f)
                batteryFill.color = Color.red;

            else
                batteryFill.color = Color.yellow;
        }


    }

    void UpdateDeliveryCount(int count)
    {
        ShowMessage($"��޿Ϸ� : {count} �� ", Color.blue);
    }

    void OnMoveStarted()
    {
        ShowMessage("�̵����� ", Color.cyan);

    }
    void OnMoveStoped()
    {
        ShowMessage("�̵�����", Color.gray);
    }

    void OnlowBatteryEmpty()
    {
        ShowMessage("���͸� ���� ", Color.magenta);
    }
    void OnBatteryEmpty()
    {
        ShowMessage("���͸� ���� ", Color.red);
    }

    void OnDeliveryCompleted()
    {
        
        
       ShowMessage("��� �Ϸ� ", Color.black);
        
    }

     void UpdateUI()
    {
        if(driver != null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }

        driver.driverevent.OnMoneyChaged.RemoveListener(UpdateMoney);
        driver.driverevent.OnBatteryChanged.RemoveListener(UpdateBattery);
        driver.driverevent.OnDeliveryCountChanged.RemoveListener(UpdateDeliveryCount);
        driver.driverevent.OnMoveStarted.RemoveListener(OnMoveStarted);
        driver.driverevent.OnMoveStoped.RemoveListener(OnMoveStoped);
        driver.driverevent.OnLowBattery.RemoveListener(OnlowBatteryEmpty);
        driver.driverevent.OnLowBatteryEmpty.RemoveListener(OnlowBatteryEmpty);
        driver.driverevent.OnDeliveryCompleted.RemoveListener(OnDeliveryCompleted);
    }



}
