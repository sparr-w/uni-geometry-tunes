using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Immunity Config")]
    [SerializeField] private GameObject immunityBubbleComponent;
    [SerializeField] private SpriteRenderer bubbleRenderer;
    [SerializeField] private float immunityDuration = 1.0f;

    private float moveSpeed = 5.0f, rotSpeed = 400.0f;
    private float horizontalInput = 0.0f, verticalInput = 0.0f;
    private int maxHitPoints = 10, hitPoints;
    private bool damageImmune = false;
    private float immunityElapsed = 0.0f;
    private Transform bodyComponent;
    
    private SpriteRenderer sRenderer;

    private void Start() {
        hitPoints = maxHitPoints;
        
        bodyComponent = transform.Find("Body");

        sRenderer = bodyComponent.GetComponent<SpriteRenderer>();

//        maskChunks = new Transform[maskChunksComponent.transform.childCount];
//        for (int i = 0; i < maskChunks.Length; i++) // should add TR, BR, BL, TL
//            maskChunks[i] = maskChunksComponent.transform.GetChild(i).GetChild(0);
    }

    private void GetInput() {
        if (hitPoints <= 0) { // stop receiving input and default to zero if the player is dead
            verticalInput = horizontalInput = 0.0f;
            return;
        }
        
        if (Input.GetKey(KeyCode.W)) verticalInput = 1.0f;
        else if (Input.GetKey(KeyCode.S)) verticalInput = -1.0f;
        else verticalInput = 0.0f;

        if (Input.GetKey(KeyCode.A)) horizontalInput = -1.0f;
        else if (Input.GetKey(KeyCode.D)) horizontalInput = 1.0f;
        else horizontalInput = 0.0f;
    }
    
    private Vector2 Move(Vector2? distance) {
        if (distance == null) return transform.position;

        this.transform.position = new Vector3(transform.position.x + distance.Value.x,
            transform.position.y + distance.Value.y,
            transform.position.z);

        float bodyRot = bodyComponent.transform.localEulerAngles.z;
        
        // rotate body towards direction of movement
        float bodyTargetRot = verticalInput > 0.25f ? 0.0f : 
            verticalInput < -0.25f ? 180.0f : 
            bodyRot;
        
        if (Mathf.Abs(verticalInput) < 0.2f)
            bodyTargetRot = horizontalInput > 0.25f ? 270.0f :
                horizontalInput < -0.25f ? 90.0f : 
                bodyRot;
        else
            bodyTargetRot = horizontalInput > 0.25f ? (bodyTargetRot + 270.0f) / 2 :
                horizontalInput < -0.25f ? (bodyTargetRot + 90.0f) / 2 : 
                bodyTargetRot;

        if (verticalInput > 0.25f && horizontalInput > 0.25f) bodyTargetRot = 315.0f; // special case
        
        /* simple rotate toward the target direction, doesn't take direction into account
        if (verticalInput != 0.0f || horizontalInput != 0.0f) {
            float bodyRot = bodyComponent.transform.localEulerAngles.z;

            if (bodyComponent.transform.localEulerAngles.z > bodyTargetRot) {
                if (bodyRot - rotSpeed * Time.deltaTime < bodyTargetRot)
                    bodyRot = bodyTargetRot;
                else
                    bodyRot = bodyRot - rotSpeed * Time.deltaTime;
            }
            else if (bodyComponent.transform.localEulerAngles.z < bodyTargetRot) {
                if (bodyRot + rotSpeed * Time.deltaTime > bodyTargetRot)
                    bodyRot = bodyTargetRot;
                else
                    bodyRot = bodyRot + rotSpeed * Time.deltaTime;
            }

            bodyComponent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, bodyRot);
        }
        */
        
        // rotate in the most optimal direction to face the direction of travel
        if (Mathf.Abs(verticalInput) > 0.2f || Mathf.Abs(horizontalInput) > 0.2f) {
            if ((bodyRot + 180.0f >= bodyTargetRot && bodyRot <= bodyTargetRot) ||
                (bodyRot + 180.0f >= bodyTargetRot + 360.0f && bodyRot <= bodyTargetRot + 360.0f))
                bodyRot = bodyRot + rotSpeed * Time.deltaTime;
                else
                bodyRot = bodyRot - rotSpeed * Time.deltaTime;

                bodyComponent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, bodyRot);
        }
        
        return transform.position;
    }

    private void SetImmunityState(bool newState) {
        immunityBubbleComponent.SetActive(newState);
        damageImmune = newState;
    }
    
    public bool DealDamage(DamageReport report) {
        if (damageImmune) return false; // can't deal damage to player, return that the hit wasn't received

        float c = hitPoints;
        
        if (report.AmountInflicted >= hitPoints) {
            hitPoints = 0;
            Die();
        }
        else {
            hitPoints -= report.AmountInflicted;
            SetImmunityState(true);
        }
        
        Debug.Log("Player taken (" + report.AmountInflicted + ") damage, previous health = " + c + ", remaining health = " + hitPoints);
        return true;
    }
    
//    private void UpdateChunks() {
//        for (int i = 0; i < maskChunks.Length; i++) {
//            float minChunk = 1.0f - (0.25f * (i + 1)); // min hp for this chunk
//            float percentageHP = (float)hitPoints / maxHitPoints;
//
//            float posY = percentageHP - minChunk;
//            posY = Mathf.Clamp(posY, 0.0f, 0.25f);
//            posY = (posY * 4) - 1.0f; 
//
//            if (posY <= -1.0f) {
//                maskChunks[i].gameObject.SetActive(false);
//            }
//            else {
//                maskChunks[i].gameObject.SetActive(true);
//                maskChunks[i].localPosition = new Vector3(0.0f, posY, 0.0f);
//            }
//        }
//    }
    
    private void Die() {
        // do some sort of simple death animation, explode? spin and shrink out of existence?
        damageImmune = true; // don't need to take more damage, prevent multiple calls of function
        sRenderer.enabled = false;
    }
    
    private void Update() {
        GetInput();
        
        Move(new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 
            verticalInput * moveSpeed * Time.deltaTime));

        if (damageImmune) {
            immunityElapsed += Time.deltaTime;
            
            if (immunityElapsed >= immunityDuration) {
                immunityElapsed = 0.0f;
                SetImmunityState(false);
            }
        }
    }
}