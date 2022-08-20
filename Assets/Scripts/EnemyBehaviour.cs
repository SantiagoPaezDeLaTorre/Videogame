using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : DetectionCore {
    
    public float speed = 1f;
    public float zombieSlower = 0.1f;
    public float shootDistance = 25f;
    public float meleeDistance = 12f;
    public float reactionTime = 1.5f;
    float enemyHeight;
    public Transform zombieHead;

    void Start() {
        enemyHeight = transform.position.y;
    }

    void Update() {
        CheckToPlayerDistance(Player);
        if (distance <= detectionDistance) {
            GetToPlayerRotation(Player);
            LookAtPlayer(Player);
            Invoke(nameof(FollowPlayer), reactionTime);
        }
        AttackBehaviour();
    }

    public void GetToPlayerRotation(Transform player) {
        zombieHead.LookAt(player);
    }

    public override void LookAtPlayer(Transform player) {
        if (distance <= detectionDistance) {
            //StartCoroutine(RotateOverTime(transform.rotation, zombieHead.rotation, zombieSlower));
            transform.LookAt(player);
            Debug.Log("enemiyyyy");
            //transform.rotation = Quaternion.Slerp(transform.rotation, player.position, zombieSlower*Time.deltaTime);
        }
    }

    //IEnumerator RotateOverTime(Quaternion originalRotation, Quaternion finalRotation, float duration) {
    //    if (duration > 0f) {
    //        float startTime = Time.time;
    //        float endTime = startTime + duration;
    //        transform.rotation = originalRotation;
    //        yield return null;
    //        while (Time.time < endTime) {
    //            float progress = (Time.time - startTime) / duration;
    //            // progress will equal 0 at startTime, 1 at endTime.
    //            transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, progress);
    //            yield return null;
    //        }
    //    }
    //    transform.rotation = finalRotation;
    //}


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
