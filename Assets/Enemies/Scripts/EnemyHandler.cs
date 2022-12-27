using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHandler : MonoBehaviour {
    public PlayerController[] Players;
    [Header("Enemy Prefabs")] [SerializeField] private Transform WormPrefab;
    [SerializeField] private Transform SpinShootPrefab, TurretPrefab, BossPrefab;
    
    private float colourHue = 1.0f;
    private float colourSaturation = 0.7f;
    private float colourBrightnessInner = 0.4f, colourBrightnessOuter = 0.7f;
    
    private bool colourOverride = false;
    public bool ColourOverride {
        get { return colourOverride; } set { colourOverride = value; }
    }

    private float overriddenHue = 0.0f;
    public float OverriddenHue {
        get { return overriddenHue; } set { overriddenHue = value; }
    }

    private void Start() {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        
        if (p.Length != 0) {
            Players = new PlayerController[p.Length];

            for (int i = 0; i < p.Length; i++)
                Players[i] = p[i].GetComponent(typeof(PlayerController)) as PlayerController;
        }
    }

    private Color[] BodyColors { // [inner, outer]
        get {
            float hue = colourOverride ? overriddenHue : colourHue;
            Color[] colors = {
                Color.HSVToRGB(hue, colourSaturation, colourBrightnessInner),
                Color.HSVToRGB(hue, colourSaturation, colourBrightnessOuter),
            };

            return colors;
        }
    }

    #region Global Enemy Variables

    private float enemySpeedMultiplier = 1.0f;
    public float EnemySpeedMultiplier {
        get { return enemySpeedMultiplier; } set { enemySpeedMultiplier = value; }
    }

    #endregion

    #region Spinning Shooter Variables

    private bool[] spinnerDirections = { false, false, false, false }; // top, right, bottom, left -- like a clock
    public bool SpinnerShootUp {
        get { return spinnerDirections[0]; } set { spinnerDirections[0] = value; }
    }
    public bool SpinnerShootRight {
        get { return spinnerDirections[1]; } set { spinnerDirections[1] = value; }
    }
    public bool SpinnerShootDown {
        get { return spinnerDirections[2]; } set { spinnerDirections[2] = value; }
    }
    public bool SpinnerShootLeft {
        get { return spinnerDirections[3]; } set { spinnerDirections[3] = value; }
    }

    private float spinnerRotationMultiplier = 1.0f;
    public float SpinnerRotationMultiplier {
        get { return spinnerRotationMultiplier; } set { spinnerRotationMultiplier = value; }
    }

    private float spinnerShotDelay = 0.2f;
    public float SpinnerShotDelay {
        get { return spinnerShotDelay; } set { spinnerShotDelay = value; }
    }

    private float spinnerProjSpeed = 1.0f;
    public float SpinnerProjSpeed {
        get { return spinnerProjSpeed; } set { spinnerProjSpeed = value; }
    }

    private float spinnerProjSize = 1.0f;
    public float SpinnerProjSize {
        get { return spinnerProjSize; } set { spinnerProjSize = value; }
    }
    
    #endregion
    
    public void SpawnSpinner() { // gotta refactor spinning shooters so that it's easier to construct them
        float posX = Random.Range(-6.0f, 6.0f);
        float posY = Random.Range(-5.0f, 5.0f);
        SpawnSpinningShooter(new Vector2(posX, posY));
    }
    
    public Transform SpawnSpinningShooter(Vector2 spawnPos) {
        Transform newShooter = Instantiate(SpinShootPrefab);
        newShooter.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        SpinShooterBehaviour behaviourComponent = newShooter.GetComponent<SpinShooterBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(spinnerDirections, spinnerRotationMultiplier);
        behaviourComponent.InitProjectiles(spinnerShotDelay, spinnerProjSpeed, spinnerProjSize);

        return newShooter;
    }

    public Transform SpawnTurret(Vector2 spawnPos) {
        Transform newShooter = Instantiate(TurretPrefab);
        newShooter.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        TurretBehaviour behaviourComponent = newShooter.GetComponent<TurretBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(Players);

        return newShooter;
    }

    public Transform SpawnWorm(Vector2 spawnPos) {
        Transform newWorm = Instantiate(WormPrefab);
        newWorm.transform.position = spawnPos;
        
        colourHue = Random.Range(0.0f, 1.0f);
        
        WormBehaviour behaviourComponent = newWorm.GetComponent<WormBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init();

        return newWorm;
    }

    public Transform SpawnBoss(Vector2 spawnPos) {
        Transform newBoss = Instantiate(BossPrefab);
        newBoss.transform.position = spawnPos;

        colourHue = Random.Range(0.0f, 1.0f);

        BossEnemy bossComponent = newBoss.GetComponent<BossEnemy>();
        bossComponent.SetColor(BodyColors);

        return newBoss;
    }
}
