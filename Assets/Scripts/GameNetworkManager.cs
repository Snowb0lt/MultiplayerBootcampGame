using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

using TMPro;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private int _maxConnections = 4;

    [Header("Connection to UI")]

    [SerializeField] private TMP_Text _statusTxt;
    [SerializeField] private GameObject _btnClient;
    [SerializeField] private GameObject _btnHost;
    [SerializeField] private TMP_Text _playerIDText;
    [SerializeField] private TMP_InputField _joinCodeText;

    private string _playerID;
    private bool _clientAuthenticated = false;
    private string _joinCode;

    private async void Start()
    {
        await AuthenticatePlayer();
    }
    async Task AuthenticatePlayer()
    {
        try
        {
            await UnityServices.InitializeAsync(); //Initalize Any Asynchronous Task
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Sign in the user anonymously
            _playerID = AuthenticationService.Instance.PlayerId;
            _clientAuthenticated = true;
            _playerIDText.text = $"Client Authentication Successful - {_playerID}";
        }
        catch (Exception e)
        {

            Debug.Log(e);
        }
    }
    public async Task<RelayServerData> AllocateRelayServerAndGetCode(int maxConnections, string region = null)
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.Log($"Relay allocation request failed - {e}");
            throw;
        }

        Debug.Log($"Server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Server: {allocation.AllocationId}");

        try
        {
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to create a join code - {e}");
            throw;
        }

        return new RelayServerData(allocation, "dtls");
    }


    public async Task<RelayServerData> JoinRelayServerWithCode(string joinCode)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log($"Relay allocation join request failed - {e}");
            throw;
        }

        Debug.Log($"Client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"Client: {allocation.AllocationId}");

        return new RelayServerData(allocation, "dtls");
    }

    IEnumerator ConfigureGetCodeAndJoinHost()
    {
        //Run the task to get code and join as host
        var allocateAndGetCode = AllocateRelayServerAndGetCode(_maxConnections);

        while(!allocateAndGetCode.IsCompleted)
        {
            //Wait until we create the allocation and get code
            yield return null;
        }

        //Show allocation error if allocation and code is not gotten
        if (allocateAndGetCode.IsFaulted)
        {
            Debug.LogError($"Cannot start the server due to an exception {allocateAndGetCode.Exception.Message}");
            yield break;
        }

        var relayServerData = allocateAndGetCode.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        //UI Updates
        _joinCodeText.gameObject.SetActive(true);
        _joinCodeText.text = _joinCode;
        _statusTxt.text = "Joined as Host";
    }

    IEnumerator ConfigureUseCodeAndJoinClient(string joinCode)
    {
        var joinAllocationFromCode = JoinRelayServerWithCode(joinCode);

        while (!joinAllocationFromCode.IsCompleted)
        {
            //Wait until we create the allocation and get code
            yield return null;
        }

        //Show allocation error if allocation and code is not gotten
        if (joinAllocationFromCode.IsFaulted)
        {
            Debug.LogError($"Cannot join the relay server due to an exception {joinAllocationFromCode.Exception.Message}");
            yield break;
        }

        var relayServerData = joinAllocationFromCode.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        //UI Updates
        _statusTxt.text = "Joined as Client";
    }
    public void JoinHost()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("Client not Authenticated. Please Try Again");
            return;
        }

        StartCoroutine(ConfigureGetCodeAndJoinHost());

        //UIUpdates
        _btnClient.gameObject.SetActive(false);
        _btnHost.gameObject.SetActive(false);
        _joinCodeText.gameObject.SetActive(false);


        //NetworkManager.Singleton.StartHost();
        //_statusTxt.text = "Joined as Host";
    }

    public void JoinClient()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("Client not Authenticated. Please Try Again");
            return;
        }

        if (_joinCodeText.text.Length <= 0)
        {
            Debug.Log($"Please Enter a Proper Join Code");
            _statusTxt.text = "Please Enter a Proper Join Code";
        }

        Debug.Log($"The join code is: {_joinCodeText.text}");
        StartCoroutine(ConfigureUseCodeAndJoinClient(_joinCodeText.text));

        //UIUpdates
        _btnClient.gameObject.SetActive(false);
        _btnHost.gameObject.SetActive(false);
        _joinCodeText.gameObject.SetActive(false);

        //NetworkManager.Singleton.StartClient();
        //_statusTxt.text = "Joined as Client";
    }

    public void JoinServer()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("Client not Authenticated. Please Try Again");
            return;
        }

        NetworkManager.Singleton.StartServer();
        _statusTxt.text = "Joined as Server";
    }
}
