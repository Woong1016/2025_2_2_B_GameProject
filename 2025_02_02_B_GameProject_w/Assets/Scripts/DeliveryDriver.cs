using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
public class DeliveryDriver : MonoBehaviour
{

    [Header("배달원 설정")]

    public float moveSpeed = 8f;
    public float rotationSpeed = 10.0f;


    [Header("상태")]
    public float currentMoney = 0f;
    public float batteryLevel = 100f;
    public int deliveryCount = 0;


    [System.Serializable]

    public class DriverEvents
    {

        [Header("이동 Event")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnMoveStoped;

        [Header("상태변화 Event")]
        public UnityEvent<float> OnMoneyChaged;
        public UnityEvent<float> OnBatteryChanged;
        public UnityEvent<int> OnDeliveryCountChanged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBatteryEmpty;
        public UnityEvent OnDeliveryCompleted;
    }

    public DriverEvents driverevent;

    public bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        driverevent.OnMoneyChaged?.Invoke(currentMoney);
        driverevent.OnBatteryChanged?.Invoke(batteryLevel);
        driverevent.OnDeliveryCountChanged?.Invoke(deliveryCount);


    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        UpdateBattery();





    }


    void HandleMovement()
    {
        if (batteryLevel <= 0)
        {

            if (isMoving)
            {

            }
            return;

        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

        if (moveDirection.magnitude > 0.1f)
        {

            if(!isMoving)
            {
                StartMoving();
            }


            moveDirection = moveDirection.normalized;

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            if(moveDirection != Vector3.zero)
            {
                Quaternion targetRoatation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRoatation, rotationSpeed * Time.deltaTime);


            }
            ChangedBattery(-Time.deltaTime * 3.0f);
        }
        else
        {


            if (isMoving)
            {
                StopMoving();
            }

        }
    }


    void ChangedBattery(float amout)
    {
        float oldBattery = batteryLevel;

        batteryLevel += amout;
        batteryLevel = Mathf.Clamp(batteryLevel, 0, 100);

        driverevent.OnBatteryChanged?.Invoke(batteryLevel);


        if(oldBattery >20f && batteryLevel <= 20f)
        {
            driverevent.OnLowBattery?.Invoke();
        }
        if (oldBattery > 0f && batteryLevel <= 0f)
        {
            driverevent.OnLowBatteryEmpty?.Invoke(); 
        }

    }
    void StartMoving()
    {
        isMoving=true;
        driverevent.OnMoveStarted?.Invoke();
    }


    void StopMoving()
    {
        isMoving= false;
        driverevent?.OnMoveStoped?.Invoke();
    }



    void UpdateBattery()
    {
        if(batteryLevel > 0 )
        {

            ChangedBattery(-Time.deltaTime * 0.5f);
        }
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverevent.OnMoneyChaged?.Invoke(currentMoney);

    }

    public void CompleteDelivery()

    {

        deliveryCount++;
        float reward = Random.Range(3000,3000);

        AddMoney(reward);
        driverevent.OnDeliveryCountChanged?.Invoke(deliveryCount);
        driverevent.OnDeliveryCompleted?.Invoke();

    }

    public void ChargeBattery()
    {
        ChangedBattery(100f - batteryLevel);

    }

    public string GetStatusText()
    {
        return $"돈  : {currentMoney:F0}  원 | 배터리 : {batteryLevel:F1}% | 배달 {deliveryCount} 건 ";


    }

    public bool CanMove()
    {
        return batteryLevel > 0;
    }


}



