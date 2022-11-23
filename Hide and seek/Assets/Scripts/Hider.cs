using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Hider : Clickable
{
    [SerializeField] private Texture[] textures;

    private PlayerMove _player;
    private bool _canHide;

    void Start()
    {
        _player = GetComponent<PlayerMove>();
        GetComponent<MeshRenderer>().material = Resources.Load("Materials/HiderMaterial") as Material;
    }

    void Update()
    {
        if(_player.CanHide && Input.GetKeyDown(KeyCode.E))
        {
            _player.HideServerRpc();
        }
    }

    public void StopMoving()
    {
        _player.CanHide = false;
        _player.IsMoving = false;
    }
    
    public override void Operate()
    {
        if(Managers.Session.Data.IsSeekMode && Managers.Session.CanFind)
        {
            _player.FindServerRpc();
        }
    }
}
