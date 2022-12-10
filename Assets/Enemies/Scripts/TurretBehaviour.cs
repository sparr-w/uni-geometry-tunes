using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour {
    public float ShotDelay = 0.5f;
    public Projectile ProjectileType;
    public float ProjectileSpeed = 0.5f;
    public Vector2 ProjectileScale = new Vector2(1.0f, 1.0f);
    
    [SerializeField] private EnemyHandler _handler;
    private PlayerController[] players; // pass through, eventually
    private bool isFiring = false;
    private float projectileRot = 0.0f;
    private Transform barrelComponent;
    
    private void Start() {
        isFiring = true;
        StartCoroutine(Shoot());
        
        barrelComponent = transform.GetChild(0).GetChild(0);
    }

    private PlayerController TargetClosest() {
        int closestIndex = 0;
        
        for (int i = 1; i < _handler.Players.Length; i++) { // computing squared magnitudes is faster
            if ((this.transform.position - _handler.Players[closestIndex].transform.position).sqrMagnitude <
                (this.transform.position - _handler.Players[i].transform.position).sqrMagnitude)
                closestIndex = i;
        }
        
        return _handler.Players[closestIndex];
    }

    private Vector2 Move(Vector2? distance) {
        
        
        // rotate barrel to face the player
        PlayerController target = TargetClosest();
        
        Vector3 diff = new Vector3(target.transform.position.x, target.transform.position.y, 0.0f) -
                       new Vector3(this.transform.position.x, this.transform.position.y, 0.0f);
        diff.Normalize();

        projectileRot = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90.0f;

        barrelComponent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, projectileRot);
        
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }
    
    private IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            FireProjectile(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, projectileRot));

            yield return new WaitForSeconds(ShotDelay);
        }
    }
    
    private Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) { // could make global?
        if (localPos == null) localPos = new Vector3(0.0f, 0.0f, 0.0f);
        if (localRot == null) localRot = new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj = Instantiate(ProjectileType, this.transform);
        proj.Init(ProjectileSpeed);
        
        proj.transform.localPosition = localPos.Value;
        proj.transform.localEulerAngles = localRot.Value;

        proj.transform.SetParent(null);
        proj.transform.localScale = new Vector3(ProjectileScale.x, ProjectileScale.y, 1.0f);

        return proj;
    }

    private void Update() {
        Move(new Vector2(0.0f, 0.0f));
    }
}
