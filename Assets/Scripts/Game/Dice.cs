using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TopFaceCheck topFaceCheck;

    private bool isRolling = false;

    public int result;
    private void OnEnable() {
        rb = GetComponentInChildren<Rigidbody>();
        topFaceCheck = GetComponentInChildren<TopFaceCheck>();
    }

    public void HaveBeenThrow(){
        isRolling = true;
    }
    private void Update() {
        if (rb==null) return;

        if (rb.linearVelocity.magnitude <= 0f && rb.angularVelocity.magnitude <= 0f && isRolling){
            isRolling = false;
            StopRolling();
        }
    }
    public void DestroyDice(){
        gameObject.SetActive(false);
        GameManager.Instance.OnSpawnDicesEvent -= DestroyDice;
        Destroy(gameObject);
    }

    public void StopRolling(){
        topFaceCheck.CheckFace();
    }
}
