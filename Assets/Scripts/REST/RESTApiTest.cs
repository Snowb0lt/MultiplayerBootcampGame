using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RESTApiTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputUserName;
    [SerializeField] private TMP_InputField _inputUserPassword;
    [SerializeField] private TMP_InputField _inputUserScore;

    [SerializeField] private TMP_Text _userNameTxt;
    [SerializeField] private GameObject _registerPanel;
    [SerializeField] private Transform _scoreBoard;
    [SerializeField] private GameObject _highScoreElement;

    [SerializeField] private GameObject _buttonRegister;
    [SerializeField] private GameObject _buttonLogin;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestAPI());
    }
    public void RegisterUserCall()
    {
        StartCoroutine(RegisterUser(_inputUserName.text, _inputUserPassword.text));
        _buttonRegister.SetActive(false);
    }

    public void LoginUserCall()
    {
        StartCoroutine(LoginUser(_inputUserName.text, _inputUserPassword.text));
        _buttonRegister.SetActive(false);
        _buttonLogin.SetActive(false);
    }

    public void SubmitScoreCall()
    {
        StartCoroutine(SubmitScore(int.Parse(_inputUserScore.text)));
    }

    public void UpdateScoreboardCall()
    {
        StartCoroutine(UpdateScoreboard());
    }

    public void LogOut()
    {
        //Delete the token
        PlayerPrefs.DeleteKey("TOKEN");

        //Show Login UI
        _registerPanel.SetActive(true);
        _buttonRegister.SetActive(true);
        _buttonLogin.SetActive(true);
    }
    IEnumerator TestAPI()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com/");

        yield return webRequest.SendWebRequest();

        Debug.Log($"Response Code: {webRequest.responseCode}");
        Debug.Log($"Response Errors: {webRequest.error}");
        Debug.Log($" {webRequest.downloadHandler.text}");

    }

    IEnumerator RegisterUser(string username, string password)
    {
        User user = new User();
        user.username = username;
        user.password = password;

        string dataToUpload = JsonUtility.ToJson(user);

        UnityWebRequest registerUserRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/register", dataToUpload, "application/json");

        yield return registerUserRequest.SendWebRequest();

        Debug.Log($"Response code is: {registerUserRequest.responseCode}");
        Debug.Log($"Response error is: {registerUserRequest.error}");
        Debug.Log($" {registerUserRequest.downloadHandler.text}");

        //If there are no errors, login the user
        StartCoroutine(LoginUser(username,password));

    }

    IEnumerator LoginUser(string username, string password)
    {
        User user = new User();
        user.username = username;
        user.password = password;

        string dataToUpload = JsonUtility.ToJson(user);

        UnityWebRequest loginUserRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/login", dataToUpload, "application/json");

        yield return loginUserRequest.SendWebRequest();

        Debug.Log($"Response code is: {loginUserRequest.responseCode}");
        Debug.Log($"Response error is: {loginUserRequest.error}");
        Debug.Log($" {loginUserRequest.downloadHandler.text}");

        //If the login is successful, save the token to player prefs
        Login loginData = JsonUtility.FromJson<Login>(loginUserRequest.downloadHandler.text);

        PlayerPrefs.SetString("TOKEN", loginData.token);
        _userNameTxt.text = username;

        //Hide Login UI
        _registerPanel.SetActive(false);
    }

    IEnumerator SubmitScore(int score)
    {
        //Create Score Object
        Score scoreData = new Score();
        scoreData.score = score;

        string dataToUpload = JsonUtility.ToJson(scoreData);

        UnityWebRequest submitScoreRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/submit-score", dataToUpload, "application/json");

        //Add the auth header
        submitScoreRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("TOKEN"));

        yield return submitScoreRequest.SendWebRequest();

        Debug.Log($"Response code is: {submitScoreRequest.responseCode}");
        Debug.Log($"Response error is: {submitScoreRequest.error}");
        Debug.Log($" {submitScoreRequest.downloadHandler.text}");

        
        //Update the highscore UI
        UpdateScoreboardCall();
    }

    IEnumerator UpdateScoreboard()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com/top-scores");

        yield return webRequest.SendWebRequest();

        Debug.Log($"Response code is: {webRequest.responseCode}");
        Debug.Log($"Response error is: {webRequest.error}");
        Debug.Log($" {webRequest.downloadHandler.text}");

        //Use escape characters to create a JSON using an array response
        string highScores = $"{{\"highScores\": {webRequest.downloadHandler.text}}}";

        HighScore topScores = JsonUtility.FromJson<HighScore>(highScores);

        //Iterate through the array and update the list of highScores
        for (int i = 0; i < topScores.highScores.Length; i++)
        {
            Transform highScoreElement;
            if (i < _scoreBoard.childCount)
            {
                highScoreElement = _scoreBoard.GetChild(i);
            }
            else
            {
                highScoreElement = Instantiate(_highScoreElement, _scoreBoard).transform;
            }

            //Update the element data
            highScoreElement.GetChild(0).GetComponent<TMP_Text>().text = (i+1).ToString();   //Rank
            highScoreElement.GetChild(1).GetComponent<TMP_Text>().text = topScores.highScores[i].username; //Username  
            highScoreElement.GetChild(2).GetComponent<TMP_Text>().text = topScores.highScores[i].highScore.ToString(); //HighScore  
        }
    }
}
