using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class AuthManager : MonoBehaviour
{

    string url = "https://sid-restapi.onrender.com";
    string Token;
    string Username;

    AuthResponse response;
    Vector3 position; 
    private List<GameObject> userInstances = new List<GameObject>();
    public Credentials credentials = new Credentials();
    [SerializeField] GameObject UserLeaderboardPrefab;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] private float verticalSpacing = 50f;
    [SerializeField] private GameObject mainMenuPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
    public void UpdateData(int score)
    {

        UpdateScore patchData = new UpdateScore();
        patchData.username = PlayerPrefs.GetString("username");
        DataUser userScore = new DataUser();
        userScore.score = score;
        patchData.data = userScore;
        string postData = JsonUtility.ToJson(patchData);
        Debug.Log(postData);
        StartCoroutine(UpdateDataPatch(postData));
    }
    public void Register()
    {
        
        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);
        
        StartCoroutine(RegisterPost(postData));
    }
    public void Login ()
    {
        credentials.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        credentials.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(credentials);  
        Debug.Log(postData);
        StartCoroutine(LoginPost(postData));
    }
    public void GetLeaderboard()
    {
        StartCoroutine(GetUsers());
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
                string json = www.downloadHandler.text;
                response = JsonUtility.FromJson<AuthResponse>(json);


                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.usuario.username);
                GameObject.Find("PanelAuth").SetActive(false);
                mainMenuPanel.SetActive(true);
                //StartCoroutine("GetUsers");
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
    IEnumerator UpdateDataPatch(string postData)
    {
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url + path, postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-token", Token);
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
                string json = www.downloadHandler.text;
                response = JsonUtility.FromJson<AuthResponse>(json);
                Debug.Log(response.usuario.username + response.usuario.data.score);
                //StartCoroutine("GetUsers");
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
    IEnumerator GetPerfil()
    {
        string path = "/api/usuarios/"+Username;
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
            StartCoroutine(ShowText(www.error, 3));
        }
        else
        {
            if (www.responseCode == 200)
            {
                string json = www.downloadHandler.text;
                response = JsonUtility.FromJson<AuthResponse>(json);
                Debug.Log(response);

                GameObject.Find("PanelAuth").SetActive(false);
                mainMenuPanel.SetActive(true); 
            }
            else
            {
                StartCoroutine(ShowText("Token Vencido... Redireccionar a login", 3));
                Debug.Log("Token Vencido... Redireccionar a login");
            }
        }
    }
    IEnumerator GetUsers()
    {
        string path = "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url + path);
        www.SetRequestHeader("x-token", Token);
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
                string json = www.downloadHandler.text;
                UsersList response = JsonUtility.FromJson<UsersList>(json);
                position = new Vector3(0, -170, 0);
                foreach (var user in response.usuarios)
                {

                    GameObject userObject = Instantiate(UserLeaderboardPrefab, transform);

                    // Asigna la posición
                    userObject.GetComponent<RectTransform>().anchoredPosition = position;

                    // Obtén los componentes TextMeshProUGUI
                    TextMeshProUGUI usernameText = userObject.transform.Find("Text_Username").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI scoreText = userObject.transform.Find("Text_Score").GetComponent<TextMeshProUGUI>();

                    // Asigna los valores de username y score
                    if (usernameText != null)
                    {
                        usernameText.text = user.username;
                    }
                    if (scoreText != null)
                    {
                        scoreText.text = user.data.score.ToString();
                    }

                    // Muestra el log en la consola
                    Debug.Log(user.username + "|" + user.data.score);

                    userInstances.Add(userObject);

                    // Ajusta la posición para el siguiente elemento
                    position.y -= verticalSpacing;
                }
                
            }
            else
            {
                Debug.Log("Token Vencido... Redireccionar a login");
            }
        }
    }

    IEnumerator ShowText(string text, int duration)
    {
        errorText.text = text;
        yield return new WaitForSeconds(duration);
        errorText.text = "";
        
    }

    // Función para desinstanciar todos los GameObjects
    public void DestroyUserInstances()
    {
        // Recorre la lista de instancias y las destruye
        foreach (var instance in userInstances)
        {
            Destroy(instance);
        }

        // Limpia la lista
        userInstances.Clear();

        Debug.Log("Todos los GameObjects han sido desinstanciados.");
    }
    public int ReturnFinalScore(int score)
    {
        return score;
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

