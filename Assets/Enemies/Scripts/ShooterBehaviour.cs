using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackType {
    Projectile,
    Laser
}

public enum ProjectileHandlers {
    Standard,
    Pooling
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
    private ProjectilePool projectilePool;

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

    public ShooterBehaviour SetProjectileSprite(Sprite newSprite) {
        this.projSprite = newSprite;
        return this;
    }

    protected override void Start() {
        projectilePool = FindObjectOfType<ProjectilePool>();
    }

    protected Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) {
        localPos ??= new Vector3(0.0f, 0.0f, 0.0f);
        localRot ??= new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj;
        Transform pTrans;
        
        switch (projectileHandler) {
            case ProjectileHandlers.Pooling:
                proj = projectilePool.GetUnusedProjectile();
                pTrans = proj.transform;
                pTrans.SetParent(this.transform);
                pTrans.localEulerAngles = localRot.Value;
                pTrans.localScale = new Vector3(projScale.x * projScaleMultiplier,
                    projScale.y * projScaleMultiplier, 1.0f);
                pTrans.localPosition = localPos.Value;
                pTrans.SetParent(projectilePool.transform);
                proj.SetSprite(projSprite);
                proj.Init(projSpeed * projSpeedMultiplier);
                proj.SetColor(OuterBodyColor);
                proj.gameObject.SetActive(true);
                
                break;
            default: // standard, default, same thing
                switch (attackType) {
                    case AttackType.Laser:
                        proj = Instantiate(laserPrefab, this.transform);
                        break;
                    default:
                        proj = Instantiate(projectilePrefab, this.transform);
                        break;
                }

                pTrans = proj.transform;
                pTrans.localEulerAngles = localRot.Value;
                switch (attackType) {
                    case AttackType.Laser:
                        pTrans.localScale = new Vector3(projScale.x * projScaleMultiplier, 1.0f, 1.0f);
                        pTrans.localPosition = new Vector3(0.0f, 0.0f, 0.01f); // centred, behind body
                        break;
                    default:
                        pTrans.localScale = new Vector3(projScale.x * projScaleMultiplier,
                            projScale.y * projScaleMultiplier, 1.0f);
                        pTrans.localPosition = localPos.Value;
                        pTrans.SetParent(null);
                        proj.SetSprite(projSprite);
                        break;
                }

                proj.Init(projSpeed * projSpeedMultiplier);
                proj.SetColor(OuterBodyColor);

                break;
        }

        return proj;
    }
}