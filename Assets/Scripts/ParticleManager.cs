using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ParticleManager : NetworkBehaviour
{
   [SerializeField] private float seconds = 5f;

    private void Start()
    {
        StartCoroutine(DestroyAfterDelay(seconds));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RequestDespawn();
    }

    void RequestDespawn()
    {
        if(!IsOwner) return;
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            SubmitDespawnRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitDespawnRequestServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }

}
