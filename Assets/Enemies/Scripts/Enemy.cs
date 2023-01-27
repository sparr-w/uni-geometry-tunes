using System;
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

    private float lifeSpan = 0.0f;
    private float lifeElapsed = 0.0f;

    protected Color[] bodyColors = { Color.black, Color.white };

    [Space(10)]
    protected float moveSpeedMultiplier = 1.0f;
    [Header("Enemy Movement Variables")]
    [SerializeField] protected EnemyMovementPatterns movementPattern = EnemyMovementPatterns.Static;
    [SerializeField] protected Vector2 directionOfTravel = new Vector2(1.0f, 0.0f);
    [SerializeField] protected float distanceFromPlayer = 4.0f;
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

        bodyColors = newColors;
        
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

    public Enemy InitMovement(float speed, EnemyMovementPatterns pattern, float angle = 90.0f, float distFromPlayer = 4.0f) {
        this.moveSpeedMultiplier = speed;
        this.movementPattern = pattern;
        
        // convert angle to directional vector for move direction
        Vector2 dir;
        angle = Mathf.Deg2Rad * (90.0f - angle);

        dir.x = Mathf.Cos(angle);
        dir.y = Mathf.Sin(angle);

        this.directionOfTravel = dir.normalized;

        this.distanceFromPlayer = distFromPlayer;
        
        return this;
    }

    public void SetLifeSpan(float duration) {
        this.lifeSpan = duration;
    }
    protected void HandleLifeSpan() {
        if (lifeSpan > 0.0f) { // make sure that the enemy has a life span, otherwise it will live until another condition is met
            lifeElapsed += Time.deltaTime;
            
            if (lifeElapsed >= lifeSpan) Destroy(this.gameObject);
        }
    }
    
    protected virtual void Move() {
        switch (movementPattern) {
            case EnemyMovementPatterns.Static:
                break;
            case EnemyMovementPatterns.Direction:
                float distance = directionOfTravel.magnitude;
                Vector2 direction = directionOfTravel / distance;

                MovePatternDirection(direction * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
                break;
            case EnemyMovementPatterns.ChasePlayer:
                if (GlobalVariables.Players[0] != null)
                    MovePatternChase(GlobalVariables.Players[0].transform.position, distanceFromPlayer);
                else Debug.LogWarning("There are no players to target!");
                break;
        }
    }

    protected virtual void Update() {
        Move();
        
        if (GlobalVariables.OutOfBoundsCheck(transform))
            Destroy(this.gameObject);
        
        HandleLifeSpan();
    }

    protected virtual void AttackPlayer(PlayerController player) {
        DamageReport damageReport = new DamageReport(1, this.transform.position);
        player.DealDamage(damageReport);
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) { // most enemies will have a trigger collider, so when the player crashes into them, he should take damage
        if (other.CompareTag("Player")) {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            AttackPlayer(playerController);
        }
    }
}