using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinShooterBehaviour : ShooterBehaviour {
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private bool shootLeft, shootRight, shootUp, shootDown;

    private void ShowHideBarrels() {
        if (shootUp) transform.GetChild(0).gameObject.SetActive(true);
        else transform.GetChild(0).gameObject.SetActive(false);
        
        if (shootLeft) transform.GetChild(1).gameObject.SetActive(true);
        else transform.GetChild(1).gameObject.SetActive(false);
        
        if (shootDown) transform.GetChild(2).gameObject.SetActive(true);
        else transform.GetChild(2).gameObject.SetActive(false);
        
        if (shootRight) transform.GetChild(3).gameObject.SetActive(true);
        else transform.GetChild(3).gameObject.SetActive(false);
    }

    #region Initializers

    public SpinShooterBehaviour InitProjectiles(float shotDelay = 0.2f, float projSpeed = 1.0f, float projSize = 1.0f) {
        this.ShotDelay = shotDelay;
        
        return this;
    }

    public SpinShooterBehaviour Init(bool[] shotDirections, float spinSpeed = 1.0f) {
        this.rotationSpeed = spinSpeed * rotationSpeed; // refactor
        shootUp = shotDirections[0];
        shootRight = shotDirections[1];
        shootDown = shotDirections[2];
        shootLeft = shotDirections[3];
        
        StartCoroutine(Shoot());
        
        return this;
    }

    #endregion

    private void Start() {
        ShowHideBarrels();
    }

    protected override IEnumerator Shoot() {
        yield return 0;
        
        while (isFiring) {
            if (shootUp) FireProjectile(new Vector3(0.0f, 0.5f, 0.0f));
            if (shootLeft) FireProjectile(new Vector3(-0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 90.0f));
            if (shootDown) FireProjectile(new Vector3(0.0f, -0.5f, 0.0f), new Vector3(0.0f, 0.0f, 180.0f));
            if (shootRight) FireProjectile(new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 270.0f));

            yield return new WaitForSeconds(ShotDelay);
        }
    }

    protected override Vector3 Move(Vector2 distance) {
        // spin around
        this.transform.localEulerAngles += Vector3.forward * rotationSpeed;

        // move (if needs to move)
        this.transform.position += new Vector3(distance.x, distance.y, 0.0f);
        
        return this.transform.position;
    }

    private void Update() {
        Move(new Vector2(0.0f, 0.0f) * moveSpeed * Time.deltaTime);
    }
}
