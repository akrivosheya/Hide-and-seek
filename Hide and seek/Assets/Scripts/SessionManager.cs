using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SessionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public SessionData Data { get => _data; set { if(_data == null) _data = value; } }
    public bool CanFind { get; set; }
    public int MaxSeekerAttempts { get => _maxSeekerAttempts; }
    public int MaxTime { get => _maxTime; set => _maxTime = value; }

    [SerializeField] private SessionData DataPrefab;

    private SessionData _data = null;
    private SceneController _sceneController;
    private int _maxSeekerAttempts = 3;
    private int _maxTime = 50;

    public void Startup()
    {
        Debug.Log("Session manager starting...");

        status = ManagerStatus.Started;
    }

    public void StartSeekMode()
    {
        if(_sceneController != null)
        {
            Data.IsSeekMode = true;
            _sceneController.StartSeekMode();
        }
            /*CanHide = false;
            int textureIndex = Random.Range(0, Textures);
            HideClientRpc(textureIndex);*/
    }

    public void StartHideMode()
    {
        if(_sceneController != null)
        {
            if(Data.SeekerAttempts == 0 || Data.RemainingTime == 0)
            {
                Data.HiderPoints++;
            }
            else
            {
                Data.SeekerPoints++;
            }
            Data.IsSeekMode = false;
            _sceneController.StartHideMode();
        }
    }

    public void InstantiateData(GameObject player)
    {
        if(Data == null)
        {
            Data = Instantiate(DataPrefab) as SessionData;
            Data.gameObject.GetComponent<NetworkObject>().Spawn();
        }
        _sceneController = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
        _sceneController.AddPlayer(player);//??????????????????
    }

    public void OnSeekerFailed()
    {
        Data.SeekerAttempts--;
        if(Data.SeekerAttempts == 0)
        {
            StartHideMode();
        }
    }
}
