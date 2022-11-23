using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject FigurePrefab;
    [SerializeField] private Material HiderMaterial;
    [SerializeField] private Vector3 HiderPosition = new Vector3(20f, 1.9f, 0f);
    [SerializeField] private Vector3 SeekerPosition = new Vector3(-20f, 1.9f, 0f);
    [SerializeField] private int Textures = 3;
    //[SerializeField] private int FiguresCount = 12;
    [SerializeField] private int FiguresInRow = 3;
    [SerializeField] private int FiguresInColumn = 4;
    [SerializeField] private float MinMagnitudeBetweenPlayersAndFigures = 5f;
    [SerializeField] private float SceneLength = 50f;

    private GameObject _hider = null;
    private GameObject _seeker = null;
    private List<GameObject> _figures = new List<GameObject>();
    private System.DateTime boundaryTime;
    private bool _hasToCountTime = false;
    private bool _canShake = true;
    private int SecondsBeforeShakingHider = 10;
    private int MillisecondsShakingHider = 500;
    private float ShakingSpeed = 3f;

    void Start()
    {
        SetFigures();
    }

    void Update()
    {
        if(_hasToCountTime)
        {
            var interval = boundaryTime - System.DateTime.Now;
            Managers.Session.Data.RemainingTime = interval.Seconds;
            if(interval.Seconds == 0)
            {
                _hasToCountTime = false;
                Managers.Session.StartHideMode();
            }
            else if (_canShake && interval.Seconds % SecondsBeforeShakingHider == 0)
            {
                _canShake = false;
                StartCoroutine(ShakeHider());
            }
        }
    }

    public void StartSeekMode()
    {
        _hider.GetComponent<PlayerMove>().CanHide = false;
        int textureIndex = Random.Range(0, Textures);
        _hider.GetComponent<PlayerMove>().HideClientRpc(textureIndex);
        StartCoroutine(InvokeSeekers());
    }

    public void StartHideMode()
    {
        /*Managers.Session.StartHideMode();
        SceneManager.LoadScene(sceneName);*/
        _hasToCountTime = false;
        _hider.GetComponent<PlayerMove>().IsMoving = false;
        _hider.GetComponent<PlayerMove>().SetMainTextureClientRpc();
        _seeker.GetComponent<PlayerMove>().IsMoving = false;
        StartCoroutine(LoadNextLevel());
    }

    public void AddPlayer(GameObject player)
    {
        if(_seeker == null)
        {
            player.transform.position = SeekerPosition;
            player.AddComponent<Seeker>();
            player.GetComponent<PlayerMove>().SetRoleClientRpc((int)PlayerRoles.Seeker);
            _seeker = player;
        }
        else if(_hider == null)
        {
            player.transform.position = HiderPosition;
            player.AddComponent<Hider>();
            player.GetComponent<PlayerMove>().SetRoleClientRpc((int)PlayerRoles.Hider);
            _hider = player;
            Managers.Session.Data.SeekerAttempts = Managers.Session.MaxSeekerAttempts;
            Managers.Session.Data.RemainingTime = Managers.Session.MaxTime;
            _hider.GetComponent<PlayerMove>().IsMoving = true;
            _hider.GetComponent<PlayerMove>().CanHide = true;
        }
    }

    private IEnumerator InvokeSeekers()
    {
        yield return new WaitForSeconds(2f);

        _seeker.GetComponent<PlayerMove>().IsMoving = true;
        boundaryTime = System.DateTime.Now + new System.TimeSpan(0, 0, Managers.Session.MaxTime);
        _hasToCountTime = true;
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);
        Managers.Session.Data.SeekerAttempts = Managers.Session.MaxSeekerAttempts;
        Managers.Session.Data.RemainingTime = Managers.Session.MaxTime;

        _seeker.transform.position = SeekerPosition;
        _hider.transform.position = HiderPosition;
        _hider.GetComponent<PlayerMove>().IsMoving = true;
        _hider.GetComponent<PlayerMove>().CanHide = true;

        foreach(GameObject figure in _figures)
        {
            if(figure != null)
            {
                figure.GetComponent<NetworkObject>().Despawn();
            }
        }

        _figures.Clear();
        SetFigures();
    }

    private IEnumerator ShakeHider()
    {
        var stopShakingTime = System.DateTime.Now + new System.TimeSpan(0, 0, 0, 0, MillisecondsShakingHider);
        int i = 0;
        while((stopShakingTime - System.DateTime.Now).Milliseconds > 0)
        {
            yield return null;
            Vector3 movement = new Vector3((float)System.Math.Sin((stopShakingTime - System.DateTime.Now).Milliseconds),
                                            0, (float)System.Math.Cos((stopShakingTime - System.DateTime.Now).Milliseconds));
            movement = movement * Time.deltaTime * ShakingSpeed;
            _hider.GetComponent<PlayerMove>().Move(movement);
            ++i;
        }
        _canShake = true;
    }

    private void SetFigures()
    {
        for(int i = 0; i < FiguresInRow; ++i)
        {
            for(int j = 0; j < FiguresInColumn; ++j)
            {
                GameObject figure = Instantiate(FigurePrefab);
                figure.gameObject.GetComponent<NetworkObject>().Spawn();
                Vector3 figurePosition = new Vector3(0, 0, 0);
                do
                {
                    figurePosition = new Vector3(Random.Range(i * SceneLength / FiguresInRow - SceneLength / 2, (i + 1) * SceneLength / FiguresInRow - SceneLength / 2),
                                                            1.9f,
                                                            Random.Range(j * SceneLength / FiguresInColumn - SceneLength / 2, (j + 1) * SceneLength / FiguresInColumn - SceneLength / 2));
                } while((HiderPosition - figurePosition).magnitude < MinMagnitudeBetweenPlayersAndFigures || (SeekerPosition - figurePosition).magnitude < MinMagnitudeBetweenPlayersAndFigures);
                figure.transform.position = figurePosition;
                figure.GetComponent<DespawningObject>().SetTextureClientRpc(Random.Range(0, Textures));
                _figures.Add(figure);
            }
        }
    }
}
