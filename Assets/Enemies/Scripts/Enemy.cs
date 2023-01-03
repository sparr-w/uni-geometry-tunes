using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyMovementPatterns {
    Static,
    Direction,
    ChasePlayer
}

public class Enemy : MonoBehaviour {
    [SerializeField] protected SpriteRenderer[] OuterBodyParts, InnerBodyParts;

    private Color outerBodyColor = Color.white;
    public Color OuterBodyColor {
        set { this.outerBodyColor = value; }
        get { return this.outerBodyColor; }
    }

    [Space(10)]
    protected float moveSpeedMultiplier = 1.0f;
    [SerializeField] protected float moveSpeed = 0.0f;
    protected EnemyMovementPatterns movementPattern = EnemyMovementPatterns.Static;

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

    protected virtual Vector3 MovePatternChase(Vector3 targetPos, float minDistance = 3.0f) {
        Vector3 heading = targetPos - this.transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        direction.z = 0.0f;

        Vector3 newPos = this.transform.position;

        if (distance >= minDistance) { // the enemy is too far away from its target, time to move closer
            if (moveSpeed * moveSpeedMultiplier >= Mathf.Abs(distance - minDistance))
                newPos += (direction * Mathf.Abs(distance - minDistance) * Time.deltaTime);
            else
                newPos += (direction * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
        } else if (distance <= minDistance) { // the enemy is too close to its target, time to retreat
            if (moveSpeed * moveSpeedMultiplier >= Mathf.Abs(minDistance - distance))
                newPos -= (direction * Mathf.Abs(minDistance - distance) * Time.deltaTime);
            else
                newPos -= (direction * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
        }

        this.transform.position = newPos;
        return newPos;
    }

    protected virtual Vector3 MovePatternDirection(Vector2 distance) {
        this.transform.position += new Vector3(distance.x, distance.y, 0.0f);
        return this.transform.position;
    }

    protected virtual void Move() {
        switch (movementPattern) {
            case EnemyMovementPatterns.Static:
                break;
            case EnemyMovementPatterns.Direction:
                MovePatternDirection(new Vector2(Time.deltaTime * 5.0f, 0.0f));
                break;
            case EnemyMovementPatterns.ChasePlayer:
                if (GlobalVariables.Players[0] != null)
                    MovePatternChase(GlobalVariables.Players[0].transform.position);
                else Debug.LogWarning("There are no players to target!");
                break;
        }
    }

    protected virtual void Update() {
        Move();
    }
    
    private void OnTriggerEnter2D(Collider2D other) { // most enemies will have a trigger collider, so when the player crashes into them, he should take damage
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.DealDamage(1);
        }
    }
}