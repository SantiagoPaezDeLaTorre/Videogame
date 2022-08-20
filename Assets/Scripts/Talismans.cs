using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {
    public class Talismans : MonoBehaviour {
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
        private int taliUsed = 0;

        [Header("Keybinds")]
        public KeyCode earthKey = KeyCode.Alpha1;
        public KeyCode windKey = KeyCode.Alpha2;
        public KeyCode fireKey = KeyCode.Alpha3;
        public KeyCode waterKey = KeyCode.Alpha4;

        void Update() {
            MyInput();
        }

        private void MyInput() {
            taliUsed = 0;
            if (Input.GetKey(earthKey) && isEarthReady) {
               //earthTali = true;
                taliUsed = 1;
            } else if (Input.GetKey(windKey) && isWindReady) {
                //windTali = true;
                taliUsed = 2;
            } else if (Input.GetKey(fireKey) && isFireReady) {
                //fireTali = true;
                taliUsed = 3;
            } else if (Input.GetKey(waterKey) && isWaterReady) {
                //waterTali = true;
                taliUsed = 4;
            }

            if (taliUsed != 0) {
                SummonTalis(taliUsed);
            }

        }

        private void SummonTalis(int tali) {
            switch (tali) {
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
            player.sprintSpeed = player.sprintSpeed + 6f;
            player.moveSpeed = player.moveSpeed + 2f;
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
        }
        private void ResetWindTali() {
            isWindReady = true;
            player.sprintSpeed = player.sprintSpeed - 6f;
            player.moveSpeed = player.moveSpeed - 2f;
            player.jumpHeight = player.jumpHeight - 2.5f;
            taliObjects.windCount += 1;
        }
        private void ResetFireTali() {
            isFireReady = true;
            taliObjects.fireCount += 1;
        }
        private void ResetWaterTali() {
            isWaterReady = true;
            taliObjects.waterCount += 1;
        }
    }
}

