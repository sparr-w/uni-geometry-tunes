using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Shapes {
    Circle,
    Triangle,
    Square
}

[System.Serializable]
public struct ProjectileShape {
    public Shapes Shape;
    public Sprite Sprite;
}

public class EnemyHandler : MonoBehaviour {
    [SerializeField] private ProjectileShape[] projectileShapes;

    [Header("Enemy Prefabs")] [SerializeField] private Transform WormPrefab;
    [SerializeField] private Transform SpinShootPrefab, TurretPrefab, BossPrefab;

    private bool spawnRandom = true;
    public bool SpawnRandom {
        get { return spawnRandom; } set { spawnRandom = value; }
    }
    private Vector2 spawnLowerBounds, spawnUpperBounds;
    public Vector2 RandomSpawnLowerBounds {
        get { return spawnLowerBounds; }
    }
    public Vector2 RandomSpawnUpperBounds {
        get { return spawnUpperBounds; }
    }
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
    public Vector2 ExactSpawnLocation {
        get { return spawnLocation; }
    }
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
    
    private readonly float colourSaturation = 0.7f;
    private readonly float colourBrightnessInner = 0.4f;
    private readonly float colourBrightnessOuter = 0.7f;

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
    public string EnemySpeedMultiplier {
        get { return "" + enemySpeedMultiplier; }
        set {
            if (Single.TryParse(value, out float i)) {
                if (i < 0.0f) // don't accept negative values
                    enemySpeedMultiplier = 1.0f;
                else enemySpeedMultiplier = i;
            }
            else enemySpeedMultiplier = 1.0f;
        }
    }

    #endregion

    #region Global Projectile Variables

    private float projShotDelay = 0.2f;
    public string ProjShotDelay {
        get { return "" + projShotDelay; }
        set {
            if (Single.TryParse(value, out float i)) {
                if (i < 0.0f) // can't enter negative value
                    projShotDelay = 0.2f;
                else projShotDelay = i;
            }
            else projShotDelay = 0.2f;
        }
    }
    
    private float projSpeed = 1.0f;
    public string ProjSpeed {
        get { return "" + projSpeed; }
        set {
            if (Single.TryParse(value, out float i)) {
                if (i < 0.0f) // no negative
                    projSpeed = 1.0f;
                else projSpeed = i;
            }
            else projSpeed = 0.2f;
        }
    }
    
    private float projSize = 1.0f;
    public string ProjSize {
        get { return "" + projSize; }
        set {
            if (Single.TryParse(value, out float i)) {
                if (i < 0.0f)
                    projSize = 1.0f;
                else projSize = i;
            }
            else projSize = 1.0f;
        }
    }

    private AttackType shooterAttackType = AttackType.Projectile;
    public Int32 DropdownAttackType {
        set {
            if (value == 0) shooterAttackType = AttackType.Projectile;
            else if (value == 1) shooterAttackType = AttackType.Laser;
        }
    }

    private ProjectileHandlers projectileHandler = ProjectileHandlers.Standard;
    public Int32 DropdownProjectileHandler {
        set {
            if (value == 0) projectileHandler = ProjectileHandlers.Standard;
            else if (value == 1) projectileHandler = ProjectileHandlers.Pooling;
            else if (value == 2) projectileHandler = ProjectileHandlers.Entities;
        }
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

    #endregion
    
    public Transform SpawnSpinningShooter(Vector2 spawnPos) {
        Transform newShooter = Instantiate(SpinShootPrefab);
        newShooter.transform.position = spawnPos;

        SpinShooterBehaviour behaviourComponent = newShooter.GetComponent<SpinShooterBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(spinnerDirections, spinnerRotationMultiplier);
        
        behaviourComponent.SetProjectileSprite(projectileShapes[2].Sprite);
        behaviourComponent.InitProjectiles(projShotDelay, projSpeed, projSize);
        behaviourComponent.InitShooterProfile(shooterAttackType, projectileHandler);

        return newShooter;
    }
    public void SpawnSpinShooter() { SpawnSpinningShooter(EnemySpawnPosition); }

    #region Turret Variables

    private float turretRotationSpeed = 0.0f;
    public string TurretRotationSpeed {
        get { return "" + turretRotationSpeed; }
        set {
            if (Single.TryParse(value, out float i))
                turretRotationSpeed = i;
            else turretRotationSpeed = 0.0f;
        }
    }

    #endregion
    
    public Transform SpawnTurret(Vector2 spawnPos) {
        Transform newShooter = Instantiate(TurretPrefab);
        newShooter.transform.position = spawnPos;
        
        TurretBehaviour behaviourComponent = newShooter.GetComponent<TurretBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(enemySpeedMultiplier ,turretRotationSpeed);

        behaviourComponent.SetProjectileSprite(projectileShapes[2].Sprite);
        behaviourComponent.InitProjectiles(projShotDelay, projSpeed, projSize);
        behaviourComponent.InitShooterProfile(shooterAttackType, projectileHandler);

        return newShooter;
    }
    public void SpawnTurret() { SpawnTurret(EnemySpawnPosition); }

    #region Worm Variables

    private float wormSineFreq = 1.0f;
    public string WormSineFrequency {
        get { return "" + wormSineFreq; }
        set {
            if (Single.TryParse(value, out float i))
                wormSineFreq = i;
            else wormSineFreq = 1.0f;
        }
    }

    private float wormSineAmp = 1.0f;
    public string WormSineAmplitude {
        get { return "" + wormSineAmp; }
        set {
            if (Single.TryParse(value, out float i))
                wormSineAmp = i;
            else wormSineAmp = 1.0f;
        }
    }

    private float wormBodyGap = 0.0f;
    public string WormBodyGap {
        get { return "" + wormBodyGap; }
        set {
            if (Single.TryParse(value, out float i))
                wormBodyGap = i;
            else wormBodyGap = 0.0f;
        }
    }
    
    private int wormBodyParts = 9;
    public string WormBodyParts {
        get { return "" + wormBodyParts; }
        set {
            if (Int32.TryParse(value, out int i))
                wormBodyParts = i;
            else wormBodyParts = 9;
        }
    }

    #endregion
    
    public Transform SpawnWorm(Vector2 spawnPos) {
        Transform newWorm = Instantiate(WormPrefab);
        newWorm.transform.position = spawnPos;
        
        WormBehaviour behaviourComponent = newWorm.GetComponent<WormBehaviour>();
        behaviourComponent.SetColor(BodyColors);
        behaviourComponent.Init(enemySpeedMultiplier, wormBodyGap, wormBodyParts);
        behaviourComponent.InitPath(wormSineFreq, wormSineAmp);

        return newWorm;
    }
    public void SpawnWorm() { SpawnWorm(EnemySpawnPosition); }
    
    public Transform SpawnBoss(Vector2 spawnPos) {
        Transform newBoss = Instantiate(BossPrefab);
        newBoss.transform.position = spawnPos;
        
        BossEnemy bossComponent = newBoss.GetComponent<BossEnemy>();
        bossComponent.SetColor(BodyColors);

        return newBoss;
    }
}
