using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour {
    public bool isFiring = true;
    
    [Header("Shooter Behaviour")]
    public float RotationSpeed = 0.1f;
    
    [Header("Shooting Behaviour")]
    public Projectile ProjectileType;
    public float ProjectileSpeed = 3.5f;
    public Vector2 ProjectileScale = new Vector2(.3f, .3f);
    public float ShotDelay = 0.2f;
    [SerializeField] private bool shootLeft, shootRight, shootUp, shootDown;

    void Start() {
        StartCoroutine(Shoot());
        
        if (shootUp) transform.GetChild(0).gameObject.SetActive(true);
        else transform.GetChild(0).gameObject.SetActive(false);
        
        if (shootLeft) transform.GetChild(1).gameObject.SetActive(true);
        else transform.GetChild(1).gameObject.SetActive(false);
        
        if (shootDown) transform.GetChild(2).gameObject.SetActive(true);
        else transform.GetChild(2).gameObject.SetActive(false);
        
        if (shootRight) transform.GetChild(3).gameObject.SetActive(true);
        else transform.GetChild(3).gameObject.SetActive(false);
    }

    private IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            if (shootUp) FireProjectile(new Vector3(0.0f, 0.5f, 0.0f));
            if (shootLeft) FireProjectile(new Vector3(-0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 90.0f));
            if (shootDown) FireProjectile(new Vector3(0.0f, -0.5f, 0.0f), new Vector3(0.0f, 0.0f, 180.0f));
            if (shootRight) FireProjectile(new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 270.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    private Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) {
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

    private void Move() {
        this.transform.eulerAngles += Vector3.forward * RotationSpeed;
    }

    void Update() {
        Move();
    }
}