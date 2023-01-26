using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SpawnConfigComponents {
    public GameObject RandomSpawnComponent, ExactSpawnComponent;
    
    public TMP_InputField ExactSpawnX, ExactSpawnY;
    public TMP_InputField RandomLowerX, RandomLowerY;
    public TMP_InputField RandomUpperX, RandomUpperY;

    public GameObject SpawnVisualComponent, RandomSpawnVisual, ExactSpawnVisual;
}

[System.Serializable]
public struct ShooterProjectileFields {
    public TMP_InputField ShotDelay, SpeedMultiplier, SizeMultiplier;
}

[System.Serializable]
public struct EnemyConfigComponents {
    public TMP_Dropdown EnemySelector;

    public GameObject MovementPatternDropdown;

    public GameObject DistanceFromPlayerField, MoveDirectionField;

    public GameObject[] EnemyVariableComponents;
}

public class TestPanel : MonoBehaviour {
    [SerializeField] private EnemyHandler _enemyHandler;
    [Header("Components")]
    [SerializeField] private Image colourHandle;
    [SerializeField] private TMP_InputField speedMultiplierField;
    [SerializeField] private SpawnConfigComponents spawnConfigComponents;
    [SerializeField] private ShooterProjectileFields shooterProjectileFields;
    [SerializeField] private EnemyConfigComponents enemyConfigComponents;
    private int enemyVarsEnabled = 0;
    
    private float defaultSpeed = 200.0f;
    private bool isActive = false, isMoving = false;
    private int direction = 0;
    private RectTransform rectTransform;

    private Vector2 curPos {
        get { return rectTransform.localPosition; }
    }

    private void Start() {
        rectTransform = this.GetComponent<RectTransform>();

        HandleHue(0.0f);
        
        foreach (GameObject vars in enemyConfigComponents.EnemyVariableComponents) vars.SetActive(false);
        enemyConfigComponents.EnemyVariableComponents[enemyVarsEnabled].SetActive(true);
    }

    private void Toggle(bool fromLeft = true) {
        if (isMoving) return;

        if (isActive) {
            if (rectTransform.localPosition.x > 0 && !fromLeft) direction = 1;
            else if (rectTransform.localPosition.x < 0 && fromLeft) direction = -1;
            else return;

            isMoving = true;
            isActive = false;

            StartCoroutine(HidePanel());
        }
        else {
            if (fromLeft) {
                rectTransform.localPosition = new Vector2(-(970.0f + rectTransform.sizeDelta.x), curPos.y);
                direction = 1;
            }
            else {
                rectTransform.localPosition = new Vector2(970.0f, curPos.y);
                direction = -1;
            }

            isMoving = true;
            
            UpdateExactSpawn();
            UpdateLowerBounds();
            UpdateUpperBounds();
            
            StartCoroutine(ShowPanel());
        }
    }

    private IEnumerator HidePanel() {
        if (direction > 0) {
            while (rectTransform.localPosition.x < 970.0f) {
                float posX = curPos.x + (direction * defaultSpeed * Time.deltaTime);

                if (posX >= 970.0f) {
                    posX = 970.0f;
                    rectTransform.localPosition = new Vector2(posX, curPos.y);
                    
                    isMoving = false;
                    direction = 0;
                    StopCoroutine(HidePanel());
                }
                else {
                    rectTransform.localPosition = new Vector2(posX, curPos.y);
                    yield return null;
                }
            }
        }
        else {
            while (rectTransform.localPosition.x > -(970.0f + rectTransform.sizeDelta.x)) {
                float posX = curPos.x + (direction * defaultSpeed * Time.deltaTime);

                if (posX <= -(970.0f + rectTransform.sizeDelta.x)) {
                    posX = -(970.0f + rectTransform.sizeDelta.x);
                    rectTransform.localPosition = new Vector2(posX, curPos.y);

                    isMoving = false;
                    direction = 0;
                    StopCoroutine(HidePanel());
                }
                else {
                    rectTransform.localPosition = new Vector2(posX, curPos.y);
                    yield return null;
                }
            }
        }
    }

    private IEnumerator ShowPanel() {
        if (direction > 0) {
            while (rectTransform.localPosition.x < -960.0f) {
                float posX = curPos.x + (direction * defaultSpeed * Time.deltaTime);

                if (posX >= -960.0f) {
                    posX = -960.0f;
                    rectTransform.localPosition = new Vector2(posX, curPos.y);

                    isMoving = false;
                    isActive = true;
                    direction = 0;
                    StopCoroutine(ShowPanel());
                }
                else {
                    rectTransform.localPosition = new Vector2(posX, curPos.y);
                    yield return null;
                }
            }
        }
        else {
            while (rectTransform.localPosition.x > (960.0f - rectTransform.sizeDelta.x)) {
                float posX = curPos.x + (direction * defaultSpeed * Time.deltaTime);

                if (posX <= (960.0f - rectTransform.sizeDelta.x)) {
                    posX = (960.0f - rectTransform.sizeDelta.x);
                    rectTransform.localPosition = new Vector2(posX, curPos.y);

                    isMoving = false;
                    isActive = true;
                    direction = 0;
                    StopCoroutine(ShowPanel());
                }
                else {
                    rectTransform.localPosition = new Vector2(posX, curPos.y);
                    yield return null;
                }
            }
        }
    }

    public void HandleHue(float value) {
        colourHandle.color = Color.HSVToRGB(value, 0.7f, 0.7f);
    }

    public void ToggleSpawnModifier(bool newValue) {
        if (newValue == true) {
            spawnConfigComponents.RandomSpawnComponent.SetActive(true);
            spawnConfigComponents.ExactSpawnComponent.SetActive(false);
        }
        else {
            spawnConfigComponents.RandomSpawnComponent.SetActive(false);
            spawnConfigComponents.ExactSpawnComponent.SetActive(true);
        }
    }

    #region Update UI after Data Validation
    
    public void UpdateExactSpawn() {
        spawnConfigComponents.ExactSpawnX.text = _enemyHandler.SpawnLocationX;
        spawnConfigComponents.ExactSpawnY.text = _enemyHandler.SpawnLocationY;
    }

    public void UpdateLowerBounds() {
        spawnConfigComponents.RandomLowerX.text = _enemyHandler.SpawnLowerBoundsX;
        spawnConfigComponents.RandomLowerY.text = _enemyHandler.SpawnLowerBoundsY;
    }

    public void UpdateUpperBounds() {
        spawnConfigComponents.RandomUpperX.text = _enemyHandler.SpawnUpperBoundsX;
        spawnConfigComponents.RandomUpperY.text = _enemyHandler.SpawnUpperBoundsY;
    }

    public void UpdateEnemySpeedMultiplier() {
        speedMultiplierField.text = _enemyHandler.EnemySpeedMultiplier;
    }

    public void UpdateProjectileShotDelay() {
        shooterProjectileFields.ShotDelay.text = _enemyHandler.ProjShotDelay;
    }

    public void UpdateProjectileSpeedMultiplier() {
        shooterProjectileFields.SpeedMultiplier.text = _enemyHandler.ProjSpeed;
    }

    public void UpdateProjectileSizeMultiplier() {
        shooterProjectileFields.SizeMultiplier.text = _enemyHandler.ProjSize;
    }
    
    #endregion
    
    public void ShowCorrectSpawnVisual() {
        if (spawnConfigComponents.RandomSpawnComponent.activeSelf) {
            spawnConfigComponents.RandomSpawnVisual.SetActive(true);
            spawnConfigComponents.ExactSpawnVisual.SetActive(false);
        }
        else {
            spawnConfigComponents.RandomSpawnVisual.SetActive(false);
            spawnConfigComponents.ExactSpawnVisual.SetActive(true);
        }
    }
    
    public void ToggleEnemySpawnVisual(bool newState) {
        spawnConfigComponents.SpawnVisualComponent.SetActive(newState);
        if (newState == false) return;

        ShowCorrectSpawnVisual();
    }

    public void UpdateRandomSpawnVisual() {
        // need to access numerous variables, no point in taking in any
        float width = _enemyHandler.RandomSpawnUpperBounds.x - _enemyHandler.RandomSpawnLowerBounds.x; // diffference
        width = (width / (GlobalVariables.ScreenBounds.x * 2)) * 1920.0f; // fractional multi by ref scale

        float height = _enemyHandler.RandomSpawnUpperBounds.y - _enemyHandler.RandomSpawnLowerBounds.y;
        height = (height / (GlobalVariables.ScreenBounds.y * 2)) * 1080.0f;

        float posX = _enemyHandler.RandomSpawnUpperBounds.x - _enemyHandler.RandomSpawnLowerBounds.x; // difference
        posX = ((posX / 2) + _enemyHandler.RandomSpawnLowerBounds.x) / (GlobalVariables.ScreenBounds.x * 2); // fractional
        posX = posX * 1920.0f; // by reference scale

        float posY = _enemyHandler.RandomSpawnUpperBounds.y - _enemyHandler.RandomSpawnLowerBounds.y;
        posY = ((posY / 2) + _enemyHandler.RandomSpawnLowerBounds.y) / (GlobalVariables.ScreenBounds.y * 2);
        posY = posY * 1080.0f;

        RectTransform visualTransform = spawnConfigComponents.RandomSpawnVisual.GetComponent<RectTransform>();
        visualTransform.localPosition = new Vector3(posX, posY, visualTransform.localPosition.z);
        visualTransform.sizeDelta = new Vector2(width, height);
    }

    public void UpdateExactSpawnVisual() {
        float posX = GlobalVariables.ScreenBounds.x - _enemyHandler.ExactSpawnLocation.x;
        posX = (posX / (GlobalVariables.ScreenBounds.x * 2)) * 1920.0f;
        posX = -(posX - 960.0f); // offset and reverse
        
        float posY = GlobalVariables.ScreenBounds.y - _enemyHandler.ExactSpawnLocation.y;
        posY = (posY / (GlobalVariables.ScreenBounds.y * 2)) * 1080.0f;
        posY = -(posY - 540.0f);

        RectTransform visualTransform = spawnConfigComponents.ExactSpawnVisual.GetComponent<RectTransform>();
        visualTransform.localPosition = new Vector3(posX, posY, visualTransform.localPosition.z);
    }
    
    public void ShowEnemyVariables(int index) {
        enemyConfigComponents.EnemyVariableComponents[enemyVarsEnabled].SetActive(false);
        enemyConfigComponents.EnemyVariableComponents[index].SetActive(true);

        enemyVarsEnabled = index;
    }

    public void UpdateMovementOptions(int value) {
        if (value == 0) {
            enemyConfigComponents.DistanceFromPlayerField.SetActive(false);
            enemyConfigComponents.MoveDirectionField.SetActive(false);
        }
        else if (value == 1) {
            enemyConfigComponents.DistanceFromPlayerField.SetActive(true);
            enemyConfigComponents.MoveDirectionField.SetActive(false);
        }
        else if (value == 2) {
            enemyConfigComponents.DistanceFromPlayerField.SetActive(false);
            enemyConfigComponents.MoveDirectionField.SetActive(true);
        }
    }

    private void Update() {
        if (!isMoving) {
            if (Input.GetKeyDown(KeyCode.Comma)) Toggle(true);
            else if (Input.GetKeyDown(KeyCode.Period)) Toggle(false);
        }
    }
}
