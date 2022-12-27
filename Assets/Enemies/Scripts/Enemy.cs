using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] protected SpriteRenderer[] OuterBodyParts, InnerBodyParts;
    
    private Color outerBodyColor = Color.white;
    public Color OuterBodyColor {
        set { this.outerBodyColor = value; }
        get { return this.outerBodyColor; }
    }

    [Space(10)]
    [SerializeField] protected float moveSpeed = 0.0f;

    public bool SetColor(Color[] newColors) {
        if (OuterBodyParts.Length < 1) return false;

        foreach (SpriteRenderer part in OuterBodyParts)
            part.color = newColors[1];

        outerBodyColor = newColors[1];

        if (InnerBodyParts.Length > 0) {
            foreach (SpriteRenderer part in InnerBodyParts)
                part.color = newColors[0];
        }

        return true;
    }

    protected virtual Vector3 Move(Vector2 distance) {
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other) { // most enemies will have a trigger collider, so when the player crashes into them, he should take damage
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.DealDamage(1);
        }
    }
}