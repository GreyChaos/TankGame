using Unity.Netcode;
using UnityEngine;

public class NetworkManagerHUD : MonoBehaviour
{
    private void OnGUI()
    {
        // Add a try-catch to prevent layout errors from crashing the game
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));

        // Only show buttons if not already in the game
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host Game"))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("Join Game"))
            {
                NetworkManager.Singleton.StartClient();
            }
        }
        else
        {
            // If already in the game, show Disconnect button
            if (GUILayout.Button("Disconnect"))
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
        GUILayout.EndArea();
    }
}
