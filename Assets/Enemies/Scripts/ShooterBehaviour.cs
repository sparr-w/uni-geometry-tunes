using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour {
    public Projectile ProjectileType;
    public float ProjectileSpeed = 3.5f;
    public Vector2 ProjectileScale = new Vector2(.3f, .3f);
    public float ShotDelay = 0.2f;
    public bool isFiring = true;

    void Start() {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            Projectile proj = Instantiate(ProjectileType);
            proj.transform.rotation = transform.rotation;
            proj.transform.localScale = new Vector3(ProjectileScale.x, ProjectileScale.y, 1.0f);
            proj.Speed = ProjectileSpeed;

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    private void Move() {
        this.transform.eulerAngles += Vector3.forward * 0.1f;
    }

    void Update() {
        Move();
    }
}