using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkRequest : MonoBehaviour
{


    public ProductType ProductType;
    public int quantily;
    public int reward;

    public WorkRequest(ProductType productType, int quantily, int reward)      // »ý¼ºÀÚ

    {
        this.ProductType = productType;
        this.quantily = quantily;
        this.reward = reward;

    }
}

