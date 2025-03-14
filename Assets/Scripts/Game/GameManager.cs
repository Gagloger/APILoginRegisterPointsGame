using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> dicesPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 5f;
    private int diceAmount; private int dicesThrown;

    public int totalScore;
    public int guessScore;
    public int userScore;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI guessText;
    [SerializeField] TextMeshProUGUI userScoreText;
    [SerializeField] TextMeshProUGUI mensageText;

    public delegate void OnSpawnDices();
    public OnSpawnDices OnSpawnDicesEvent;

    public APIManager apiManager;
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject gameMenu1;
    public GameObject gameMenu2;
    public GameObject endMenu;

    private void Awake() {
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }
    private void Start() {
        scoreText.gameObject.SetActive(false);
        guessText.gameObject.SetActive(false);
        userScoreText.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
        gameMenu1.SetActive(true);
        gameMenu2.SetActive(false);
        endMenu.SetActive(false);
    }

    public void SetGuess(){
        int guess = int.Parse(GameObject.Find("GuessInputField").GetComponent<TMP_InputField>().text);
        if (guess <= diceAmount*6 && guess > 0){
        guessScore = guess;
        guessText.text = $"Tu número: {guessScore}";

        scoreText.gameObject.SetActive(true);
        guessText.gameObject.SetActive(true);
        mensageText.text = "";  
        gameMenu2.SetActive(false);
        OnThrowDices();
        } else {
            GameObject.Find("GuessInputField").GetComponent<TMP_InputField>().text = "";
            //GameObject.Find("GuessInputField").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = "El número debe estar en el rango de los dados";
            mensageText.text = "El número debe estar en el rango de los dados";
        }
    }

    public void SetDiceAmount(int amount){
        diceAmount = amount;
    }

    public void OnThrowDices(){
        SpawnDices(diceAmount);
    }
    public void SpawnDices(int amount){
        OnSpawnDicesEvent?.Invoke(); //Desaparecer los dados si existe el evento

        for(int i = 0; i < amount; i++){
            int randomIndex = UnityEngine.Random.Range(0, dicesPrefabs.Count);
            StartCoroutine(ThrowDices(randomIndex));
        }
    }
    private IEnumerator ThrowDices(int randomIndex){
        GameObject dice = Instantiate(dicesPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        Dice d = dice.GetComponent<Dice>();
        OnSpawnDicesEvent += d.DestroyDice;
            

        Rigidbody rb = dice.GetComponentInChildren<Rigidbody>();
        rb.AddForce(new Vector3(1,0.2f,UnityEngine.Random.Range(1, 3)) * throwForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(UnityEngine.Random.Range(50, 200), UnityEngine.Random.Range(50, 200), UnityEngine.Random.Range(50, 200)));
        yield return new WaitForSeconds(0.5f);
        d.HaveBeenThrow();
    }

    public void SetScore(int value){
        dicesThrown++;

        totalScore += value;
        scoreText.text =  $"Valor dados: {totalScore}";

        if (dicesThrown == diceAmount){
            CalculateScore();
        }
        
    }
    private void CalculateScore(){
        endMenu.SetActive(true);
        

        userScore = math.abs(totalScore-guessScore);
        if (userScore == 0){
            userScore = totalScore * 5;
        }
        userScoreText.gameObject.SetActive(true);
        userScoreText.text = $"Tu puntaje es: \n {userScore}";

        SendScoreToServer();
    }
    public void SendScoreToServer(){
        apiManager.UpdateData(userScore);
        apiManager.LoadLeaderBoard();
    }

    public void Reset(){
        totalScore = 0;
        dicesThrown = 0;
        userScoreText.text = "";
        scoreText.text = "";
        guessText.text = "";

        userScoreText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        guessText.gameObject.SetActive(false);
        endMenu.SetActive(false);
        gameMenu1.SetActive(true);

        EraseLeaderBoard();
    }
    public void Return(){
        totalScore = 0;
        dicesThrown = 0;
        userScoreText.text = "";
        scoreText.text = "";
        guessText.text = "";


        userScoreText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        guessText.gameObject.SetActive(false);

        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
        gameMenu1.SetActive(true);
        gameMenu2.SetActive(false);
        endMenu.SetActive(false);

        EraseLeaderBoard();
    }

    private void EraseLeaderBoard(){
        apiManager.EraseLeaderBoard();
    }
}
