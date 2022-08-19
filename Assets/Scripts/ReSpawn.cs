using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawn : MonoBehaviour
{
    private CharacterController _controller;
    private GameObject player;
    private List<GameObject> playerSpawners = new List<GameObject>();
    private Vector3[] spawnPositions;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        FindSpawnPoints();
    }

    // Update is called once per frame
    void Update()
    {
        RespawnPlayer();
    }

    private void FindSpawnPoints() {
        // get all spawners into list
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("SpawnSpot")) {
            playerSpawners.Add(spawner);
        }
        // set array with spawners positions
        spawnPositions = new Vector3[playerSpawners.Count];
        for (var i=0; i<playerSpawners.Count; i++) {
            spawnPositions[i] = playerSpawners[i].transform.position;
        }
    }
    private void RespawnPlayer() {
        if (player.transform.position.y < -45f) {
            _controller.enabled = false;
            player.transform.position = spawnPositions[Random.Range(0,spawnPositions.Length)];
            _controller.enabled = true;
        }
    }
}
