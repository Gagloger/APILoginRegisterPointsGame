using UnityEngine;

public class Diceface : MonoBehaviour
{
    public int faceValue;
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("FaceCheck")){
            other.GetComponent<TopFaceCheck>().UpFaceValue = faceValue;
        }
    }
}
