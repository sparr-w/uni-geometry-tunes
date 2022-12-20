using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBehaviour : Enemy {
    public bool isFiring = true;

    [Header("Shooting Behaviour")]
    public Projectile ProjectileType;
    public float ProjectileSpeed = 3.5f;
    public Vector2 ProjectileScale = new Vector2(.3f, .3f);
    public float ShotDelay = 0.2f;
    
    protected virtual IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            FireProjectile(new Vector3(0.0f, 0.5f, 0.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    protected Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) {
        if (localPos == null) localPos = new Vector3(0.0f, 0.0f, 0.0f);
        if (localRot == null) localRot = new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj = Instantiate(ProjectileType, this.transform);
        proj.Init(OuterBodyColor, ProjectileSpeed);
        
        proj.transform.localPosition = localPos.Value;
        proj.transform.localEulerAngles = localRot.Value;

        proj.transform.SetParent(null);
        proj.transform.localScale = new Vector3(ProjectileScale.x, ProjectileScale.y, 1.0f);

        return proj;
    }
}