using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour {
    public bool isFiring = true;

    [Header("Shooting Behaviour")]
    public Projectile ProjectileType;
    public float ProjectileSpeed = 3.5f;
    public Vector2 ProjectileScale = new Vector2(.3f, .3f);
    public float ShotDelay = 0.2f;
    
    public virtual IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            FireProjectile(new Vector3(0.0f, 0.5f, 0.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    protected Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) { // could make global?
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

    public virtual Vector2 Move(Vector2? distance) {
        if (distance == null) distance = new Vector2(0.0f, 0.0f);

        Vector3 newPos = new Vector3(this.transform.position.x + distance.Value.x,
            this.transform.position.y + distance.Value.y,
            this.transform.position.z);

        return this.transform.position = newPos;
    }
}