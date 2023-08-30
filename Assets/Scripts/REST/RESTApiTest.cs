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
        //TO DO: Call Login User Coroutine

    }
}
