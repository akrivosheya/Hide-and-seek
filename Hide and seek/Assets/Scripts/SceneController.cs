using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Hider hider;
    [SerializeField] private Seeker seeker;
    [SerializeField] private FollowCamera cam;

    [SerializeField] private string sceneName = "HideAndSeekScene";

    void Start()
    {
        seeker.gameObject.SetActive(false);
        hider.gameObject.SetActive(true);
        cam.target = hider.transform;
        Messenger.Broadcast(GameEvent.StartedHideMode);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartSeekMode()
    {
        Managers.Session.StartSeekMode();
        hider.StopMoving();
        seeker.gameObject.SetActive(true);
        cam.target = seeker.transform;
        Messenger.Broadcast(GameEvent.StartedSeekMode);
    }

    public void StartHideMode()
    {
        Managers.Session.StartHideMode();
        SceneManager.LoadScene(sceneName);
    }
}
