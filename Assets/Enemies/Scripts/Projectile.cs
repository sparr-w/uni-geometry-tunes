using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private float speed = 1.0f;

    public Projectile Init(float speed = 1.0f) {
        this.speed = speed;
        return this;
    }

    private void Move(float distance) {
        this.transform.position += transform.up * distance;
    }

    private void Start() {
        
    }

    private void Update() {
        Move(speed * Time.deltaTime);
    }
}