using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Linq;

public class NetworkManagerHUD : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText;
    private string joinCode = "";  // Holds the relay join code for clients to enter

    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously to Unity services");
        }
    }

    public async void StartHost()
    {
        await InitializeUnityServices();
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);  // Up to 4 players
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.SetText("Join Code: " + joinCode);

            // Set relay data
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay error: {e.Message}");
        }
    }

    public async void StartClient()
    {
        await InitializeUnityServices();
        try
        {
            joinCode = PlayerPrefs.GetString("JoinCode");
            joinCode = new string(joinCode.Where(c => "6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw".Contains(c)).ToArray());
            joinCodeText.SetText("Join Code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Failed to join relay: {e.Message}");
        }
    }

    public void copyText(){
        GUIUtility.systemCopyBuffer = joinCode;
    }

}
