using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    public Animator animator;
    public Rigidbody body; 

    public Transform player;

    public float speed = 100f;
    private Vector3 moveDirection;
    public float shootDistance = 25f;
    public float meleeDistance = 12f;
    float enemyHeight;

    [HideInInspector] 
    public float distance;
    public float detectionDistance = 50f;

    void Start() {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        enemyHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update() {

        CheckPlayerDistance(player);
        moveDirection = Vector3.zero;
        LookAtPlayer(player);
    }
    void FixedUpdate() {
        body.AddForce(moveDirection * speed);
    }

    public void CheckPlayerDistance(Transform player) {
        distance = Vector3.Distance(transform.position, player.position);
    }

    public virtual void LookAtPlayer(Transform player)  {
        if ((distance <= detectionDistance)) {
            //transform.LookAt(player);
            Vector3 direction = player.position - this.transform.position;
            direction.y = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                          Quaternion.LookRotation(direction), 0.1f);
            animator.SetBool("isIdle", false);
            //FollowPlayer();
            //AttackBehaviour();
            if (direction.magnitude > .8) {
                moveDirection = direction.normalized;
                animator.SetBool("isWalking", true);
                animator.SetBool("isStriking", false);
            } else {
                animator.SetBool("isStriking", true);
                animator.SetBool("isWalking", false);
            }
        } else {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isStriking", false);
        }
        //else {
        //    transform.rotation = Quaternion.EulerAngles(player.position - transform.position);
        //}
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
            body.AddForce(Vector3.forward * speed, ForceMode.Force);
            Debug.Log("adding force");
            //transform.position = Vector3
            //.Lerp(transform.position, Player.position, speed*Time.deltaTime);
            if (transform.position.y != enemyHeight) {
                transform.position = new Vector3(transform.position.x, enemyHeight, transform.position.z);
            }   
        }
    }
}
