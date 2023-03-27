using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
        C_JOIN_GAME,            // [NAME:string]
        C_INPUTS,               // [INPUTS:input]
        C_PUZZLE_IMAGE,         // [TEXTURE:texture]
        C_PUZZLE,               // [SIZE:v2][SCALE:v2]

        S_ID,                   // [ID:u8]
        S_NEW_PLAYER,           // [ID:u8][NAME:string]
        S_PLAYER_LEFT,          // [ID:u8]
        S_PLAYERS_LIST,         // [COUNT:u8]{PLAYERS:[ID:u8][NAME:string]}
        S_PUZZLE_IMAGE,         // [TEXTURE:texture]
        S_PUZZLE,               // [SIZE:v2][SCALE:v2]
        S_PIECES                // [COUNT:u16]{PIECES:[PHYSIC:PhysicState]}
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

public struct C_PuzzleImagePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.C_PUZZLE_IMAGE;
    public Protocols.Opcode Opcode => _opcode;

    public Texture2D texture;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_texture(texture);

        return packet;
    }

    public static C_PuzzleImagePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        C_PuzzleImagePacket packet = new C_PuzzleImagePacket();
        packet.texture = bytes.Unserialize_texture(ref offset);

        return packet;
    }
}

public struct C_PuzzlePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.C_PUZZLE;
    public Protocols.Opcode Opcode => _opcode;

    public Vector2Int size;
    public Vector2 scale;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_v2i(size);
        packet.Serialize_v2(scale);

        return packet;
    }

    public static C_PuzzlePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        C_PuzzlePacket packet = new C_PuzzlePacket();
        packet.size = bytes.Unserialize_v2i(ref offset);
        packet.scale = bytes.Unserialize_v2(ref offset);

        return packet;
    }
}

// Server

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

public struct S_NewPlayerPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_NEW_PLAYER;
    public Protocols.Opcode Opcode => _opcode;

    public uint id;
    public string name;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();
        packet.Serialize_u8(id);
        packet.Serialize_str(name);
        return packet;
    }

    public static S_NewPlayerPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_NewPlayerPacket packet = new S_NewPlayerPacket();
        packet.id = bytes.Unserialize_u8(ref offset);
        packet.name = bytes.Unserialize_str(ref offset);
        return packet;
    }
}

public struct S_PlayerLeftPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PLAYER_LEFT;
    public Protocols.Opcode Opcode => _opcode;

    public uint id;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();
        packet.Serialize_u8(id);
        return packet;
    }

    public static S_PlayerLeftPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PlayerLeftPacket packet = new S_PlayerLeftPacket();
        packet.id = bytes.Unserialize_u8(ref offset);
        return packet;
    }
}

public struct S_PlayersListPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PLAYERS_LIST;
    public Protocols.Opcode Opcode => _opcode;

    public struct Player {
        public uint id;
        public string name;
    };

    public List<Player> players;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_u8((uint)players.Count);
        for (int i = 0; i < players.Count; i++) {
            packet.Serialize_u8(players[i].id);
            packet.Serialize_str(players[i].name);
        }

        return packet;
    }

    public static S_PlayersListPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PlayersListPacket packet = new S_PlayersListPacket();
        packet.players = new List<Player>();
        int count = (int)bytes.Unserialize_u8(ref offset);

        for (int i = 0; i < count; i++) {
            Player player = new Player();
            player.id = bytes.Unserialize_u8(ref offset);
            player.name = bytes.Unserialize_str(ref offset);
            packet.players.Add(player);
        }

        return packet;
    }
}

public struct S_PuzzleImagePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PUZZLE_IMAGE;
    public Protocols.Opcode Opcode => _opcode;

    public Texture2D texture;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_texture(texture);

        return packet;
    }

    public static S_PuzzleImagePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PuzzleImagePacket packet = new S_PuzzleImagePacket();
        packet.texture = bytes.Unserialize_texture(ref offset);

        return packet;
    }
}

public struct S_PuzzlePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PUZZLE;
    public Protocols.Opcode Opcode => _opcode;

    public Vector2 size;
    public Vector2 piecesScale;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_v2(size);
        packet.Serialize_v2(piecesScale);

        return packet;
    }

    public static S_PuzzlePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PuzzlePacket packet = new S_PuzzlePacket();
        packet.size = bytes.Unserialize_v2(ref offset);
        packet.piecesScale = bytes.Unserialize_v2(ref offset);

        return packet;
    }
}

public struct S_PiecesPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PIECES;
    public Protocols.Opcode Opcode => _opcode;

    public List<PhysicState> pieces;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_u16((uint)pieces.Count);
        for (int i = 0; i < pieces.Count; i++) {
            pieces[i].Serialize(packet);
        }

        return packet;
    }

    public static S_PiecesPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PiecesPacket packet = new S_PiecesPacket();
        packet.pieces = new List<PhysicState>();
        int count = (int)bytes.Unserialize_u16(ref offset);

        for (int i = 0; i < count; i++) {
            PhysicState physic = PhysicState.Unserialize(bytes, ref offset);
            packet.pieces.Add(physic);
        }

        return packet;
    }
}