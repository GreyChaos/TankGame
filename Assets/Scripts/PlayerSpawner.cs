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
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Instantiate the player on the server
        GameObject player = Instantiate(playerPrefab, GetSpawnPosition(), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        // Send the color to clients
        SetPlayerColorClientRpc(player.GetComponent<NetworkObject>(), randomColor);
    }

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
    }

    [ClientRpc]
    private void SetPlayerColorClientRpc(NetworkObjectReference playerObject, Color color)
    {
        if (playerObject.TryGet(out NetworkObject player))
        {
            player.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
