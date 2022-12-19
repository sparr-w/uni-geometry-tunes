using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour {
    public PlayerController[] Players;
    public Transform WormPrefab;

    private float colourHue = 1.0f;
    private float colourSaturation = 0.7f;
    private float colourBrightnessInner = 0.4f, colourBrightnessOuter = 0.7f;

    private void Start() {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        
        if (p.Length != 0) {
            Players = new PlayerController[p.Length];

            for (int i = 0; i < p.Length; i++)
                Players[i] = p[i].GetComponent(typeof(PlayerController)) as PlayerController;
        }
        
        SpawnWorm();
    }

    public void SpawnTurret() {
        
    }

    public Transform SpawnWorm() {
        Transform newWorm = Instantiate(WormPrefab);

        WormBehaviour behaviourComponent = newWorm.GetComponent<WormBehaviour>();

        colourHue = Random.Range(0.0f, 1.0f);
        Color newColor = Color.HSVToRGB(colourHue, colourSaturation, colourBrightnessOuter);
        
        behaviourComponent.SetColor(newColor);

        return newWorm;
    }

    public void SpawnSpinningShooter() {
        
    }
}
