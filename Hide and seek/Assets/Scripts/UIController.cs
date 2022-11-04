using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text information;
    [SerializeField] private GameObject networkButtons;

    private const string HowToHide = "Press E to Hide";
    private const string PrepareToSeek = "Ready for seek!";
    private const string HowToSeek = "To check object use mouse";
    private const string PrepareToHide = "Everybody is found!";

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
        information.text = HowToHide;
    }

    public void OnReadyToSeek()
    {
        information.text = PrepareToSeek;
    }

    public void OnStartedSeekMode()
    {
        information.text = HowToSeek;
    }

    public void OnReadyToHide()
    {
        information.text = PrepareToHide;
    }

    public void OnStartedHideMode()
    {
        information.text = HowToHide;
    }

    public void OnStartedHost(){
        Managers.Network.StartHost();
        networkButtons.SetActive(false);
    }

    public void OnStartedClient(){
        Managers.Network.StartClient();
        networkButtons.SetActive(false);
    }
}
