using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientMenu : MonoBehaviour {
    [Header("Fields")]
    [SerializeField] ushort _port = 14769;
    [SerializeField] string _ip = "localhost";
    [SerializeField] string _name = "Billy";
    [SerializeField] int _nextScene = 0;
    [Header("Objects")]
    [SerializeField] TMP_InputField ipInputField;
    [SerializeField] TMP_InputField portInputField;
    [SerializeField] TMP_InputField nameInputField;

    private void Start() {
        ClientCore.OnConnect += _OnConnect;
        ClientCore.OnReceive += _OnReceive;

        try {
            if (ipInputField != null) {
                ipInputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = _ip;
                ipInputField.onValueChanged.AddListener(EnterIp);
            }
            if (portInputField != null) {
                portInputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = _port.ToString();
                portInputField.onValueChanged.AddListener(EnterPort);
            }
            if (nameInputField != null) {
                nameInputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = _name;
                nameInputField.onValueChanged.AddListener(EnterName);
            }
        } catch (System.Exception e) {
            Debug.LogWarning(e);
        }
    }

    private void OnDestroy() {
        ClientCore.OnConnect -= _OnConnect;
        ClientCore.OnReceive -= _OnReceive;
    }

    public void EnterIp(string text) {
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
        if (_name.Equals("")) {
            _name = "Billy";
        }
    }

    public void Connect() {
        ClientCore.Connect(_ip, _port);
    }

    private void _OnConnect(ENet.Peer peer) {
        C_JoinGamePacket packet;
        packet.name = _name;

        ClientCore.Send(packet);

        Debug.Log("Connected ! ");
    }

    private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
        if (opcode != Protocols.Opcode.S_ID) { return; }

        S_IdPacket packet = S_IdPacket.Unserialize(bytes);
        PlayerInformation.Id = packet.id;
        PlayerInformation.Name = _name;

        Debug.Log("#" + PlayerInformation.Id + ":" + PlayerInformation.Name);

        SceneLoader.LoadScene(_nextScene);
    }
}
