using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject BaseLayer;
    public GameObject JoinGameLayer;
    public GameObject HostingGameLayer;
    public GameObject SettingLayer;

    String joinCode = "";
    public TextMeshProUGUI PlayerInput;
    public String sceneName = "Level 1";
    public Slider musicSlider;
    public Slider soundSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JoinGameLayer.SetActive(false);
        HostingGameLayer.SetActive(false);
        SettingLayer.SetActive(false);
        if(!PlayerPrefs.HasKey("MusicVolume")){
            Debug.Log("No key found, generating defaults");
            PlayerPrefs.SetFloat("MusicVolume", 1f);
            PlayerPrefs.SetFloat("EffectVolume", 1f);
        }else{
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            soundSlider.value = PlayerPrefs.GetFloat("EffectVolume");
        }

        musicSlider.onValueChanged.AddListener((value) => UpdateSoundLevels());
        soundSlider.onValueChanged.AddListener((value) => UpdateSoundLevels());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSoundLevels(){
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", soundSlider.value);
        PlayerPrefs.Save();
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
        if(JoinGameLayer.activeSelf){
            JoinGameLayer.SetActive(false);
            BaseLayer.SetActive(true);
        }else{
            JoinGameLayer.SetActive(true);
            BaseLayer.SetActive(false);
        }
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
        if(SettingLayer.activeSelf){
            SettingLayer.SetActive(false);
            BaseLayer.SetActive(true);
        }else{
            SettingLayer.SetActive(true);
            BaseLayer.SetActive(false);
        }    
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
