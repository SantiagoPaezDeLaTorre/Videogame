using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidFloor : MonoBehaviour
{
    //private PlayerStats stats;
    private bool isOnAcid;
    public int damage = -10;
    private float timeToDamage = 1f;
    private float timer = 0;
    [SerializeField]
    private HealthManager healthManager;


    void OnTriggerStay(Collider other) {
        if (other.transform.gameObject.tag == "Acid") {
            isOnAcid = true;
            timer = 0;
            timer += Time.deltaTime;
            if (timer >= timeToDamage) {
                Debug.Log(damage+" de vida");
                healthManager.Health(damage);
                //TakeDamage();
                isOnAcid = false;
            }
        }
    }

    
    // void TakeDamage() {
    //     stats.currentHealth -= damage;
    //     timer = 0;
    //     Debug.Log(stats.currentHealth);
    // }

}
