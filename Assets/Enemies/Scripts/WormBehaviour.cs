using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBehaviour : MonoBehaviour {
    [SerializeField] private float speed = 2.0f;
    [Header("Pathing Variables")]
    [SerializeField] private float sineFrequency = 1.0f;
    [SerializeField] private float sineAmplitude = 1.0f;
    [Header("Body Part Variables")]
    [SerializeField] private float bodyPartGap = 0.0f;
    [SerializeField] private int bodyPartCount = 9;

    private float posX = 0.0f, posY = 0.0f;
    private Vector2 startPos;

    private Transform[] bodyParts;
    private float bodyPartRadius = 1.0f;

    private void Move(Vector2 distance) {
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
    }

    void Start() {
        startPos = new Vector2(this.transform.position.x, this.transform.position.y);
        
        // collate body parts into an array
        bodyParts = new Transform[bodyPartCount];
        for (int i = 0; i < bodyPartCount; i++) {
            if (i >= transform.childCount) {
                GameObject newChild = Instantiate(transform.GetChild(0).gameObject, this.transform);
                bodyParts[i] = newChild.transform;
            } else bodyParts[i] = transform.GetChild(i).transform;
        }

        bodyPartRadius = bodyParts[0].localScale.x / 2;
    }

    void Update() {
        Move(new Vector2(speed * Time.deltaTime, 0.0f));
    }
}
