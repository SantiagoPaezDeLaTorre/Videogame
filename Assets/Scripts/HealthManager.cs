using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "HealthManager", menuName = "myGame2/HealthManager", order = 0)]
public class HealthManager : ScriptableObject {
    
    public int health = 100;

    [SerializeField]
    private int maxHealth = 100;

    [System.NonSerialized]
    public UnityEvent<int> healthChangeEvent;

    private void OnEnable() {
        health = maxHealth;
        if (healthChangeEvent == null) {
            healthChangeEvent = new UnityEvent<int>();
        }
    }

    public void Health(int healthAmount) {
        health += healthAmount;
        healthChangeEvent.Invoke(health);
        Debug.Log("changed health to " +health);
    }

}
