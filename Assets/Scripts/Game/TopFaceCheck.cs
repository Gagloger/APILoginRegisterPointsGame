using System.Collections;
using UnityEngine;

public class TopFaceCheck : MonoBehaviour
{
    public Transform dice;
    public int UpFaceValue;

    private void Awake() {
        gameObject.SetActive(false);
    }
        

    public void CheckFace(){
        gameObject.SetActive(true);
        transform.position = new Vector3(dice.position.x, dice.position.y + 1, dice.position.z);
        
        StartCoroutine(sendScore());
    }

    private IEnumerator sendScore(){
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.SetScore(UpFaceValue);
        Debug.Log("Has sacado un " + UpFaceValue);
        GetComponentInParent<Dice>().result = UpFaceValue;
    }
}
