using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormPartCollider : MonoBehaviour {
    private WormBehaviour associatedWorm;

    public WormPartCollider Init(WormBehaviour worm) {
        this.associatedWorm = worm;
        
        return this;
    }
    
    private void OnTriggerEnter2D(Collider2D other) { // most enemies will have a trigger collider, so when the player crashes into them, he should take damage
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            associatedWorm.CollisionFlag(playerController);
        }
    }
}
