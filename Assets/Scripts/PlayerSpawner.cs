using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {

        // Instantiate the player on the server
        GameObject player = Instantiate(playerPrefab, GetSpawnPosition(), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        player.GetComponent<Player>().playerScale.Value = player.transform.localScale;
    }

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
    }

}
