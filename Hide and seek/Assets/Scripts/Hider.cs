using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hider : Clickable
{
    [SerializeField] private SceneController controller;
    [SerializeField] private Texture mainTexture;
    [SerializeField] private Texture[] textures;

    private bool _canHide;

    void Start()
    {
        _canHide = true;
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mainTexture;
    }

    void Update()
    {
        if(_canHide && Input.GetKeyDown(KeyCode.E)){
            StartCoroutine(Hide());
        }
    }

    public void StopMoving()
    {
        _canHide = false;
        GetComponent<PlayerMove>().IsMoving = false;
    }
    
    public override void Operate()
    {
        if(Managers.Session.IsSeekMode())
        {
            StartCoroutine(Lose());
        }
    }

    private IEnumerator Hide()
    {
        int index = Random.Range(0, textures.Length);
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = textures[index];
        Messenger.Broadcast(GameEvent.ReadyToSeek);

        yield return new WaitForSeconds(2f);

        controller.StartSeekMode();
    }

    private IEnumerator Lose()
    {
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mainTexture;
        Messenger.Broadcast(GameEvent.ReadyToHide);

        yield return new WaitForSeconds(2f);

        controller.StartHideMode();
    }
}
