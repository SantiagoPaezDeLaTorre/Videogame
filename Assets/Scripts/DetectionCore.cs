using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCore : MonoBehaviour
{
    
    [HideInInspector] public float distance;
    public float detectionDistance = 50f;
    public Transform Player;

    // Update is called once per frame
    void Update()
    {
        CheckToPlayerDistance(Player);
        LookAtPlayer(Player);
    }

    public void CheckToPlayerDistance(Transform player){
        distance = Vector3.Distance(transform.position, player.position);
    }
    public void LookAtPlayer(Transform player) {
        if (distance <= detectionDistance) {
            transform.LookAt(player);
        }
    }
}
