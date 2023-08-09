using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _statusTxt;
    
    public void JoinHost()
    {
        NetworkManager.Singleton.StartHost();
        _statusTxt.text = "Joined as Host";
    }

    public void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
        _statusTxt.text = "Joined as Client";
    }

    public void JoinServer()
    {
        NetworkManager.Singleton.StartServer();
        _statusTxt.text = "Joined as Server";
    }
}
