using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace MyGame {
    public class Talismans : MonoBehaviour {
        //private ThirdPersonActionAsset playerActionAsset;
        public TaliObjects taliObjects;
        public HealthManager healthManager;
        public PlayerMovement player;

        private float taliTimeEffect = 5f;
        private float taliCooldown = 10f;
        private bool earthTali;
        private bool isEarthReady = true;
        private bool windTali;
        private bool isWindReady = true;
        private bool fireTali;
        private bool isFireReady = true;
        private bool waterTali;
        private bool isWaterReady = true;

        private void Awake() {
            //playerActionAsset = GetComponent<ThirdPersonActionAsset>();
        }
        // private void OnEnable() {
        //     playerActionAsset.Player.EarthTali.started += UseEarth;
        //     playerActionAsset.Player.WindTali.started += UseWind;
        //     playerActionAsset.Player.FireTali.started += UseFire;
        //     playerActionAsset.Player.WaterTali.started += UseWater;
        //     playerActionAsset.Player.Enable();
        // }
        // private void OnDisable() {
        //     playerActionAsset.Player.EarthTali.started -= UseEarth;
        //     playerActionAsset.Player.WindTali.started -= UseWind;
        //     playerActionAsset.Player.FireTali.started -= UseFire;
        //     playerActionAsset.Player.WaterTali.started -= UseWater;
        //     if (playerActionAsset != null) {
        //         playerActionAsset.Player.Disable();
        //     }
        // }

        void Update() {
            pickTali();
        }

        // prepare scenario for switch
        public int getTaliPicked() {
            int response = 0;
            // creo que los & ready estan de mas 
            if (earthTali & isEarthReady) {
                earthTali = false;
                response = 1;
            } else if (windTali & isWindReady) {
                windTali = false;
                response = 2;
            } else if (fireTali & isFireReady) {
                fireTali = false;
                response = 3;
            } else if (waterTali & isWaterReady) {
                waterTali = false;
                response = 4;
            }
            return response;
        }
        // SWITCH
        public void pickTali() {
            switch (getTaliPicked()) {
                case 1:
                    SummonEarth();
                    break;
                case 2:
                    SummonWind();
                    break;
                case 3:
                    SummonFire();
                    break;
                case 4:
                    SummonWater();
                    break;
            }
        }
        
        
// ---------- USE TALIS ----------
        public void SummonEarth() {
            isEarthReady = false;
            Debug.Log("active earth");
            Invoke(nameof(ResetEarthTali), taliTimeEffect);
        }
        public void SummonWind() {
            Debug.Log("active wind");
            isWindReady = false;
            player.sprintSpeed = player.sprintSpeed + 4.2f;
            player.moveSpeed = player.moveSpeed + 1.5f;
            player.jumpHeight = player.jumpHeight + 2.5f;
            Invoke(nameof(ResetWindTali), taliTimeEffect);
        }
        public void SummonFire() {
            Debug.Log("active fire");
            isFireReady = false;
            healthManager.Health(20);
            Invoke(nameof(ResetFireTali), taliTimeEffect);
        }
        public void SummonWater() {
            isWaterReady = false;
            Debug.Log("active water");
            Invoke(nameof(ResetWaterTali), taliTimeEffect);
        }


// ---------- RESET TALIS ----------
        private void ResetEarthTali() {
            isEarthReady = true;
            taliObjects.earthCount += 1;
            int value = 1;
            taliObjects.addUse(value);
        }
        private void ResetWindTali() {
            isWindReady = true;
            player.sprintSpeed = player.sprintSpeed - 4.2f;
            player.moveSpeed = player.moveSpeed - 1.5f;
            player.jumpHeight = player.jumpHeight - 2.5f;
            
            // int value = 2;
            // taliObjects.addUse(value);
        }
        private void ResetFireTali() {
            isFireReady = true;
            
            int value = 3;
            taliObjects.addUse(value);
        }
        private void ResetWaterTali() {
            isWaterReady = true;
            
            int value = 4;
            taliObjects.addUse(value);
        }

// --------- INPUT --------
    //     private void UseEarth(InputAction.CallbackContext value) {
    //         if (isEarthReady) {
    //             earthTali = true;
    //         }
    //     }
    //     private void UseWind(InputAction.CallbackContext value) {
    //         if (isWindReady) {
    //             windTali = true;
    //         }
    //         if (value.performed) {
    //             int num = 2;
    //             taliObjects.addUse(num);
    //         }
    //     }
    //     private void UseFire(InputAction.CallbackContext value) {
    //         if (isFireReady) {
    //             fireTali = true;
    //         }
    //     }
    //     private void UseWater(InputAction.CallbackContext value) {
    //         waterTali = true;
    //     }
    }
}

