using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    public bool IsPlayerObject = false;
    public float speed = 6.0f;
    public bool IsMoving { get => _isMoving.Value; set => _isMoving.Value = value; }
    public bool CanHide { get => IsOwner && _canHide.Value; set => _canHide.Value = value; }
    public int Role { get => _role.Value; set => _role.Value = value; }

    private NetworkVariable<bool> _isMoving = new NetworkVariable<bool>();
    private NetworkVariable<bool> _canHide = new NetworkVariable<bool>();
    private NetworkVariable<int> _role = new NetworkVariable<int>();
    private CharacterController _charController;
    private float _speedBoost = 2f;
    private float _hiderCameraHeight = 20f;

    public override void OnNetworkSpawn()
    {
        _isMoving.OnValueChanged += OnMovingChanged;
        if(IsServer)
        {
            Role = (int)PlayerRoles.NoRole;
            IsMoving = false;
            _charController = GetComponent<CharacterController>();
            transform.Rotate(90, 0, 0);
            Managers.Session.InstantiateData(gameObject);
        }
        else if(IsClient && !IsOwner && Role != (int)PlayerRoles.NoRole)
        {
            SetRole(Role);
        }
        if(IsOwner)
        {
            var camera = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
            camera.target = transform;
        }
    }

    void Update()
    {
        if(NetworkManager.Singleton.IsClient && IsOwner)
        {
            if(IsMoving)
            {
                MoveServerRpc(Input.GetKey(KeyCode.LeftShift), Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
        }
    }

    public void OnMovingChanged(bool wasMoving, bool isMoving)
    {
        if(wasMoving != isMoving)
        {
            if(isMoving)
            {
                if(Role == (int)PlayerRoles.Seeker)
                {
                    Messenger.Broadcast(GameEvent.StartedSeekMode);
                }
                else
                {
                    Messenger.Broadcast(GameEvent.StartedHideMode);
                }
            }
        }
    }

    public void Move(Vector3 movement)
    {
        _charController.Move(movement);
    }

    [ServerRpc]
    public void MoveServerRpc(bool isRunning, float horizontalAxis, float verticalAxis)
    {
        float fixedSpeed = isRunning ? speed * _speedBoost : speed;
        float deltaX = horizontalAxis * fixedSpeed;
        float deltaY = verticalAxis * fixedSpeed;
        Vector3 movement = new Vector3(deltaX, deltaY, 0);
        movement = Vector3.ClampMagnitude(movement, fixedSpeed) * Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }

    [ServerRpc]
    public void HideServerRpc()
    {
        if(!Managers.Session.Data.IsSeekMode)
        {
            Managers.Session.StartSeekMode();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FindServerRpc()
    {
        if(GetComponent<PlayerMove>() != null)
        {
            Managers.Session.StartHideMode();
        }
    }

    [ClientRpc]
    public void HideClientRpc(int textureIndex)
    {
        GetComponent<MeshRenderer>().material = Resources.Load("Materials/Figure" + textureIndex) as Material;
    }

    [ClientRpc]
    public void SetMainTextureClientRpc()
    {
        GetComponent<MeshRenderer>().material = Resources.Load("Materials/HiderMaterial") as Material;
    }

    [ClientRpc]
    public void SetRoleClientRpc(int role)
    {
        Role = role;
        if(!IsOwner)
        {
            return;
        }
        SetRole(role);
    }

    private void SetRole(int role)
    {
        if((int)PlayerRoles.Seeker == role )
        {
            if(gameObject.GetComponent<Seeker>() == null)
            {
                gameObject.AddComponent<Seeker>();
            }
            Managers.Session.CanFind = true;
        }
        else if((int)PlayerRoles.Hider == role )
        {
            if(gameObject.GetComponent<Hider>() == null)
            {
                gameObject.AddComponent<Hider>();
            }
            Managers.Session.CanFind = false;
            var camera = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
            Debug.Log(camera.transform.position + " " + _hiderCameraHeight);
            camera.transform.position = new Vector3(camera.transform.position.x, _hiderCameraHeight, camera.transform.position.z);
            Debug.Log(camera.transform.position);
        }
        Messenger.Broadcast(GameEvent.SessionDataChanged);
        Messenger.Broadcast(GameEvent.StartedHideMode);
    }
}
