using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text information;
    [SerializeField] private Text instruction;
    [SerializeField] private Text sessionData;
    [SerializeField] private Text seekerPoints;
    [SerializeField] private Text hiderPoints;
    [SerializeField] private GameObject networkButtons;
    [SerializeField] private GameObject sceneControllerPrefab;

    private const string HowToHide = "Press E to Hide";
    private const string WaitHiders = "Wait while hiders are hiding";
    private const string BeQuite = "Try to not move.\nBut you will shake every 5 seconds";
    private const string HideMode = "Hide Mode";
    private const string SeekMode = "Seek Mode";
    private const string PrepareToSeek = "Ready for seek!";
    private const string HowToSeek = "To check object use mouse";
    private const string SeekerWon = "Hider is found!";
    private const string SeekerLosed = "Seeker losed!";
    private const string Empty = "";

    void Awake()
    {
        Messenger.AddListener(GameEvent.ReadyToSeek, OnReadyToSeek);
        Messenger.AddListener(GameEvent.StartedSeekMode, OnStartedSeekMode);
        Messenger.AddListener(GameEvent.ReadyToHide, OnReadyToHide);
        Messenger.AddListener(GameEvent.StartedHideMode, OnStartedHideMode);
        Messenger.AddListener(GameEvent.SessionDataChanged, OnSessionDataChanged);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ReadyToSeek, OnReadyToSeek);
        Messenger.RemoveListener(GameEvent.StartedSeekMode, OnStartedSeekMode);
        Messenger.RemoveListener(GameEvent.ReadyToHide, OnReadyToHide);
        Messenger.RemoveListener(GameEvent.StartedHideMode, OnStartedHideMode);
        Messenger.RemoveListener(GameEvent.SessionDataChanged, OnSessionDataChanged);
    }

    void Start()
    {
        //OnStartedHideMode();
    }

    public void OnReadyToSeek()
    {
        information.text = PrepareToSeek;
        instruction.text = Empty;
    }

    public void OnStartedSeekMode()
    {
        information.text = SeekMode;
        if(!Managers.Network.IsServer)
        {
            instruction.text = BeQuite;
        }
        else
        {
            instruction.text = HowToSeek;
        }
    }

    public void OnReadyToHide()
    {
        instruction.text = Empty;
        if(Managers.Session.Data.SeekerAttempts == 0 || Managers.Session.Data.RemainingTime == 0)
        {
            information.text = SeekerLosed;
        }
        else
        {
            information.text = SeekerWon;
        }
    }

    public void OnStartedHideMode()
    {
        information.text = HideMode;
        if(!Managers.Network.IsServer)
        {
            instruction.text = HowToHide;
        }
        else
        {
            instruction.text = WaitHiders;
        }
    }

    public void OnStartedHost()
    {
        Instantiate(sceneControllerPrefab);//Лучше как-то по-другому
        Managers.Network.StartHost();
        networkButtons.SetActive(false);
        OnSessionDataChanged();
    }

    public void OnStartedClient()
    {
        Managers.Network.StartClient();
        networkButtons.SetActive(false);
        OnSessionDataChanged();
    }

    public void OnSessionDataChanged()
    {
        sessionData.text = "Attempts: " + Managers.Session.Data.SeekerAttempts + "\nTime: " + Managers.Session.Data.RemainingTime;
        seekerPoints.text = Managers.Session.Data.SeekerPoints.ToString();
        hiderPoints.text = Managers.Session.Data.HiderPoints.ToString();
    }
}
