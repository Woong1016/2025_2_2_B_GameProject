using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureItem : MonoBehaviour
{
    public int goldAmount = 100;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"���� ȹ�� ! ��� + {goldAmount}");
            Destroy(gameObject);
        }
    }
}
