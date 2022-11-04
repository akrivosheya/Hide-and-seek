using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    private bool _isSeekMode;

    public void Startup()
    {
        Debug.Log("Session manager starting...");

        _isSeekMode = false;

        status = ManagerStatus.Started;
    }

    public void StartSeekMode()
    {
        _isSeekMode = true;
    }

    public void StartHideMode()
    {
        _isSeekMode = false;
    }

    public bool IsSeekMode()
    {
        return _isSeekMode;
    }
}
