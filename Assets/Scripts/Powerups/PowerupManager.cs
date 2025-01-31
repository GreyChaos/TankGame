using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PowerupManager : NetworkBehaviour
{

    public float spawnTimer = 3f;
    Dictionary<GameObject, bool> powerupSpawnPointsInUse = new();
    GameObject[] powerupSpawnPoints; 
    public List<Powerup> powerups = new();
    bool isSpawning = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerupSpawnPoints = GameObject.FindGameObjectsWithTag("PowerupSpawn");
        foreach(GameObject spawnpoint in powerupSpawnPoints){
            powerupSpawnPointsInUse.Add(spawnpoint, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) return;
        if(!isSpawning)
        StartCoroutine(SpawnTimer(spawnTimer));
    }

    private IEnumerator SpawnTimer(float delay)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);
        SpawnNewPowerups();
        isSpawning = false;
    }

    void SpawnNewPowerups(){
        int randomSpotSpot = Random.Range(0, powerupSpawnPoints.Length);
        if(powerupSpawnPoints[randomSpotSpot].gameObject.GetComponentInChildren<Powerup>() == null){
            powerupSpawnPointsInUse[powerupSpawnPoints[randomSpotSpot]] = false;
        }
        if(powerupSpawnPointsInUse[powerupSpawnPoints[randomSpotSpot]]){
            return;
        }
        powerupSpawnPointsInUse[powerupSpawnPoints[randomSpotSpot]] = true;
        SpawnPowerupServerRpc(powerupSpawnPoints[randomSpotSpot].gameObject.transform.position, Quaternion.identity, randomSpotSpot);
    }

    [ServerRpc]
    void SpawnPowerupServerRpc(Vector3 position, Quaternion rotation, int spawnSpotIndex){
        // Instantiate the shell at the given position and rotation on the server
        GameObject newPowerup = Instantiate(powerups[Random.Range(0, powerups.Count)].gameObject, position, rotation);
        if(newPowerup.GetComponent<Powerup>() == null){
            return;
        }
        newPowerup.GetComponent<Powerup>().spawnSpot = powerupSpawnPoints[spawnSpotIndex];
        // Spawns shell with clientID
        newPowerup.GetComponent<NetworkObject>().Spawn();
        newPowerup.transform.parent = powerupSpawnPoints[spawnSpotIndex].transform;
    }
}
