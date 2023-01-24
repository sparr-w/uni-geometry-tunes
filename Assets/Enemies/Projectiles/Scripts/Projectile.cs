using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed = 1.0f;

    protected Color parentColor = Color.white;

    private SpriteRenderer spriteRenderer;

    private ProjectilePool associatedPool; private bool pooledObject = false;

    public void SetAssociatedPool(ProjectilePool pool) {
        associatedPool = pool;
        pooledObject = true;
    }

    private void GetSpriteRenderer() { spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>(); }
    
    public void SetColor(Color newColor) {
        if (spriteRenderer == null) GetSpriteRenderer();
        
        spriteRenderer.color = parentColor = newColor;
    }

    public void SetSprite(Sprite newShape) {
        if (spriteRenderer == null) GetSpriteRenderer();

        spriteRenderer.sprite = newShape;
    }

    public Projectile Init(float speed = 1.0f) {
        this.Speed = speed;
        return this;
    }

    private void Move(float distance) {
        this.transform.position += transform.up * distance;
    }

    private void Update() {
        Move(Speed * Time.deltaTime);

        if (GlobalVariables.OutOfBoundsCheck(this.transform)) {
            if (pooledObject) associatedPool.ReturnUsedProjectile(this);
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