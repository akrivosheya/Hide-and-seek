using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DespawningObject : NetworkBehaviour
{
    public int TextureIndex { get => _textureIndex.Value; set => _textureIndex.Value = value; }

    private NetworkVariable<int> _textureIndex = new NetworkVariable<int>();
    public override void OnNetworkSpawn()
    {
        if(!IsHost && IsClient)
        {
            GetComponent<MeshRenderer>().material = Resources.Load("Materials/Figure" + TextureIndex) as Material;
        }
    }

    [ClientRpc]
    public void SetTextureClientRpc(int textureIndex)
    {
        TextureIndex = textureIndex;
        GetComponent<MeshRenderer>().material = Resources.Load("Materials/Figure" + textureIndex) as Material;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisappearServerRpc()
    {
        Managers.Session.OnSeekerFailed();
        GetComponent<NetworkObject>().Despawn();
    }
}
