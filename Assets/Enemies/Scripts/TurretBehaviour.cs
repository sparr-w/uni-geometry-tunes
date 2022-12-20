using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretBehaviour : ShooterBehaviour {
    private PlayerController[] players;

    private Transform barrelComponent;
    private float barrelRotation = 0.0f;

    public TurretBehaviour Init(PlayerController[] players) {
        this.players = players;
        return this;
    }

    private void Start() {
        StartCoroutine(Shoot());
        barrelComponent = this.transform.GetChild(0).transform.GetChild(0); // this should, if the structure of turrets isn't tampered with, find the barrel component
    }
    
    private PlayerController TargetClosest() {
        int closestIndex = 0;
        
        for (int i = 1; i < players.Length; i++) { // computing squared magnitudes is faster
            if ((this.transform.position - players[closestIndex].transform.position).sqrMagnitude <
                (this.transform.position - players[i].transform.position).sqrMagnitude)
                closestIndex = i;
        }
        
        return players[closestIndex];
    }
    
    protected override IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            FireProjectile(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, barrelRotation));

            yield return new WaitForSeconds(ShotDelay);
        }
    }
    
    protected override Vector3 Move(Vector2 distance) {
        
        
        // rotate barrel to face the player
        PlayerController target = TargetClosest();
        
        Vector3 diff = new Vector3(target.transform.position.x, target.transform.position.y, 0.0f) -
                       new Vector3(this.transform.position.x, this.transform.position.y, 0.0f);
        diff.Normalize();

        barrelRotation = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90.0f;

        barrelComponent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, barrelRotation);
        
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }
    
    private void Update() {
        Move(new Vector2(0.0f, 0.0f));
    }
}