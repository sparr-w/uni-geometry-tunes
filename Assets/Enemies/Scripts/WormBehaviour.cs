using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBehaviour : Enemy {
    [Header("Pathing Variables")]
    [SerializeField] private float sineFrequency = 1.0f;
    [SerializeField] private float sineAmplitude = 1.0f;
    [Header("Body Part Variables")]
    [SerializeField] private float bodyPartGap = 0.0f;
    [SerializeField] private int bodyPartCount = 9;

    private float posX = 0.0f;
    private Vector2 startPos;
    private Transform[] bodyParts;
    private float bodyPartRadius = 1.0f;

    public WormBehaviour Init() {
        return this;
    }
    
    /* old method, only moves from left to right
    protected override Vector3 Move(Vector2 distance) {
        posX += distance.x;
        posY = Mathf.Sin(posX * sineFrequency) * sineAmplitude;

        this.transform.position = new Vector3(startPos.x + posX, startPos.y + posY, 0.0f);

        float partX = 0.0f;
        float partY = 0.0f;
        
        // body trail that follows the head - head is static to the object location
        for (int i = 1; i < bodyParts.Length; i++) {
            partX = posX - (bodyPartRadius * i) - (bodyPartGap * i);
            partY = Mathf.Sin(partX * sineFrequency) * sineAmplitude;
            
            bodyParts[i].transform.position = new Vector3(startPos.x + partX, startPos.y + partY, 0.0f);
        }

        return this.transform.position;
    }
    */

    protected override Vector3 Move(Vector2 distance) {
        posX += distance.x;
        
        Vector2 horizontalMomentum = this.transform.right * posX;
        Vector2 verticalMomentum = (Mathf.Sin(posX * sineFrequency) * sineAmplitude) * this.transform.up;
        
        this.transform.position = new Vector3(startPos.x + horizontalMomentum.x + verticalMomentum.x,
             startPos.y + horizontalMomentum.y + verticalMomentum.y, 0.0f);

        float partX = 0.0f;

        // body trail that follows the head - head is static to the object location
        for (int i = 1; i < bodyParts.Length; i++) {
            partX = (posX - (bodyPartRadius * i) - (bodyPartGap * i));

            horizontalMomentum = this.transform.right * partX;
            verticalMomentum = (Mathf.Sin(partX * sineFrequency) * sineAmplitude) * this.transform.up;

            bodyParts[i].transform.position = new Vector3(startPos.x + horizontalMomentum.x + verticalMomentum.x,
                startPos.y + horizontalMomentum.y + verticalMomentum.y, 0.0f);
        }
        
        return this.transform.position;
    }

    private void Start() {
        startPos = this.transform.position;
        
        OuterBodyParts = new SpriteRenderer[bodyPartCount];
        OuterBodyParts[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        // collate body parts into an array
        bodyParts = new Transform[bodyPartCount];
        for (int i = 0; i < bodyPartCount; i++) {
            if (i >= transform.childCount) {
                GameObject newChild = Instantiate(transform.GetChild(0).gameObject, this.transform);
                bodyParts[i] = newChild.transform;
                
                OuterBodyParts[i] = newChild.GetComponent<SpriteRenderer>();
            } else {
                bodyParts[i] = transform.GetChild(i).transform;

                OuterBodyParts[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
            }
        }

        bodyPartRadius = bodyParts[0].localScale.x / 2;
    }

    private void Update() {
        Move(new Vector2(moveSpeed * Time.deltaTime, 0.0f));
    }
}
