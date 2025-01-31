using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Sandpit : NetworkBehaviour
{

    bool active = false;
    bool isSpawning = false;
    bool readyToDespawn = false;
    public float SpawnTimerDelay = 10f;
    public float StayingTime = 5f;
    public float growShrinkSpeed = .1f;
    Vector3 startingScale = new Vector3(.01f, .01f, 1f);
    Vector3 endingScale = new Vector3(2f, 2f, 1f);
    GameObject[] spawningPoints; 
    public GameObject particles;
    public LayerMask collisionLayer;
    Collider2D player = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawningPoints = GameObject.FindGameObjectsWithTag("ObstacleSpawnpoint");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer){
            if(!active && !isSpawning){
                StartCoroutine(SpawnTimer(SpawnTimerDelay));
            }else if(readyToDespawn){
                DespawnSandPit();
            }else if (active){
                SpawnSandPit();
            }
            // Update Position with Server
            SubmitPositionAndScaleRequestServerRpc(gameObject.transform.position, gameObject.transform.localScale);
        }
        if(gameObject.transform.localScale.x >= startingScale.x){
            CheckForPlayers();
        }
        gameObject.transform.Rotate(new Vector3(0, 0, 10 * Time.deltaTime));
    }

    private IEnumerator SpawnTimer(float delay)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);
        active =  true;
        isSpawning = false;
        // Spawn at random spawnpoint
        transform.position = spawningPoints[Random.Range(0, spawningPoints.Length)].gameObject.transform.localPosition;
    }

    private IEnumerator DespawnTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToDespawn = true;
    }

    void SpawnSandPit(){
        // Particles

        // Grow with particles
        if(gameObject.transform.localScale.x <= endingScale.x){
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + growShrinkSpeed * Time.deltaTime,
         gameObject.transform.localScale.y + growShrinkSpeed * Time.deltaTime, 1);
        }

        // Reach Max size
        if(gameObject.transform.localScale.x >= endingScale.x){
            StartCoroutine(DespawnTimer(StayingTime));
        }
    }

    void DespawnSandPit(){
        // Shrink to starting scale
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - growShrinkSpeed * Time.deltaTime,
         gameObject.transform.localScale.y - growShrinkSpeed * Time.deltaTime, 1);
        // Disable
        if(gameObject.transform.localScale.x <= startingScale.x){
            active =  false;
            readyToDespawn = false;
        }
    }

    void CheckForPlayers() {
    // Get all colliders within the circle
    Collider2D[] playersInArea = Physics2D.OverlapCircleAll(transform.position, gameObject.transform.localScale.x * 1.1f, collisionLayer);

    // Iterate through all colliders
    foreach (Collider2D playerCollider in playersInArea) {
        // Check if the collider has a Player component
        Player playerComponent = playerCollider.GetComponent<Player>();
        if (playerComponent != null) {
            // Rotate the player with the circle
            playerCollider.transform.Rotate(new Vector3(0, 0, 50f * Time.deltaTime));
            }
        }
    }


    [ServerRpc]
    void SubmitPositionAndScaleRequestServerRpc(Vector3 position, Vector3 scale)
    {
        transform.position = position;
        transform.localScale = scale;
        particles.transform.localScale = scale;
        SyncPositionAndScaleClientRpc(position, scale);
    }

    [ClientRpc]
    void SyncPositionAndScaleClientRpc(Vector3 position, Vector3 scale)
    {
        if (!IsOwner)
        {
            transform.position = position;
            transform.localScale = scale;
            particles.transform.localScale = scale;
        }
    }
}
