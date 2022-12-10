using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile {
    private Vector2 startPos;
    
    // appear during initialisation, in the correct position
    public Laser Init(Vector2 _startPos) {
        this.startPos = _startPos;
        return this;
    }
    
    // expand in size, becoming more opaque and clear it is going to "blast"
    
    // "blast", everything disappears briefly before a solid beam appears, dealing damage
    
    // the beam should stick the barrel it is firing from, so it may need to move with owner
}
