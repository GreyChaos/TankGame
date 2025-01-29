using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float shootSpeed = .5f;
    int shotsPerFire = 1;
    bool canFire = true;
    public float rotationSpeed = 100f;
    public Animator animator;
    public GameObject shell;
    public GameObject shotFiredParticle;
    public GameObject shotHitParticle;
    Rigidbody2D rb;
    public LayerMask collisionLayer;
    public int hitCount = 0;
    public NetworkVariable<Color> playerColor = new();
    ulong myClientId;
    public NetworkVariable<Vector3> playerScale = new();
    public bool powerupActive = false;

    void Start(){
        myClientId = NetworkManager.Singleton.LocalClientId;
        rb = GetComponent<Rigidbody2D>();
        playerColor.OnValueChanged += (oldColor, newColor) =>
        {
            GetComponent<SpriteRenderer>().color = newColor;
        };
    }

    void Update()
    {
        if (playerScale.Value != transform.localScale)
        {
            transform.localScale = playerScale.Value;
        }
        if (IsOwner)
        {
            float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float rotateInput = Input.GetAxis("Horizontal");
            float rotation = rotateInput * rotationSpeed * Time.deltaTime;
            

            if(CanMove(moveY)){
                transform.Translate(new Vector3(0, moveY, 0));
                transform.Rotate(0, 0, -rotation);
            }else{
                transform.Rotate(0, 0, -rotation);
            }
            SubmitPositionRequestServerRpc(transform.position, transform.rotation);
            if (moveY > 0){
                animator.SetTrigger("MoveForward");
                SumbitAnimationTriggerServerRpc("MoveForward");
            }else if (moveY < 0){
                animator.SetTrigger("MoveBackward");
                SumbitAnimationTriggerServerRpc("MoveBackward");
            }else{
                animator.SetTrigger("Idle");
                SumbitAnimationTriggerServerRpc("Idle");
            }
            
            if(Input.GetKeyDown(KeyCode.Space) && canFire){
                FireShell();
            }
        }
    }


    float tempShootSpeed;
    float tempMoveSpeed;
    int tempShotsPerFire;
    public void PickupPowerup(Powerup powerup){
        tempShootSpeed = shootSpeed;
        tempMoveSpeed = moveSpeed;
        tempShotsPerFire = shotsPerFire;

        shootSpeed = powerup.shootSpeedChange * shootSpeed;
        moveSpeed = powerup.moveSpeedChange * moveSpeed;
        shotsPerFire = powerup.shotsPerShoot;

        StopCoroutine(FireShellTimer(shootSpeed));
    }

    public void ResetPowerup(){
        shootSpeed = tempShootSpeed;
        moveSpeed = tempMoveSpeed;
        shotsPerFire = tempShotsPerFire;
    }

    public void IncrementHitCount(int damage, ulong shellOwner)
    {
        hitCount += damage;
        PlayerHitParticleServerRpc(transform.position, transform.rotation, myClientId);
        if (hitCount >= 3)
        {
            hitCount = 0;
            UpdatePlayerSizesServerRpc(shellOwner);   
        }
    }

    [ServerRpc(RequireOwnership=false)]
    void PlayerHitParticleServerRpc(Vector3 position, Quaternion rotation, ulong spawnPlayerID){
        GameObject newShotFiredParticle = Instantiate(shotHitParticle, position, rotation);
        newShotFiredParticle.GetComponent<NetworkObject>().SpawnWithOwnership(spawnPlayerID);
    }

    [ServerRpc(RequireOwnership=false)]
    public void UpdatePlayerSizesServerRpc(ulong shellOwner){
        if(playerScale.Value.x > .5){
            playerScale.Value -= new Vector3(0.1f, 0.1f, 0f);
            NetworkManager.ConnectedClients[shellOwner].PlayerObject.GetComponent<Player>().playerScale.Value += new Vector3(0.1f, 0.1f, 0f);
        }
    }

    bool CanMove(float direction){
        Vector2 newPosition;
        if(direction > 0){
            newPosition = transform.position + transform.up * playerScale.Value.x;
        }else{
            newPosition = transform.position - transform.up * playerScale.Value.x;
        }
        // OverlapCircle checks if there's any collider within a radius of the new position
        return !Physics2D.OverlapCircle(newPosition, .05f, collisionLayer);
    }

    void FireShell(){
        canFire = false;
        StartCoroutine(FireShellTimer(shootSpeed));
        // move shell x + 1
        Vector3 spawnPosition = transform.position + transform.up * playerScale.Value.x;
        FireShellServerRpc(spawnPosition, transform.rotation, myClientId);
    }

    private IEnumerator FireShellTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        canFire = true;
    }

    [ServerRpc]
    void FireShellServerRpc(Vector3 position, Quaternion rotation, ulong spawnPlayerID)
    {
        // Instantiate the shell at the given position and rotation on the server
        GameObject newShell = Instantiate(shell, position, rotation);
        // Spawns shell with clientID
        newShell.GetComponent<NetworkObject>().SpawnWithOwnership(spawnPlayerID);

        // Instantiate the shotFiredParticle at the given position and rotation on the server
        GameObject newShotFiredParticle = Instantiate(shotFiredParticle, position, rotation);
        newShotFiredParticle.GetComponent<NetworkObject>().SpawnWithOwnership(spawnPlayerID);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        SyncPositionClientRpc(position, rotation); // Update position for clients
    }

    [ServerRpc]
    void SumbitAnimationTriggerServerRpc(String trigger){
        animator.SetTrigger(trigger);
        SyncAnimationTriggerClientRpc(trigger);
    }

    [ClientRpc]
    void SyncPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        if (!IsOwner) // Skip updating the position for the local player (it's already updated locally)
        {
            transform.SetPositionAndRotation(position, rotation);
        }
    }

    [ClientRpc]
    void SyncAnimationTriggerClientRpc(String trigger){
        if(!IsOwner){
            animator.SetTrigger(trigger);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Only the server assigns the player's color
            playerColor.Value = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        // Apply the color immediately for the host
        ApplyColor(playerColor.Value);
    }

    private void ApplyColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
