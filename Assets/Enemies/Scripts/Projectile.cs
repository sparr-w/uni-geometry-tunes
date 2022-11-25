using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float Speed = 1.0f;
    public Vector2 Direction = new Vector2(0.0f, 1.0f);

    private void Move(Vector2 distance) {

    }

    private void Start() {}

    private void Update() {
        Move(Direction * Speed * Time.deltaTime);
    }
}