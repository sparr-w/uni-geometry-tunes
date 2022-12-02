using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed = 1.0f;

    private void Move(float distance) {
        this.transform.position += transform.up * distance;
    }

    private void Start() {
        
    }

    private void Update() {
        Move(Speed * Time.deltaTime);
    }
}