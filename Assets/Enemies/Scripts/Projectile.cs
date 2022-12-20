using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed = 1.0f;

    private SpriteRenderer spriteRenderer;

    public void SetColor(Color newColor) {
        if (spriteRenderer == null) spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        spriteRenderer.color = newColor;
    }

    public Projectile Init(float speed = 1.0f) {
        this.Speed = speed;
        return this;
    }

    public Projectile Init(Color newColor, float speed = 1.0f) {
        this.Speed = speed;
        SetColor(newColor);
        return this;
    }

    private void Move(float distance) {
        this.transform.position += transform.up * distance;
    }

    private void Update() {
        Move(Speed * Time.deltaTime);
    }
}