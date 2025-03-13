using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class APIManager : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";
    string Token;
    string Username;

    AuthResponse response;
    public Credentials credentials = new Credentials();

    [SerializeField] private GameObject menuAuth;
    [SerializeField] private GameObject menuGame;

    [SerializeField] private GameObject usernameTextField;
    [SerializeField] private GameObject passwordTextField;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake() {
        usernameTextField = GameObject.Find("UsernameInputField");
        passwordTextField = GameObject.Find("PasswordInputField");

        errorText.text = "";
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        credentials.username = Username;
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Username))
        {
            StartCoroutine(ShowText("No hay token", 3));
            Debug.Log("No hay Token");
        }
        else
        {
            StartCoroutine(GetPerfil());
        }
    }

    public void Register()
    {
        //crear el post data y llamar coroutine
        credentials.username = usernameTextField.GetComponent<TMP_InputField>().text;
        credentials.password = passwordTextField.GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);

        Debug.Log(postData);

        StartCoroutine(RegisterPost(postData));

        usernameTextField.GetComponent<TMP_InputField>().text = "";
        passwordTextField.GetComponent<TMP_InputField>().text = "";
    }
    public void Login()
    {
        //crear el post data y llamar coroutine
        credentials.username = GameObject.Find("UsernameInputField").GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);  

        Debug.Log(postData);

        StartCoroutine(LoginPost(postData));
    }

    IEnumerator RegisterPost(string postData)
 {
     string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            
            Debug.Log(www.error);
            StartCoroutine(ShowText(www.error, 3));
        }
        else
        {
            if (www.responseCode == 200)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                string mensaje = "status: " + www.responseCode;
                mensaje += "\nError: " + www.downloadHandler.text;
                Debug.Log(mensaje);
                StartCoroutine(ShowText(mensaje, 3));
            }
        }
 }

 IEnumerator LoginPost(string postData)
 {
     string path = "/api/auth/login";
     UnityWebRequest www = UnityWebRequest.Put(url+path, postData);
     www.method = "POST";
     www.SetRequestHeader("Content-Type", "application/json");
     yield return www.SendWebRequest();

     if (www.result == UnityWebRequest.Result.ConnectionError)
     {
         Debug.Log(www.error);
     }
     else
     {
         if (www.responseCode == 200)
         {
             string json = www.downloadHandler.text;

             AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);


             menuAuth.SetActive(false);
             menuGame.SetActive(true);

             PlayerPrefs.SetString("token", response.token);
             PlayerPrefs.SetString("username", response.usuario.username);
             Token = response.token;
         }
         else
         {
             string mensaje = "status:" + www.responseCode;
             mensaje += "\nError: " + www.downloadHandler.text;
             Debug.Log(mensaje);
         }
     }
 }

 IEnumerator GetPerfil()
 {
     string path = "/api/usuarios";
     UnityWebRequest www = UnityWebRequest.Get(url + path);
     www.SetRequestHeader("x-token", Token);
     yield return www.SendWebRequest();

     if (www.result == UnityWebRequest.Result.ConnectionError)
     {
         Debug.Log(www.error);
     }
     else
     {
         if (www.responseCode == 200)
         {
             string json = www.downloadHandler.text;
             response = JsonUtility.FromJson<AuthResponse>(json);
             Debug.Log(response);
             GameObject.Find("PanelAuth").SetActive(false);
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }

 IEnumerator ShowText(string text, int duration)
    {
        errorText.text = text;
        yield return new WaitForSeconds(duration);
        errorText.text = "";
        
    }

    public class Credentials
{
    public string username;
    public string password;
}

[System.Serializable]
public class AuthResponse
{
    public UserModel usuario;
    public string token;
}

[System.Serializable]
public class UserModel
{
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;
}

[System.Serializable]
public class UsersList
{
    public UserModel[] usuarios;
}

[System.Serializable]
public class DataUser
{
    public int score;
}
public class UpdateScore
{
    public string username;
    public DataUser data;
}
}



