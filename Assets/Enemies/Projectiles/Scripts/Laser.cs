using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Laser : Projectile {
    [SerializeField] private float finalOpacity = 0.5f;
    [SerializeField] private float beamLifetime = 1.0f;

    private Transform beamComponent;
    private SpriteRenderer beamRendererComponent;
    private BoxCollider2D coll2D;
    
    private float chargeProgress = 0.0f;
    private const float beamHideTimeframe = 0.15f;

    private void Start() {
        beamComponent = this.transform.GetChild(0); // this should get the expanding beam part if the structure isn't tampered with
        beamRendererComponent = beamComponent.GetComponent<SpriteRenderer>();
        
        beamRendererComponent.color = new Color(beamRendererComponent.color.r,
            beamRendererComponent.color.g,
            beamRendererComponent.color.b,
            0.0f); // zero out the alpha by default

        coll2D = gameObject.GetComponent<BoxCollider2D>();
        coll2D.enabled = false; // ensure it's disabled from the get go
    }

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
    
    /* refactor this, if needed, so that the "shot delay" is the cooldown between the laser "blasting"
     * and the laser beginning to charge again, this way there won't be lasers repeatedly overlapping one another
     */

    private IEnumerator Blast() {
        StopCoroutine(nameof(Shoot));

        beamRendererComponent.color = bodyColors[1];
        coll2D.enabled = true; // finally, deal damage

        yield return new WaitForSeconds(beamLifetime);
        
        Destroy(this.gameObject);

        yield return null;
    }

    protected override void Update() {
        Charge(Speed * Time.deltaTime);
    }
}
