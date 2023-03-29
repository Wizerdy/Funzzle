using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Informations", menuName = "Scriptable Objects/Informations/Player Informations")]
public class PlayerInformation : ScriptableObject {
    uint _id = 4444;
    string _name = "Not Set";
    bool _isServer = false;

    public uint Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public bool IsServer { get => _isServer; set => _isServer = value; }
}
