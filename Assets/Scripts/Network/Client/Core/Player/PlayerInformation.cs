using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInformation {
    static uint _id = 4444;
    static string _name = "Not Set";
    static bool _isServer = false;

    public static uint Id { get => _id; set => _id = value; }
    public static string Name { get => _name; set => _name = value; }
    public static bool IsServer { get => _isServer; set => _isServer = value; }
}
