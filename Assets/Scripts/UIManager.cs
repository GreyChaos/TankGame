using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        DisconnectAndReturnToMainMenu();
    }
}

    private void DisconnectAndReturnToMainMenu()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // Clean up the network session
            NetworkManager.Singleton.Shutdown();
        }

        // Wait a frame before loading the scene to ensure shutdown completes
        StartCoroutine(LoadMainMenuScene());
    }

    private IEnumerator LoadMainMenuScene()
    {
        yield return new WaitForEndOfFrame(); // Let Shutdown fully complete
        SceneManager.LoadScene("MainMenuScene");
    }


}
