using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldValidator : MonoBehaviour {
    private TMP_InputField _inputField;
    private string defaultValue;
    
    private void Start() {
        _inputField = this.gameObject.GetComponent<TMP_InputField>();
        defaultValue = _inputField.text;
    }

    public void FieldEmptyCheck(string value) {
        if (value == "") _inputField.text = defaultValue;
    }
}
