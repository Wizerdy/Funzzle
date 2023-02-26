using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
        C_JOIN_GAME,            // [NAME:string]
        C_INPUTS,               // [INPUTS:input]

        S_ID,                   // [ID:u8]
        S_NEW_PLAYER,           // [ID:u8][NAME:string]
        S_PLAYER_LEFT,          // [ID:u8]
        S_PLAYERS_LIST,         // {PLAYERS:[ID:u8]}
        S_PUZZLE,               // [TEXTURE:texture][SIZE:v2][SCALE:v2]
        S_PIECES                // {PIECES:[PIECE:pieceState]}
*/

public struct C_JoinGamePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.C_JOIN_GAME;
    public Protocols.Opcode Opcode => _opcode;

    public string name;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();
        packet.Serialize_str(name);
        return packet;
    }

    public static C_JoinGamePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        C_JoinGamePacket packet = new C_JoinGamePacket();
        packet.name = bytes.Unserialize_str(ref offset);
        return packet;
    }
}

public struct S_IdPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_ID;
    public Protocols.Opcode Opcode => _opcode;

    public uint id;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();
        packet.Serialize_u8(id);
        return packet;
    }

    public static S_IdPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_IdPacket packet = new S_IdPacket();
        packet.id = bytes.Unserialize_u8(ref offset);
        return packet;
    }
}
