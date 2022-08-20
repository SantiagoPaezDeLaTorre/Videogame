using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Events;

public class UIManager : MonoBehaviour {   
    
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private HealthManager healthManager;

    private void Start() {
        ChangeSliderValue(healthManager.health);
    }
    private void OnEnable() {
        healthManager.healthChangeEvent.AddListener(ChangeSliderValue);
    }
    private void OnDisable() {
        healthManager.healthChangeEvent.RemoveListener(ChangeSliderValue);
    }
    public void ChangeSliderValue(int healthAmount) {
        slider.value = healthAmount;
    } 
}
