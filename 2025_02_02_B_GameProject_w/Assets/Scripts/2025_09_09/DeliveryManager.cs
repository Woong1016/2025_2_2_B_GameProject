using System.Collections;
using System.Collections.Generic;

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
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped .AddListener(OnMoveStoped);
            driver.driverEvents.OnLowBattery.AddListener(OnlowBattery);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);

        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (statusText != null && driver != null)
        {

            statusText.text = driver.GetStatusText();
        }
    }

   private void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAgterDelay(2f));
        }


    }

   private IEnumerator ClearMessageAgterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    private void UpdateMoney(float money)
    {

        ShowMessage($"�� : {money} �� ", Color.green);

    }

    private void UpdateBattery(float battery)
    {
        if (batterySlider != null)
        {
            batterySlider.value = battery / 100f;

        }

        if (batteryFill != null)
        {
            if (battery > 50f)
            {
                batteryFill.color = Color.green;
            }
            else if (battery < 20f)
            {
                batteryFill.color = Color.yellow;
            }
            else
                batteryFill.color = Color.red;
        }


    }

    private void UpdateDeliveryCount(int count)
    {
        ShowMessage($"��޿Ϸ� : {count} �� ", Color.blue);
    }

    private void OnMoveStarted()
    {
        ShowMessage($"�̵����� ", Color.cyan);

    }
    private void OnMoveStoped()
    {
        ShowMessage($"�̵�����", Color.gray);
    }

    private void OnlowBattery()
    {
        ShowMessage($"���͸� ���� ", Color.magenta);
    }
    private void OnBatteryEmpty()
    {
        ShowMessage($"���͸� ���� ", Color.red);
    }

    private void OnDeliveryCompleted()
    {
        
        
       ShowMessage($"��� �Ϸ� ", Color.green);
        
    }

    private void UpdateUI()
    {
        if(driver != null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }

       
    }

    private void OnDestroy()
    {

        if (driver != null)
        {
            driver.driverEvents.OnMoneyChanged.RemoveListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.RemoveListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.RemoveListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.RemoveListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.RemoveListener(OnMoveStoped);
            driver.driverEvents.OnLowBattery.RemoveListener(OnlowBattery);
            driver.driverEvents.OnLowBatteryEmpty.RemoveListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.RemoveListener(OnDeliveryCompleted);
        }
        
    }
    

}
