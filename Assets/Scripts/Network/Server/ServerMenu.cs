using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerMenu : MonoBehaviour {
    [Header("Fields")]
    [SerializeField] ushort _port = 14769;
    [SerializeField] string _name = "Billy";
    [SerializeField] int _nextScene = 0;
    [Header("Objects")]
    [SerializeField] TMP_InputField portInputField;
    [SerializeField] TMP_InputField nameInputField;

    private void Start() {
        try {
            if (portInputField != null) {
                portInputField.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _port.ToString();
                portInputField.onValueChanged.AddListener(EnterPort);
            }
            if (nameInputField != null) {
                nameInputField.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _name;
                nameInputField.onValueChanged.AddListener(EnterName);
            }
        } catch (System.Exception e) {
            Debug.LogWarning(e);
        }
    }

    public void EnterPort(string text) {
        _port = ushort.Parse(text);
        if (_port == 0) {
            _port = 14769;
        }
    }

    public void EnterName(string text) {
        if (_name.Equals("")) {
            _name = "Billy";
        }
    }

    public void CreateServer() {
        PlayerInformation.Id = 0;
        PlayerInformation.Name = _name;

        ServerCore.CreateServer(_port);

        SceneLoader.LoadScene(_nextScene);
    }
}
