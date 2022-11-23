using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Figure : Clickable
{
    private DespawningObject _despawning;

    void Start()
    {
        _despawning = GetComponent<DespawningObject>();
    }

    public override void Operate()
    {
        if(Managers.Session.Data.IsSeekMode && Managers.Session.CanFind)
        {
            _despawning.DisappearServerRpc();
        }
    }
}
