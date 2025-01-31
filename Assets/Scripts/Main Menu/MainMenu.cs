using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject BaseLayer;
    public GameObject JoinGameLayer;
    public GameObject HostingGameLayer;
    String joinCode = "";
    public TextMeshProUGUI PlayerInput;
    public String sceneName = "Level 1";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JoinGameLayer.SetActive(false);
        HostingGameLayer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchLevel(String sceneName){
        this.sceneName = sceneName;
    }

    public void StartHostGame(){
        BaseLayer.SetActive(false);
        HostingGameLayer.SetActive(true);
    }

    public void HostGame(){
        SceneManager.sceneLoaded += OnGameSceneLoadedHost;
        SceneManager.LoadScene(sceneName);
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
        SceneManager.LoadScene(sceneName);
    }

    public void Settings(){
        Debug.Log("Setting");
    }

    public void ExitGame(){
        Application.Quit();
    }

    private void OnGameSceneLoadedHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneName)
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
        if (scene.name == sceneName)
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
