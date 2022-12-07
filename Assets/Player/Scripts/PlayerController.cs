using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float moveSpeed = 2.0f;
    private float rotSpeed = 250.0f;
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

        // rotate body towards direction of movement
        float bodyTargetRot = verticalInput > 0.25f ? 0.0f : verticalInput < -0.25f ? 180.0f : bodyComponent.transform.localEulerAngles.z;
        if (verticalInput == 0.0f)
            bodyTargetRot = horizontalInput > 0.25f ? 270.0f : horizontalInput < -0.25f ? 90.0f : bodyComponent.transform.localEulerAngles.z;
        else {
            if (horizontalInput > 0.25f) // silly billy fam
                bodyTargetRot = bodyTargetRot > 270.0f ? 225.0f : 315.0f;
            else if (horizontalInput < -0.25f)
                bodyTargetRot = bodyTargetRot > 90.0f ? 135.0f : 45.0f;
        }

        if (verticalInput != 0.0f || horizontalInput != 0.0f) {
            float bodyRot = bodyComponent.transform.localEulerAngles.z;
            
            if (bodyComponent.transform.localEulerAngles.z > bodyTargetRot)
                bodyRot = bodyRot - rotSpeed * Time.deltaTime;
            else if (bodyComponent.transform.localEulerAngles.z < bodyTargetRot)
                bodyRot = bodyRot + rotSpeed * Time.deltaTime;

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