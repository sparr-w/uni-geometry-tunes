using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretBehaviour : ShooterBehaviour {
    [Header("Turret Variables")]
    [SerializeField] private float rotationRate = 0.0f;
    
    private PlayerController[] players;
    private Transform barrelComponent;

    public TurretBehaviour Init(float moveSpeedMult = 1.0f, float rotationSpeed = 0.0f) {
        players = GlobalVariables.Players;
        this.rotationRate = rotationSpeed;
        this.moveSpeedMultiplier = moveSpeedMult;
        
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
            FireProjectile(new Vector3(0.0f, 0.0f, 0.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }
    
    private void AimAtClosest() {
        // rotate barrel to face the player
        PlayerController target = TargetClosest();

        Vector3 diff = new Vector3(target.transform.position.x, target.transform.position.y, 0.0f) -
                       new Vector3(this.transform.position.x, this.transform.position.y, 0.0f);
        diff.Normalize();

        float currentRotation = this.transform.localEulerAngles.z;
        float targetRotation = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90.0f;
    
        // interpolating rotation so that if the enemy is using a laser or fast firing weaponry, it can be made weaker by rotating slowly
        if (rotationRate > 0.0f) { // only interpolates if the rate is more than 0, 0 is default, snap to target
            if ((currentRotation + 180.0f >= targetRotation && currentRotation <= targetRotation) ||
                (currentRotation + 180.0f >= targetRotation + 360.0f && currentRotation <= targetRotation + 360.0f))
                currentRotation = currentRotation + rotationRate * Time.deltaTime * 30.0f; // * 30.0f because the rate of change is so low
            else
                currentRotation = currentRotation - rotationRate * Time.deltaTime * 30.0f; // same * 30.0f the rate it too slow
        } else currentRotation = targetRotation;

        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, currentRotation);
    }
    
    protected override void Update() {
        AimAtClosest();
        MovePatternChase(TargetClosest().transform.position);
    }
}