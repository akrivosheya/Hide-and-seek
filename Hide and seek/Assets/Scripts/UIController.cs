using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text information;

    private string _howToHide = "Press E to Hide";
    private string _prepareToSeek = "Ready for seek!";
    private string _howToSeek = "To check object use mouse";
    private string _prepareToHide = "Everybody is found!";

    void Awake()
    {
        Messenger.AddListener(GameEvent.ReadyToSeek, OnReadyToSeek);
        Messenger.AddListener(GameEvent.StartedSeekMode, OnStartedSeekMode);
        Messenger.AddListener(GameEvent.ReadyToHide, OnReadyToHide);
        Messenger.AddListener(GameEvent.StartedHideMode, OnStartedHideMode);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ReadyToSeek, OnReadyToSeek);
        Messenger.RemoveListener(GameEvent.StartedSeekMode, OnStartedSeekMode);
        Messenger.RemoveListener(GameEvent.ReadyToHide, OnReadyToHide);
        Messenger.RemoveListener(GameEvent.StartedHideMode, OnStartedHideMode);
    }

    void Start()
    {
        information.text = _howToHide;
    }

    public void OnReadyToSeek()
    {
        information.text = _prepareToSeek;
    }

    public void OnStartedSeekMode()
    {
        information.text = _howToSeek;
    }

    public void OnReadyToHide()
    {
        information.text = _prepareToHide;
    }

    public void OnStartedHideMode()
    {
        information.text = _howToHide;
    }
}
