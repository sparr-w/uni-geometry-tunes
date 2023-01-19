using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBehaviour : Enemy {
    [Header("Shooter Variables")]
    public bool isFiring = true;

    [Header("Shooter Projectile Variables")]
    [SerializeField] protected Projectile ProjectileType;
    [SerializeField] protected float projSpeedMultiplier = 1.0f;
    [SerializeField] protected float projScaleMultiplier = 1.0f;
    [SerializeField] protected float ShotDelay = 0.2f;
    
    protected float projSpeed = 3.5f;
    protected Vector2 projScale = new Vector2(.3f, .3f);

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
    
    protected Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) {
        if (localPos == null) localPos = new Vector3(0.0f, 0.0f, 0.0f);
        if (localRot == null) localRot = new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj = Instantiate(ProjectileType, this.transform);
        
        proj.transform.localPosition = localPos.Value;
        proj.transform.localEulerAngles = localRot.Value;
        proj.transform.SetParent(null);
        if (proj is Laser) {
            proj.transform.localScale = new Vector3(projScale.x * projScaleMultiplier, 1.0f, 1.0f);
            proj.transform.SetParent(this.transform);
            proj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.01f); // centred, behind everything
        } else proj.transform.localScale = new Vector3(projScale.x * projScaleMultiplier, 
            projScale.y * projScaleMultiplier, 1.0f);
        
        proj.Init(projSpeed * projSpeedMultiplier);
        proj.SetColor(OuterBodyColor);

        return proj;
    }
}