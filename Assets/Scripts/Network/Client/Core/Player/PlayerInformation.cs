using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInformation {
    static uint _id;
    static string _name;

    public static uint Id { get => _id; set => _id = value; }
    public static string Name { get => _name; set => _name = value; }
}
