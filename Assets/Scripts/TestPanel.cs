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
}

public class TestPanel : MonoBehaviour {
    [SerializeField] private EnemyHandler _enemyHandler;
    [Header("Components")]
    [SerializeField] private Image colourHandle;
    [SerializeField] private SpawnConfigComponents spawnConfigComponents;
    [SerializeField] private GameObject[] enemyVariablesComponents;
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

        foreach (GameObject vars in enemyVariablesComponents) vars.SetActive(false);
        enemyVariablesComponents[enemyVarsEnabled].SetActive(true);
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

    public void ShowEnemyVariables(int index) {
        enemyVariablesComponents[enemyVarsEnabled].SetActive(false);
        enemyVariablesComponents[index].SetActive(true);

        enemyVarsEnabled = index;
    }
    
    private void Update() {
        if (!isMoving) {
            if (Input.GetKeyDown(KeyCode.Comma)) Toggle(true);
            else if (Input.GetKeyDown(KeyCode.Period)) Toggle(false);
        }
    }
}
