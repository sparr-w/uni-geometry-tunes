using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] protected SpriteRenderer[] OuterBodyParts, InnerBodyParts;

    private Color outerBodyColor = Color.white;
    public Color OuterBodyColor {
        set { this.outerBodyColor = value; }
        get { return this.outerBodyColor; }
    }

    [Space(10)]
    [SerializeField] protected float moveSpeedMultiplier = 0.0f;
    protected float moveSpeed = 0.0f;

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

    protected Vector3 MovePatternChase(Vector3 targetPos, float minDistance = 10.0f) {
        float distance = Vector2.Distance(this.transform.position, targetPos);
        
        

        return this.transform.position;
    }
    
    protected virtual Vector3 Move(Vector2 distance) {
        this.transform.position += new Vector3(distance.x, distance.y, 0.0f);
        return this.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) { // most enemies will have a trigger collider, so when the player crashes into them, he should take damage
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.DealDamage(1);
        }
    }
}