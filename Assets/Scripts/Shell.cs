using Unity.Netcode;
using UnityEngine;

public class Shell : NetworkBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public LayerMask collisionLayer;
    public NetworkVariable<Color> shellColor = new();
    private bool isMarkedForDespawn = false;

    public override void OnNetworkSpawn(){
        if (IsServer)
        {
            // The Server sets the color
            shellColor.Value = NetworkManager.Singleton
            .ConnectedClients[OwnerClientId]
            .PlayerObject.GetComponent<Player>()
            .playerColor.Value;
        }
        // Clients get the color.
        GetComponent<SpriteRenderer>().color = shellColor.Value;
    }

    void Update()
    {
        if (!IsOwner) return;
        if(isMarkedForDespawn) return;
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        SubmitPositionRequestServerRpc(transform.position);
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, 0.02f, collisionLayer);
        if (hitCollider != null)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                player.IncrementHitCount(damage, OwnerClientId);
            }
            isMarkedForDespawn = true;
            RequestDespawn();
        }
    }


    void RequestDespawn()
    {
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

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 position)
    {
        transform.position = position;
        SyncPositionClientRpc(position);
    }

    [ClientRpc]
    void SyncPositionClientRpc(Vector3 position)
    {
        if (!IsOwner)
        {
            transform.position = position;
        }
    }
}
