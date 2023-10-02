using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using System;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Services.Authentication;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text _txtPlayerName;
    [SerializeField] private MeshRenderer _playerMesh;
    [SerializeField] private GameObject _colourDropdown;

    //A variable to hold the name of the player
    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("PlayerName"),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        new Color(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    public void Awake()
    {
        _colourDropdown = GameObject.FindWithTag("Dropdown");
    }

    private void Start()
    {
        SetColor();
    }

    public void SetName(string name)
    {
        playerName.Value = new FixedString64Bytes(name);
    }
    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged; //Subscribe

        _txtPlayerName.SetText(playerName.Value.ToString());
        gameObject.name = "Player_" + playerName.Value.ToString();

        if (IsLocalPlayer)
        {
            GameManager.Instance.SetLocalPlayer(NetworkObject);
        }

        GameManager.Instance.OnPlayerJoined(NetworkObject);
    }
    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged; //Unsubscribe
    }

    private void OnNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        if (newValue != previousValue)
        {
            _txtPlayerName.SetText(newValue.Value);
            GameManager.Instance.SetPlayerName(NetworkObject, newValue.Value.ToString());  
        }
    }
    private void SetColor()
    {
        int val = TankColour._instance.colourSelectNumber;
        if (val == 0)
        {
            _playerMesh.material.color = Color.red;
        }
        if (val == 1)
        {
            _playerMesh.material.color = Color.blue;
        }
        if (val == 2)
        {
            _playerMesh.material.color = Color.green;
        }
        if (val == 3)
        {
            _playerMesh.material.color = Color.white;
        }
        if (val == 4)
        {
            _playerMesh.material.color = Color.black;
        }
        GameManager.Instance.SetPlayerColour(NetworkObject, _playerMesh.material.color);
    }
}
