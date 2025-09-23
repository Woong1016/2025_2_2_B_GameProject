using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesCube : MonoBehaviour
{
    public ResourcesType Type;
    public void initalize(ResourcesType resourcesType)
    {
        Type = resourcesType;
        Renderer renderer = GetComponent<Renderer>();

        if (resourcesType == ResourcesType.Wood) renderer.material.color = new Color(0.6f, 0.3f, 0.1f); //갈색
        if (resourcesType == ResourcesType.Metal) renderer.material.color = Color.gray;     //회색
    }
}
