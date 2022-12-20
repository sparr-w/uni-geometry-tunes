using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile {
    [SerializeField] private float finalOpacity = 0.5f;
    [SerializeField] private float beamLifetime = 1.0f;

    private Transform beamComponent;
    private SpriteRenderer beamRendererComponent;
    
    private float chargeProgress = 0.0f;
    private const float beamHideTimeframe = 0.15f;

    private void Start() {
        beamComponent = this.transform.GetChild(0); // this should get the expanding beam part if the structure isn't tampered with
        beamRendererComponent = beamComponent.GetComponent<SpriteRenderer>();
        
        beamRendererComponent.color = new Color(beamRendererComponent.color.r,
            beamRendererComponent.color.g,
            beamRendererComponent.color.b,
            0.0f); // zero out the alpha by default
    }
    
    // appear during initialisation, in the correct position

    // expand in size, becoming more opaque and clear it is going to "blast"
    private bool Charge(float increment) {
        if (chargeProgress >= 1.0f) return true;
        
        chargeProgress += increment;
        chargeProgress = chargeProgress > 1.0f ? 1.0f : chargeProgress;
        
        beamComponent.localScale = new Vector3(chargeProgress, beamComponent.localScale.y, beamComponent.localScale.z);

        float newOpacity = finalOpacity * chargeProgress;
        beamRendererComponent.color = new Color(beamRendererComponent.color.r,
            beamRendererComponent.color.g,
            beamRendererComponent.color.b,
            newOpacity);
        
        if (chargeProgress >= 1.0f) StartCoroutine(nameof(Shoot));
        return false;
    }
    
    // "blast", everything disappears briefly before a solid beam appears, dealing damage
    private IEnumerator Shoot() {
        beamRendererComponent.color = new Color(0.0f, 0.0f, 0.0f, 0.0f); // hide the beam temporarily

        yield return new WaitForSeconds(beamHideTimeframe);

        StartCoroutine(nameof(Blast));

        yield return null;
    }

    private IEnumerator Blast() {
        StopCoroutine(nameof(Shoot));

        beamRendererComponent.color = parentColor;

        yield return new WaitForSeconds(beamLifetime);
        
        Destroy(this.gameObject);

        yield return null;
    }
    
    // the beam should stick the barrel it is firing from, so it may need to move with owner

    private void Update() {
        if (!Charge(Speed * Time.deltaTime)) ;
    }
}
