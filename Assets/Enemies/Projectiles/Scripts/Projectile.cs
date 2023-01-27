using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed = 1.0f;

    protected Color[] bodyColors = {Color.black, Color.white};

    protected SpriteRenderer spriteRenderer;

    private bool pooledObject = false;
    public void SetPooledObject(bool newValue) {
        pooledObject = newValue;
    }

    protected virtual void GetSpriteRenderer() { spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>(); }
    
    public virtual void SetColors(Color[] newColors) {
        if (spriteRenderer == null) GetSpriteRenderer();

        spriteRenderer.color = newColors[1];
        bodyColors = newColors;
    }

    public virtual void SetSprite(Sprite newShape) {
        if (spriteRenderer == null) GetSpriteRenderer();

        spriteRenderer.sprite = newShape;
    }

    public virtual Projectile Init(float speed = 1.0f) {
        this.Speed = speed;
        return this;
    }

    protected void Move(float distance) {
        this.transform.position += transform.up * distance;
    }

    protected virtual void Update() {
        Move(Speed * Time.deltaTime);

        if (GlobalVariables.OutOfBoundsCheck(this.transform)) {
            if (pooledObject) GlobalVariables.ProjectilePool.ReturnUsedProjectile(this);
            else Destroy(this.gameObject);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            DamageReport damageReport = new DamageReport(1, this.transform.position);
            playerController.DealDamage(damageReport);
        }
    }
}