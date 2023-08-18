using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkObject _localPlayer;
    void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    [SerializeField] private TMP_InputField _playerNameField;
    private void Awake()
    {
        Singleton();
    }
    public void SetLocalPlayer(NetworkObject localPlayer)
    {
        _localPlayer = localPlayer;

        if(_playerNameField.text.Length > 0)
        {
            _localPlayer.GetComponent<PlayerInfo>().SetName(_playerNameField.text);
        }
        else
        {
            _localPlayer.GetComponent<PlayerInfo>().SetName($"Player-{_localPlayer.OwnerClientId}");
        }

        _playerNameField.gameObject.SetActive(false);
    }
}
