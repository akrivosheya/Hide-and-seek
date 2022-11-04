using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SessionManager))]

public class Managers : MonoBehaviour
{
    public static SessionManager Session { get; private set; }
    public static NetworkManager Network { get => NetworkManager.Singleton; }

    private List<IGameManager> _startSequence;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Session = GetComponent<SessionManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Session);

        StartCoroutine(StartupManager());
    }

    private IEnumerator StartupManager()
    {
        foreach(IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while(numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach(IGameManager manager in _startSequence)
            {
                if(manager.status == ManagerStatus.Started)
                {
                    ++numReady;
                }
            }

            if(numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
                //Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }

            yield return null;
        }
        
        Debug.Log("All managers started up");
        //Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }
}
