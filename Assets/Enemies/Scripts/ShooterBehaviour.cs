using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBehaviour : Enemy {
    public bool isFiring = true;

    [Header("Shooting Behaviour")]
    [SerializeField] protected Projectile ProjectileType;
    [SerializeField] protected float ProjectileSpeed = 3.5f;
    [SerializeField] protected Vector2 ProjectileScale = new Vector2(.3f, .3f); 
    [SerializeField] protected float ShotDelay = 0.2f;
    
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
        
        proj.transform.localPosition = localPos.Value;
        proj.transform.localEulerAngles = localRot.Value;
        proj.transform.SetParent(null);
        if (proj is Laser) {
            proj.transform.localScale = new Vector3(ProjectileScale.x, 1.0f, 1.0f);
            proj.transform.SetParent(this.transform);
            proj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.01f); // centred, behind everything
        } else proj.transform.localScale = new Vector3(ProjectileScale.x, ProjectileScale.y, 1.0f);
        
        proj.Init(ProjectileSpeed);
        proj.SetColor(OuterBodyColor);

        return proj;
    }
}