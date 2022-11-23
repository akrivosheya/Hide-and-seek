using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Clickable : MonoBehaviour
{

    void OnMouseDown()
    {
        Operate();
    }

    public virtual void Operate()
    {
    }
}
