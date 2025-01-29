using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Powerup : NetworkBehaviour
{

    bool inUse = false;
    float rotationSpeed = 35f;
    public float shootSpeedChange = 1f;
    public float moveSpeedChange = 1f;
    public int shotsPerShoot = 1;
    public float effectDuration = 5f;
    public LayerMask collisionLayer;
    Collider2D player = null;
    public GameObject spawnSpot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        if(Physics2D.OverlapCircle(transform.position, .05f, collisionLayer)){
            player = Physics2D.OverlapCircle(transform.position, 0.05f, collisionLayer);
            PickedUpByPlayer();
        }
    }



    void PickedUpByPlayer(){
        if(inUse) return;
        inUse = true;
        if(player.GetComponent<Player>().powerupActive){
            return;
        }
        player.GetComponent<Player>().powerupActive = true;
        player.GetComponent<Player>().PickupPowerup(this);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
        StartCoroutine(DestroyAfterDelay(effectDuration));
    }

    void ResetPlayer(){
        player.GetComponent<Player>().ResetPowerup();
        player.GetComponent<Player>().powerupActive = false;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetPlayer();
        RequestDespawn();
    }

    void RequestDespawn()
    {
        if(!IsOwner) return;
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            SubmitDespawnRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitDespawnRequestServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
