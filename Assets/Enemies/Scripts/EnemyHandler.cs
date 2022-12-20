using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour {
    public PlayerController[] Players;
    public Transform WormPrefab, SpinShootPrefab, TurretPrefab;

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
        
        SpawnWorm(new Vector2(-10.0f, 0.0f));
        SpawnSpinningShooter(new Vector2(0.0f, 0.0f));
        SpawnTurret(new Vector2(5.0f, 2.0f));
    }

    private Color InnerColor {
        get {
            Color newColor = Color.HSVToRGB(colourHue, colourSaturation, colourBrightnessInner);
            return newColor;
        }
    }

    private Color OuterColor {
        get {
            Color newColor = Color.HSVToRGB(colourHue, colourSaturation, colourBrightnessOuter);
            return newColor;
        }
    }

    public Transform SpawnTurret(Vector2 spawnPos) {
        Transform newShooter = Instantiate(TurretPrefab);
        newShooter.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        TurretBehaviour behaviourComponent = newShooter.GetComponent<TurretBehaviour>();
        behaviourComponent.SetColor(OuterColor, InnerColor);
        behaviourComponent.Init(Players);

        return newShooter;
    }

    public Transform SpawnWorm(Vector2 spawnPos) {
        Transform newWorm = Instantiate(WormPrefab);
        newWorm.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        WormBehaviour behaviourComponent = newWorm.GetComponent<WormBehaviour>();
        behaviourComponent.SetColor(OuterColor);
        behaviourComponent.Init();

        return newWorm;
    }

    public Transform SpawnSpinningShooter(Vector2 spawnPos) {
        Transform newShooter = Instantiate(SpinShootPrefab);
        newShooter.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        SpinShooterBehaviour behaviourComponent = newShooter.GetComponent<SpinShooterBehaviour>();
        behaviourComponent.SetColor(OuterColor, InnerColor);
        behaviourComponent.Init();

        return newShooter;
    }
}
