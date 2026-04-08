using UnityEngine; using System;

public class CreatureMovement : MonoBehaviour {
    float wonderCooldown = 2;
    float moveTimeout;
    bool Stopped;

    public Vector3 MoveTarget;
    Vector3 targetDirection;
    MovementMethods method;

    CreatureBrain brain;

    void Start() {
        brain = gameObject.GetComponent<CreatureBrain>();
    }

    public void setMovementMethod(MovementMethods newMethod) {
        method = newMethod;
    }

    void Update() {
        if (!Stopped) {
            moveTimeout += Time.deltaTime;
            if (Vector2.Distance(transform.position, MoveTarget) >= 0.2f) {
                transform.Translate(targetDirection * Time.deltaTime);
            }

            if (method == MovementMethods.wonder && moveTimeout > wonderCooldown) {
                manageWonder();
            }
        }
    }
    void manageWonder() {
        moveTimeout = 0;
        Debug.Log("Target Reassembled ");
        for (int i = 0; i < 10; i++) {
            Vector2 candidate = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 2;
            Vector2 direction = candidate - (Vector2)transform.position; 
            float distance = direction.magnitude;
            RaycastHit2D hitObstacle = Physics2D.Raycast(transform.position, direction.normalized, distance, LayerMask.NameToLayer("obst"));
            Debug.DrawRay(transform.position, direction.normalized, Color.red, distance);
            if (hitObstacle.collider == null) { 
                targetDirection = direction; 
                MoveTarget = candidate; 

                brain.VasteEnergy(); 
                return; 
            }
        } 
    }

    void StopMovement() { Stopped = true; }
    void ResumeMovement() { Stopped = false; }
    public void sleep() { StopMovement(); brain.ToggleSleep(); }
    public void wakeup() { ResumeMovement(); brain.ToggleSleep(); }
}

public enum MovementMethods {
    wonder,
    run
}