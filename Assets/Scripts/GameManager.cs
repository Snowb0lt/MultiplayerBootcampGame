using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkObject _localPlayer;

    public NetworkVariable<short> _state = new NetworkVariable<short>(
    0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server

    );
    void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    Dictionary<ulong, Color> _colors = new Dictionary<ulong, Color>()
    {

    };

    Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();

    Dictionary<ulong, int> playerScores = new Dictionary<ulong, int>();

    [SerializeField] private Transform[] startPositions;

    [Header("UI Elements")]

    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private TMP_Text _scoreUI;

    [SerializeField] private GameObject _endGameScreen;
    [SerializeField] private TMP_Text _endGameMessage;
    private void Awake()
    {
        Singleton();

        if(IsServer)
        {
            _state.Value = 0;
        }
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

    //Will be called by Player Info of each Player in Game

    public void OnPlayerJoined(NetworkObject playerObject)
    {
        //Assign Start Position to Player
        playerObject.transform.position = startPositions[(int)playerObject.OwnerClientId].position;
        playerScores.Add(playerObject.OwnerClientId, 0);
    }

    public void StartGame()
    {
        _state.Value = 1;
        ShowScoreUI();
    }

    public void SetPlayerName(NetworkObject playerObject, string name)
    {
        if(playerNames.ContainsKey(playerObject.OwnerClientId))
        {
            playerNames[playerObject.OwnerClientId] = name;
        }
        else
        {
            playerNames.Add(playerObject.OwnerClientId, name);
        }
    }

    //Change Player Colour

    public void SetPlayerColour(NetworkObject playerObject, Color colour)
    {
        if (_colors.ContainsKey(playerObject.OwnerClientId))
        {
            _colors[playerObject.OwnerClientId] = colour;
        }
        else
        {
            _colors.Add(playerObject.OwnerClientId, colour);
        }
    }

    //If the bullet hits, we will call the server to do this. 

    public void AddScore(ulong playerID)
    {
        if (IsServer)
        {
            playerScores[playerID]++;
            ShowScoreUI();
            CheckWinner(playerID);
        }
    }
    public void ShowScoreUI()
    {
        _scoreUI.text = "";

        PlayerScores _scores = new PlayerScores();
        _scores._scores = new List<ScoreInfo>();
        foreach (var item in playerScores)
        {
            ScoreInfo temp = new ScoreInfo();
            temp.score = item.Value;
            temp.id = item.Key;
            temp.name = playerNames[item.Key];
            _scores._scores.Add(temp);

            _scoreUI.text += $"[{item.Key}] {playerNames[item.Key]}: {item.Value}\n";
        }

        //Update the client
        UpdateClientScoreClientRPC(JsonUtility.ToJson(_scores));
    }
    [ClientRpc]
    public void UpdateClientScoreClientRPC(string scoreInfo)
    {
        PlayerScores _scores = JsonUtility.FromJson<PlayerScores>(scoreInfo);
        _scoreUI.text = "";

        foreach (var item in _scores._scores)
        {
            _scoreUI.text += $"[{item.id}] {item.name}: {item.score}\n";
        }
    }
    void CheckWinner(ulong playerID)
    {
        if (playerScores[playerID] >= 5)
        {
            EndGame(playerID);
        }
    }

    public void EndGame(ulong winnerID)
    {
        if (IsServer)
        {
            //Show Win/Lose UI
            _endGameScreen.SetActive(true);
            if(winnerID == NetworkManager.LocalClientId)
            {
                _endGameMessage.text = "YOU WIN";
            }
            else
            {
                _endGameMessage.text = $"YOU LOSE!\nThe winner is {playerNames[winnerID]}";
            }

            ScoreInfo temp = new ScoreInfo();
            temp.score = playerScores[winnerID];
            temp.id = winnerID;
            temp.name = playerNames[winnerID];

            ShowGameEndUIClientRPC(JsonUtility.ToJson(temp));
        }
    }

    [ClientRpc]
    public void ShowGameEndUIClientRPC(string winnerInfo)
    {
        _endGameScreen.SetActive(true);
        ScoreInfo info = JsonUtility.FromJson<ScoreInfo>(winnerInfo);

        if(info.id == NetworkManager.LocalClientId)
        {
            _endGameMessage.text = "YOU WIN!";
        }
        else
        {
            _endGameMessage.text = $"YOU LOSE!\nThe winner is {info.name}";
        }
    }
    public void ResetPlayerPosition(NetworkObject playerObject, ulong playerId)
    {

        playerObject.transform.position = startPositions[(int)playerId].position;
    }


}
[System.Serializable]
public class PlayerScores
{
    public List<ScoreInfo> _scores;
}

[System.Serializable]
public class ScoreInfo
{
    public ulong id;
    public string name;
    public int score;
}