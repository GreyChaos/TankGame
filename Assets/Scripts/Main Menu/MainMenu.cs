using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject BaseLayer;
    public GameObject JoinGameLayer;
    String joinCode = "";
    public TextMeshProUGUI PlayerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JoinGameLayer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HostGame(){
        SceneManager.sceneLoaded += OnGameSceneLoadedHost;
        SceneManager.LoadScene("GameScene");
    }

    public void JoinGame(){
        JoinGameLayer.SetActive(true);
        BaseLayer.SetActive(false);
        
    }

    public void SubmitCode(){
        joinCode = PlayerInput.text;
        PlayerPrefs.SetString("JoinCode", joinCode);
        SceneManager.sceneLoaded += OnGameSceneLoadedClient;
        JoinGameLayer.SetActive(false);
        BaseLayer.SetActive(true);
        SceneManager.LoadScene("GameScene");
    }

    public void Settings(){
        Debug.Log("Setting");
    }

    public void ExitGame(){
        Application.Quit();
    }

    private void OnGameSceneLoadedHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            NetworkManagerHUD networkManagerHUD = GameObject.FindAnyObjectByType<NetworkManagerHUD>();
            if (networkManagerHUD != null)
            {
                networkManagerHUD.StartHost();  // Call the host game method
            }
        }

        SceneManager.sceneLoaded -= OnGameSceneLoadedHost;  // Unsubscribe to prevent duplicate calls
    }

    private void OnGameSceneLoadedClient(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            NetworkManagerHUD networkManagerHUD = GameObject.FindAnyObjectByType<NetworkManagerHUD>();
            if (networkManagerHUD != null)
            {
                networkManagerHUD.StartClient();  // Call the host game method
            }
        }

        SceneManager.sceneLoaded -= OnGameSceneLoadedClient;  // Unsubscribe to prevent duplicate calls
    }
}
