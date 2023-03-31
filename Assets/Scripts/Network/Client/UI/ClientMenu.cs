using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientMenu : MonoBehaviour {
    [Header("Fields")]
    [SerializeField] PlayerInformation _playerInformation;
    [SerializeField] ushort _port = 14769;
    [SerializeField] string _ip = "localhost";
    [SerializeField] string _name = "Billy";
    [SerializeField] int _nextScene = 0;
    [Header("Objects")]
    [SerializeField] TMP_InputField ipInputField;
    [SerializeField] TMP_InputField portInputField;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TextMeshProUGUI outputField;

    private void Start() {
        ClientCore.OnConnect += _OnConnect;
        ClientCore.OnReceive += _OnReceive;
        ClientCore.OnTimeout += _OnTimeout;

        try {
            if (ipInputField != null) {
                ipInputField.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _ip;
                ipInputField.onValueChanged.AddListener(EnterIp);
            }
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

    private void OnDestroy() {
        ClientCore.OnConnect -= _OnConnect;
        ClientCore.OnReceive -= _OnReceive;
        ClientCore.OnTimeout -= _OnTimeout;
    }

    public void EnterIp(string text) {
        _ip = text;
        if (_ip.Equals("")) {
            _ip = "localhost";
        }
    }

    public void EnterPort(string text) {
        _port = ushort.Parse(text);
        if (_port == 0) {
            _port = 14769;
        }
    }

    public void EnterName(string text) {
        _name = text;
        if (_name.Equals("")) {
            _name = "Billy";
        }
    }

    public void Connect() {
        if (ClientCore.Connect(_ip, _port)) {
            outputField.text = "Attempting to connect to " + _ip + ":" + _port;
        }
    }

    private void _OnTimeout(ENet.Peer peer) {
        outputField.text = "Failed to connect";
    }

    private void _OnConnect(ENet.Peer peer) {
        C_JoinGamePacket packet;
        packet.name = _name;

        Debug.Log("Connected ! ");

        ClientCore.Send(packet);

        outputField.text = "Connected !";
    }

    private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
        if (opcode != Protocols.Opcode.S_ID) { return; }

        S_IdPacket packet = S_IdPacket.Unserialize(bytes);
        _playerInformation.Id = packet.id;
        _playerInformation.Name = _name;

        Debug.Log("#" + _playerInformation.Id + ":" + _playerInformation.Name);

        SceneLoader.LoadScene(_nextScene);
    }
}
