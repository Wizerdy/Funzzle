using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/*
        C_JOIN_GAME,            // [NAME:string]
        C_INPUTS,               // [INPUTS:input]
        C_PUZZLE_IMAGE,         // [TEXTURE:texture]
        C_PUZZLE,               // [SIZE:v2][SCALE:v2]
        C_START_GAME            // 

        S_ID,                   // [ID:u8]
        S_NEW_PLAYER,           // [ID:u8][NAME:string]
        S_PLAYER_LEFT,          // [ID:u8]
        S_PLAYERS_LIST,         // [COUNT:u8]{PLAYERS:[ID:u8][NAME:string]}
        S_PUZZLE_IMAGE,         // [TEXTURE:texture]
        S_PUZZLE,               // [SIZE:v2][SCALE:v2]
        S_WHOLE_PUZZLE,         // [COUNT:u16]{PIECES:[ID:u32][GATES:bool][POSITION:v2i]}
        S_PIECES,               // [COUNT:u16]{PIECES:[PHYSIC:PhysicState]}
        S_START_GAME            // 
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

public struct C_StartGamePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.C_START_GAME;
    public Protocols.Opcode Opcode => _opcode;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        return packet;
    }

    public static C_StartGamePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        C_StartGamePacket packet = new C_StartGamePacket();

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

    public Vector2Int size;
    public Vector2 piecesScale;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_v2i(size);
        packet.Serialize_v2(piecesScale);

        return packet;
    }

    public static S_PuzzlePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PuzzlePacket packet = new S_PuzzlePacket();
        packet.size = bytes.Unserialize_v2i(ref offset);
        packet.piecesScale = bytes.Unserialize_v2(ref offset);

        return packet;
    }
}

public struct S_WholePuzzlePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_WHOLE_PUZZLE;
    public Protocols.Opcode Opcode => _opcode;

    public struct Piece {
        public int[] gates;
        public Vector2Int position;
    }

    public List<Piece> pieces;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_u16((uint)pieces.Count);
        for (int i = 0; i < pieces.Count; i++) {
            packet.Serialize_i8(pieces[i].gates[0]);
            packet.Serialize_i8(pieces[i].gates[1]);
            packet.Serialize_i8(pieces[i].gates[2]);
            packet.Serialize_i8(pieces[i].gates[3]);
            packet.Serialize_v2i(pieces[i].position);
        }

        return packet;
    }

    public static S_WholePuzzlePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_WholePuzzlePacket packet = new S_WholePuzzlePacket();
        int count = (int)bytes.Unserialize_u16(ref offset);

        packet.pieces = new List<Piece>();
        for (int i = 0; i < count; i++) {
            Piece piece = new Piece();
            piece.gates = new int[4];
            piece.gates[0] = bytes.Unserialize_i8(ref offset);
            piece.gates[1] = bytes.Unserialize_i8(ref offset);
            piece.gates[2] = bytes.Unserialize_i8(ref offset);
            piece.gates[3] = bytes.Unserialize_i8(ref offset);
            piece.position = bytes.Unserialize_v2i(ref offset);

            packet.pieces.Add(piece);
        }

        return packet;
    }
}

public struct S_PiecesPacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_PIECES;
    public Protocols.Opcode Opcode => _opcode;

    public List<(Vector2Int, PhysicState)> pieces;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        packet.Serialize_u16((uint)pieces.Count);
        for (int i = 0; i < pieces.Count; i++) {
            packet.Serialize_v2i(pieces[i].Item1);
            pieces[i].Item2.Serialize(packet);
        }

        return packet;
    }

    public static S_PiecesPacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_PiecesPacket packet = new S_PiecesPacket();
        packet.pieces = new List<(Vector2Int, PhysicState)>();
        int count = (int)bytes.Unserialize_u16(ref offset);

        for (int i = 0; i < count; i++) {
            Vector2Int ppos = bytes.Unserialize_v2i(ref offset);
            PhysicState physic = PhysicState.Unserialize(bytes, ref offset);
            packet.pieces.Add(new (ppos, physic));
        }

        return packet;
    }
}

public struct S_StartGamePacket : Protocols.IPacket {
    static Protocols.Opcode _opcode => Protocols.Opcode.S_START_GAME;
    public Protocols.Opcode Opcode => _opcode;

    public List<byte> Serialize() {
        List<byte> packet = new List<byte>();

        return packet;
    }

    public static S_StartGamePacket Unserialize(List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, _opcode, ref offset);

        S_StartGamePacket packet = new S_StartGamePacket();

        return packet;
    }
}