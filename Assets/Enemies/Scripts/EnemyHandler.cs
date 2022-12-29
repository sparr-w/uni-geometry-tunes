using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHandler : MonoBehaviour {
    [Header("Enemy Prefabs")] [SerializeField] private Transform WormPrefab;
    [SerializeField] private Transform SpinShootPrefab, TurretPrefab, BossPrefab;

    private bool spawnRandom = true;
    public bool SpawnRandom {
        get { return spawnRandom; } set { spawnRandom = value; }
    }
    private Vector2 spawnLowerBounds, spawnUpperBounds;
    public string SpawnLowerBoundsX {
        get { return spawnLowerBounds.x + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsX = Mathf.Abs(GlobalVariables.ScreenBounds.x);
                
                if (i >= spawnUpperBounds.x)
                    spawnLowerBounds = new Vector2(spawnUpperBounds.x, spawnLowerBounds.y);
                else if (i >= boundsX)
                    spawnLowerBounds = new Vector2(boundsX, spawnLowerBounds.y);
                else if (i <= -boundsX)
                    spawnLowerBounds = new Vector2(-boundsX, spawnLowerBounds.y);
                else
                    spawnLowerBounds = new Vector2(i, spawnLowerBounds.y);
            } else spawnLowerBounds = new Vector2(spawnLowerBounds.x, spawnLowerBounds.y);
        }
    }
    public string SpawnLowerBoundsY {
        get { return spawnLowerBounds.y + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsY = Mathf.Abs(GlobalVariables.ScreenBounds.y);

                if (i >= spawnUpperBounds.y)
                    spawnLowerBounds = new Vector2(spawnLowerBounds.x, spawnUpperBounds.y);
                else if (i >= boundsY)
                    spawnLowerBounds = new Vector2(spawnLowerBounds.x, boundsY);
                else if (i <= -boundsY)
                    spawnLowerBounds = new Vector2(spawnLowerBounds.x, -boundsY);
                else
                    spawnLowerBounds = new Vector2(spawnLowerBounds.x, i);
            } else spawnLowerBounds = new Vector2(spawnLowerBounds.x, spawnLowerBounds.y);
        }
    }
    public string SpawnUpperBoundsX {
        get { return spawnUpperBounds.x + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsX = Mathf.Abs(GlobalVariables.ScreenBounds.x);

                if (i <= spawnLowerBounds.x)
                    spawnUpperBounds = new Vector2(spawnLowerBounds.x, spawnUpperBounds.y);
                else if (i >= boundsX)
                    spawnUpperBounds = new Vector2(boundsX, spawnUpperBounds.y);
                else if (i <= -boundsX)
                    spawnUpperBounds = new Vector2(-boundsX, spawnUpperBounds.y);
                else
                    spawnUpperBounds = new Vector2(i, spawnUpperBounds.y);
            } else spawnUpperBounds = new Vector2(Mathf.Abs(GlobalVariables.ScreenBounds.x), spawnUpperBounds.y);
        }
    }
    public string SpawnUpperBoundsY {
        get { return spawnUpperBounds.y + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsY = Mathf.Abs(GlobalVariables.ScreenBounds.y);

                if (i <= spawnLowerBounds.y)
                    spawnUpperBounds = new Vector2(spawnUpperBounds.x, spawnLowerBounds.y);
                else if (i >= boundsY)
                    spawnUpperBounds = new Vector2(spawnUpperBounds.x, boundsY);
                else if (i <= -boundsY)
                    spawnUpperBounds = new Vector2(spawnUpperBounds.x, -boundsY);
                else
                    spawnUpperBounds = new Vector2(spawnUpperBounds.x, i);
            } else spawnUpperBounds = new Vector2(spawnUpperBounds.x, Mathf.Abs(GlobalVariables.ScreenBounds.y));
        }
    }
    private Vector2 spawnLocation = new Vector2(0.0f, 0.0f);
    public string SpawnLocationX {
        get { return spawnLocation.x + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsX = Mathf.Abs(GlobalVariables.ScreenBounds.x);
                
                if (i >= boundsX)
                    spawnLocation = new Vector2(boundsX, spawnLocation.y);
                else if (i <= -boundsX)
                    spawnLocation = new Vector2(-boundsX, spawnLocation.y);
                else
                    spawnLocation = new Vector2(i, spawnLocation.y);
            } else spawnLocation = new Vector2(0.0f, spawnLocation.y);
        }
    }
    public string SpawnLocationY {
        get { return spawnLocation.y + ""; }
        set {
            if (Single.TryParse(value, out float i)) {
                float boundsY = Mathf.Abs(GlobalVariables.ScreenBounds.y);

                if (i >= boundsY)
                    spawnLocation = new Vector2(spawnLocation.x, boundsY);
                else if (i <= -boundsY)
                    spawnLocation = new Vector2(spawnLocation.x, -boundsY);
                else
                    spawnLocation = new Vector2(spawnLocation.x, i);
            } else spawnLocation = new Vector2(spawnLocation.x, 0.0f);
        }
    }
    
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
        spawnLowerBounds = new Vector2(-(Mathf.Abs(GlobalVariables.ScreenBounds.x)),
            -(Mathf.Abs(GlobalVariables.ScreenBounds.y)));
        spawnUpperBounds = new Vector2(Mathf.Abs(GlobalVariables.ScreenBounds.x),
            Mathf.Abs(GlobalVariables.ScreenBounds.y));
    }

    private Color[] BodyColors { // [inner, outer]
        get {
            float hue = colourOverride ? overriddenHue : Random.Range(0.0f, 1.0f);
            Color[] colors = {
                Color.HSVToRGB(hue, colourSaturation, colourBrightnessInner),
                Color.HSVToRGB(hue, colourSaturation, colourBrightnessOuter),
            };

            return colors;
        }
    }

    private Vector2 EnemySpawnPosition {
        get {
            if (spawnRandom) {
                float posX = Random.Range(spawnLowerBounds.x, spawnUpperBounds.x);
                float posY = Random.Range(spawnLowerBounds.y, spawnUpperBounds.y);
                Vector2 loc = new Vector2(posX, posY);

                return loc;
            } else return spawnLocation;
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
    public string SpinnerRotationMultiplier {
        get { return "" + spinnerRotationMultiplier; }
        set {
            if (Single.TryParse(value, out float i))
                spinnerRotationMultiplier = i;
            else spinnerRotationMultiplier = 1.0f;
        }
    }
    
    private float spinnerShotDelay = 0.2f;
    public string SpinnerShotDelay {
        get { return "" + spinnerShotDelay; }
        set {
            if (Single.TryParse(value, out float i))
                spinnerShotDelay = i;
            else spinnerShotDelay = 0.2f;
        }
    }
    
    private float spinnerProjSpeed = 1.0f;
    public string SpinnerProjSpeed {
        get { return "" + spinnerProjSpeed; }
        set {
            if (Single.TryParse(value, out float i))
                spinnerProjSpeed = i;
            else spinnerProjSpeed = 0.2f;
        }
    }
    
    private float spinnerProjSize = 1.0f;
    public string SpinnerProjSize {
        get { return "" + spinnerProjSize; }
        set {
            if (Single.TryParse(value, out float i))
                spinnerProjSize = i;
            else spinnerProjSize = 0.2f;
        }
    }
    
    #endregion
    
    public Transform SpawnSpinningShooter(Vector2 spawnPos) { // gotta refactor spinning shooters so that it's easier to construct them
        Transform newShooter = Instantiate(SpinShootPrefab);
        newShooter.transform.position = spawnPos;

        SpinShooterBehaviour behaviourComponent = newShooter.GetComponent<SpinShooterBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(spinnerDirections, spinnerRotationMultiplier);
        behaviourComponent.InitProjectiles(spinnerShotDelay, spinnerProjSpeed, spinnerProjSize);

        return newShooter;
    }
    public void SpawnSpinShooter() { SpawnSpinningShooter(EnemySpawnPosition); }

    public Transform SpawnTurret(Vector2 spawnPos) {
        Transform newShooter = Instantiate(TurretPrefab);
        newShooter.transform.position = spawnPos;
        
        TurretBehaviour behaviourComponent = newShooter.GetComponent<TurretBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init();

        return newShooter;
    }

    public Transform SpawnWorm(Vector2 spawnPos) {
        Transform newWorm = Instantiate(WormPrefab);
        newWorm.transform.position = spawnPos;
        
        WormBehaviour behaviourComponent = newWorm.GetComponent<WormBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init();

        return newWorm;
    }

    public Transform SpawnBoss(Vector2 spawnPos) {
        Transform newBoss = Instantiate(BossPrefab);
        newBoss.transform.position = spawnPos;
        
        BossEnemy bossComponent = newBoss.GetComponent<BossEnemy>();
        bossComponent.SetColor(BodyColors);

        return newBoss;
    }
}
