using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public SpriteRenderer[] OuterBodyParts, InnerBodyParts;
    
    [Space(10)]
    [SerializeField] protected float speed = 0.0f;

    public virtual void Init() {}

    public bool SetColor(Color outerColor) {
        if (OuterBodyParts.Length < 1) return false;

        foreach (SpriteRenderer part in OuterBodyParts)
            part.color = outerColor;
        
        return true;
    }

    public bool SetColor(Color outerColor, Color innerColor) {
        if (OuterBodyParts.Length < 1) return false;

        foreach (SpriteRenderer part in OuterBodyParts)
            part.color = outerColor;

        if (InnerBodyParts.Length > 0) {
            foreach (SpriteRenderer part in InnerBodyParts)
                part.color = innerColor;
        }

        return true;
    }

    protected virtual Vector3 Move(Vector2 distance) {
        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}