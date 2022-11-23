using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SessionData : NetworkBehaviour
{
    public bool IsSeekMode { get => _isSeekMode.Value; set => _isSeekMode.Value = value; }
    public int SeekerAttempts { get => _seekerAttempts.Value; set => _seekerAttempts.Value = value; }
    public int RemainingTime { get => _remainingTime.Value; set => _remainingTime.Value = value; }
    public int SeekerPoints { get => _seekerPoints.Value; set => _seekerPoints.Value = value; }
    public int HiderPoints { get => _hiderPoints.Value; set => _hiderPoints.Value = value; }

    private NetworkVariable<bool> _isSeekMode = new NetworkVariable<bool>();
    private NetworkVariable<int> _seekerAttempts = new NetworkVariable<int>();
    private NetworkVariable<int> _remainingTime = new NetworkVariable<int>();
    private NetworkVariable<int> _seekerPoints = new NetworkVariable<int>();
    private NetworkVariable<int> _hiderPoints = new NetworkVariable<int>();
    
    public override void OnNetworkSpawn()
    {
        _isSeekMode.OnValueChanged += OnModeChanged;
        _seekerAttempts.OnValueChanged += OnAttemptsChanged;
        _remainingTime.OnValueChanged += OnTimeChanged;
        _seekerPoints.OnValueChanged += OnPointsChanged;
        _hiderPoints.OnValueChanged += OnPointsChanged;
        if(IsServer)
        {
            SeekerAttempts = Managers.Session.MaxSeekerAttempts;
            RemainingTime = Managers.Session.MaxTime;
            SeekerPoints = 0;
            HiderPoints = 0;
        }
        if(!IsHost && IsClient)
        {
            Managers.Session.Data = this;
        }
    }

    public void OnModeChanged(bool wasSeekMode, bool isSeekMode)
    {
        if(wasSeekMode != isSeekMode)
        {
            if(isSeekMode)
            {
                Messenger.Broadcast(GameEvent.ReadyToSeek);
            }
            else
            {
                Messenger.Broadcast(GameEvent.ReadyToHide);
            }
        }
    }

    public void OnAttemptsChanged(int previousAttempts, int currentAttempts)
    {
        if(previousAttempts != currentAttempts)
        {
            Messenger.Broadcast(GameEvent.SessionDataChanged);
        }
    }

    public void OnTimeChanged(int previousTime, int currentTime)
    {
        if(previousTime != currentTime)
        {
            Messenger.Broadcast(GameEvent.SessionDataChanged);
        }
    }

    public void OnPointsChanged(int previousPoints, int currentPoints)
    {
        if(previousPoints != currentPoints)
        {
            Messenger.Broadcast(GameEvent.SessionDataChanged);
        }
    }
}
