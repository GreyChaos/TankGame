using Unity.Netcode;
using UnityEngine;

public class Shell : NetworkBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public LayerMask collisionLayer;

    void Update()
    {
        if (IsOwner)
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            SubmitPositionRequestServerRpc(transform.position);
        }

        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, 0.02f, collisionLayer);
        if (hitCollider != null)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                player.IncrementHitCount(damage);
            }

            RequestDespawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Player hit");
                player.IncrementHitCount(damage);
            }

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

    [ServerRpc(RequireOwnership = false)]
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
