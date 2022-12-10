using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float moveSpeed = 5.0f;
    private float rotSpeed = 400.0f;
    private float horizontalInput = 0.0f, verticalInput = 0.0f;
    private Transform bodyComponent;

    private void Start() {
        bodyComponent = transform.Find("Body");
    }

    private void GetInput() {
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
    
    private void Update() {
        GetInput();
        
        Move(new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 
            verticalInput * moveSpeed * Time.deltaTime));
    }
}