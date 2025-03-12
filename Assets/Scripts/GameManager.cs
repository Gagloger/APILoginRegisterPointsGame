using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<GameObject> dicesPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 5f;

    public void SpawnDices(int amount){
        for(int i = 0; i < amount; i++){
            int randomIndex = Random.Range(0, dicesPrefabs.Count);
            GameObject dice = Instantiate(dicesPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

            Rigidbody rb = dice.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
        }
    }
}
