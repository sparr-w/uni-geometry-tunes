using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackType {
    Projectile,
    Laser
}

public enum ProjectileHandlers {
    Standard,
    Pooling,
    Entities
}

public class ShooterBehaviour : Enemy {
    [Header("Shooter Variables")]
    [SerializeField] protected ProjectileHandlers projectileHandler = ProjectileHandlers.Standard;
    public bool isFiring = true;

    [Header("Shooter Attack Prefabs")] 
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected Projectile laserPrefab;
    
    [Header("Shooter Projectile Variables")]
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float projSpeedMultiplier = 1.0f;
    [SerializeField] protected float projScaleMultiplier = 1.0f;
    [SerializeField] protected float ShotDelay = 0.2f;
    
    protected readonly float projSpeed = 3.5f;
    protected Vector2 projScale = new Vector2(.3f, .3f);

    private Sprite projSprite;
    private Shapes projShape;

    protected virtual IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            FireProjectile(new Vector3(0.0f, 0.5f, 0.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    public ShooterBehaviour InitProjectiles(float shotDelay = 0.2f, float projSpeed = 1.0f, float projSize = 1.0f) {
        this.ShotDelay = shotDelay;
        this.projSpeedMultiplier = projSpeed;
        this.projScaleMultiplier = projSize;
        return this;
    }

    public ShooterBehaviour SetAttackType(AttackType newType) {
        this.attackType = newType;
        return this;
    }
    public ShooterBehaviour SetProjectileHandler(ProjectileHandlers handler) {
        this.projectileHandler = handler;
        return this;
    }
    public ShooterBehaviour InitShooterProfile(AttackType attackType, ProjectileHandlers handler) {
        SetAttackType(attackType);
        SetProjectileHandler(handler);
        return this;
    }

    public ShooterBehaviour SetProjectileShape(Shapes newShape) {
        this.projShape = newShape;
        return this;
    }
    public ShooterBehaviour SetProjectileSprite(Sprite newSprite) {
        this.projSprite = newSprite;
        return this;
    }
    
    protected Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) {
        // if the values aren't set (are null), give them default values -- this method is used because you cannot null regular vectors
        localPos ??= new Vector3(0.0f, 0.0f, 0.0f);
        localRot ??= new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj;
        // get/make new projectile
        switch (projectileHandler) {
            case ProjectileHandlers.Pooling:
                proj = GlobalVariables.ProjectilePool.GetUnusedProjectile();
                proj.transform.SetParent(this.transform);
                break;
            case ProjectileHandlers.Entities:
                Vector3 eRot = localRot.Value + this.transform.localEulerAngles;
                Vector3 inheritPos = this.transform.position;
                Vector3 ePos = inheritPos + localPos.Value;

                float theta = Mathf.Deg2Rad * this.transform.localEulerAngles.z;
                ePos.x = (Mathf.Cos(theta) * localPos.Value.x) - (Mathf.Sin(theta) * localPos.Value.y) + inheritPos.x;
                ePos.y = (Mathf.Sin(theta) * localPos.Value.x) + (Mathf.Cos(theta) * localPos.Value.y) + inheritPos.y;

                GlobalVariables.EntityHandler.Spawn(ePos, Quaternion.Euler(eRot), projScale.x * projScaleMultiplier, 
                    projSpeed * projSpeedMultiplier, projShape, OuterBodyColor);
                
                return null;
            default:
                switch (attackType) {
                    case AttackType.Laser:
                        proj = Instantiate(laserPrefab, this.transform);
                        break;
                    default:
                        proj = Instantiate(projectilePrefab, this.transform);
                        break;
                }
                break;
        }
        
        Transform pTrans = proj.transform;
        // apply transforms
        pTrans.localEulerAngles = localRot.Value;
        if (attackType == AttackType.Projectile || projectileHandler == ProjectileHandlers.Pooling) {
            pTrans.localScale = new Vector3(projScale.x * projScaleMultiplier, 
                projScale.y * projScaleMultiplier, 1.0f);
            pTrans.localPosition = localPos.Value;
            proj.SetSprite(projSprite);
            
            // return object to the pool
            if (projectileHandler == ProjectileHandlers.Pooling)
                pTrans.SetParent(GlobalVariables.ProjectilePool.transform);
            else 
                pTrans.SetParent(null);
        } 
        else if (attackType == AttackType.Laser) {
            pTrans.localScale = new Vector3(projScale.x * projScaleMultiplier, 1.0f, 1.0f);
            pTrans.localPosition = new Vector3(0.0f, 0.0f, 0.01f); // centred, behind body
        }
        
        // initialise projectile
        proj.Init(projSpeed * projSpeedMultiplier);
        proj.SetColors(bodyColors);
        
        // if pooling, set object to being used by making it active
        if (projectileHandler == ProjectileHandlers.Pooling)
            proj.gameObject.SetActive(true);
        
        return proj;
    }
}