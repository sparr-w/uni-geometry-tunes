using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBehaviour : MonoBehaviour {
    [SerializeField] private Vector2 startPos = new Vector2(-100.0f, 0.0f);
    [SerializeField] private Vector2 endPos = new Vector2(50.0f, 0.0f);

    private float posX = 0.0f;
    private float posY = 0.0f;

    private Transform[] bodyParts;
    private float bodyPartRadius = 1.0f;
    private float partXoffset = 1.0f;

    private void Move(float distance, float frequency, float amplitude) {
        posX += distance;
        posY = Mathf.Sin(posX * frequency) * amplitude;

        this.transform.position = new Vector3(startPos.x + posX, startPos.y + posY, 0.0f);

        float partX = 0.0f;
        float partY = 0.0f;

        for (int i = 1; i < bodyParts.Length; i++) {
            partX = posX - (bodyPartRadius * partXoffset * i);
            partY = Mathf.Sin(partX * frequency) * amplitude;
            
            bodyParts[i].transform.position = new Vector3(startPos.x + partX, startPos.y + partY, 0.0f);
        }
    }

    void Start() {
        bodyParts = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) 
            bodyParts[i] = transform.GetChild(i).transform;

        bodyPartRadius = bodyParts[0].localScale.x / 2;

        Move(0.0f, 0.0f, 0.0f);
    }

    void Update() {
        Move(2.0f * Time.deltaTime, 1.0f, 1.0f);
    }
}
