using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : DetectionCore {
    
    public float speed = 1f;
    public float shootDistance = 25f;
    public float meleeDistance = 12f;
    float enemyHeight;

    void Start() {
        enemyHeight = transform.position.y;
    }

    void Update() {
        CheckToPlayerDistance(Player);
        LookAtPlayer(Player);
        AttackBehaviour();
        FollowPlayer();
    }

    void AttackBehaviour() {
        if ((distance > meleeDistance) && (distance <= detectionDistance)) {
            Debug.Log("will shoot");
        } else if (distance <= meleeDistance) {
            Debug.Log("will strike melee");
        }
    }
    void FollowPlayer() {
        if (distance <= detectionDistance 
        && distance > meleeDistance) {
            
            transform.position = Vector3
            .Lerp(transform.position, Player.position, speed*Time.deltaTime);
            if (transform.position.y != enemyHeight) {
                transform.position = new Vector3(transform.position.x, enemyHeight, transform.position.z);
            }   
        }
    }
}
