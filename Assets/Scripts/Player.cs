using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public Animator animator;
    public GameObject shell;
    Rigidbody2D rb;
    public LayerMask collisionLayer;
    private int hitCount = 0;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsOwner)
        {
            float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;
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
            
            if(Input.GetKeyDown(KeyCode.Space)){
                FireShell();
            }
        }
    }
    public void IncrementHitCount(int damage)
    {
        hitCount += damage;
        if (hitCount >= 3)
        {
            DestroyPlayer();
        }
    }

    private void DestroyPlayer()
    {
        hitCount = 0;
        transform.localScale -= new Vector3(0.1f, 0.1f, 0f);
    }

    bool CanMove(float direction){
        Vector2 newPosition;
        if(direction > 0){
            newPosition = transform.position + transform.up * 1f;
        }else{
            newPosition = transform.position - transform.up * 1f;
        }
        // OverlapCircle checks if there's any collider within a radius of the new position
        return !Physics2D.OverlapCircle(newPosition, .05f, collisionLayer);
    }

    void FireShell(){
        // move shell x + 1
        Vector3 spawnPosition = transform.position + transform.up * 1f;
        FireShellServerRpc(spawnPosition, transform.rotation);
    }

    [ServerRpc]
    void FireShellServerRpc(Vector3 position, Quaternion rotation)
    {
        // Instantiate the shell at the given position and rotation on the server
        GameObject newShell = Instantiate(shell, position, rotation);

        // Spawn the shell object on all clients via NetworkObject
        newShell.GetComponent<NetworkObject>().Spawn();
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
}
