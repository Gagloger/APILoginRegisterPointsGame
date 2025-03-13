using UnityEngine;

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
