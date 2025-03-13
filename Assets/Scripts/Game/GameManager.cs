using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> dicesPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 5f;

    public int score;
    [SerializeField] TextMeshProUGUI scoreText;

    public delegate void OnSpawnDices();
    public OnSpawnDices OnSpawnDicesEvent;

    private void Awake() {
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }
    public void SpawnDices(int amount){
        OnSpawnDicesEvent?.Invoke(); //Desaparecer los dados si existe el evento

        for(int i = 0; i < amount; i++){
            int randomIndex = Random.Range(0, dicesPrefabs.Count);
            StartCoroutine(ThrowDices(randomIndex));
        }
    }
    private IEnumerator ThrowDices(int randomIndex){
        GameObject dice = Instantiate(dicesPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        Dice d = dice.GetComponent<Dice>();
        OnSpawnDicesEvent += d.DestroyDice;
            

        Rigidbody rb = dice.GetComponentInChildren<Rigidbody>();
        rb.AddForce(new Vector3(1,0.2f,Random.Range(1, 3)) * throwForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(50, 200), Random.Range(50, 200), Random.Range(50, 200)));
        yield return new WaitForSeconds(0.5f);
        d.HaveBeenThrow();
    }

    public void SetScore(int value){
        score += value;
        scoreText.text =  $"Score: {score}";
    }
}
