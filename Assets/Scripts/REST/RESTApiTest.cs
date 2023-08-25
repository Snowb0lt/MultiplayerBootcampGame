using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RESTApiTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestAPI());
    }

    IEnumerator TestAPI()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com/");

        yield return webRequest.SendWebRequest();

        Debug.Log($"Response Code: {webRequest.responseCode}");
        Debug.Log($"Response Errors: {webRequest.error}");
        Debug.Log($" {webRequest.downloadHandler.text}");

    }
}
