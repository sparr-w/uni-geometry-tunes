using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile {
    [SerializeField] private float finalWidth = 0.5f;
    [SerializeField] private float finalOpacity = 0.5f;

    private Transform beamComponent;
    private SpriteRenderer beamRendererComponent;
    private Vector2 startPos;
    
    private float chargeProgress = 0.0f;

    private void Start() {
        beamComponent = this.transform.GetChild(0); // this should get the expanding beam part if the structure isn't tampered with
        beamRendererComponent = beamComponent.GetComponent<SpriteRenderer>();
        
        beamRendererComponent.color = new Color(beamRendererComponent.color.r,
            beamRendererComponent.color.g,
            beamRendererComponent.color.b,
            0.0f); // zero out the alpha by default
    }
    
    // appear during initialisation, in the correct position
    public Laser Init(Vector2 _startPos) {
        this.startPos = _startPos;
        return this;
    }
    
    // expand in size, becoming more opaque and clear it is going to "blast"
    private bool Charge(float increment) {
        chargeProgress += increment;
        chargeProgress = chargeProgress > 1.0f ? 1.0f : chargeProgress;

        float newWidth = finalWidth * chargeProgress;
        beamComponent.localScale = new Vector3(beamComponent.localScale.x,
            newWidth, beamComponent.localScale.z);

        float newOpacity = finalOpacity * chargeProgress;
        beamRendererComponent.color = new Color(beamRendererComponent.color.r,
            beamRendererComponent.color.g,
            beamRendererComponent.color.b,
            newOpacity);
        
        if (chargeProgress >= 1.0f) Shoot();
        return chargeProgress >= 1.0f;
    }
    
    // "blast", everything disappears briefly before a solid beam appears, dealing damage
    private void Shoot() {
        beamRendererComponent.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
    
    // the beam should stick the barrel it is firing from, so it may need to move with owner

    private void Update() {
        if (!Charge(Speed * Time.deltaTime)) ;
    }
}