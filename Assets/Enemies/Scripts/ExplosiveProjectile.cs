using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveProjectile : Projectile {
    [SerializeField] private Projectile projectileType;
    [SerializeField] private int projectileCount = 0;

    private Vector2 screenBounds;

    private void Start() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
    
    private Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) { // copied behaviour from shooterbehaviour
        if (localPos == null) localPos = new Vector3(0.0f, 0.0f, 0.0f);
        if (localRot == null) localRot = new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj = Instantiate(projectileType, this.transform);
        proj.Init(3.0f);
        
        proj.transform.localPosition = localPos.Value;
        proj.transform.localEulerAngles = localRot.Value;

        proj.transform.SetParent(null);
        proj.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);

        return proj;
    }
    
    private void Explode() {
        if (projectileType == null) {
            Destroy(this.gameObject); // no behaviour just delete object
            return;
        }

        float projAngle = 360.0f / projectileCount;

        for (int i = 1; i <= projectileCount; i++) {
            FireProjectile(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, (projAngle * i)));
        }
        
        Destroy(this.gameObject); // all projectiles deployed, no need for this object
        return;
    }
    
    private void LateUpdate() {
        if (Mathf.Abs(transform.position.x) >= Mathf.Abs(screenBounds.x) ||
            Mathf.Abs(transform.position.y) >= Mathf.Abs(screenBounds.y))
            Explode();
    }
}
