using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string url = "https://sid-restapi.onrender.com/";
    [SerializeField] private string Token;




    IEnumerator RegisterPost(string postData)
 {
     string path = "/api/usuarios";
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
             Debug.Log(www.downloadHandler.text);
             StartCoroutine(LoginPost(postData));
         }
            
         else 
         {
             string mensaje = "status:" + www.responseCode;
             mensaje += "\nError: " + www.downloadHandler.text;
             Debug.Log(mensaje);
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


             GameObject.Find("PanelAuth").SetActive(false);

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
             AuthResponse response = JsonUtility.FromJson<AuthResponse>(json);
             GameObject.Find("PanelAuth").SetActive(false);
         }
         else
         {
             Debug.Log("Token Vencido... redirecionar a Login");
         }
     }
 }
}

public class AuthResponse
{
    public string token;
    public Usuario usuario;
}

public class Usuario
{
    public int id;
    public string username;
    public string password;
    public int points;
}
