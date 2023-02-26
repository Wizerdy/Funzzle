using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Protocols {
    public const uint SIZE_8 = 1;
    public const uint SIZE_16 = 2;
    public const uint SIZE_32 = 4;
    public const uint SIZE_V3 = SIZE_32 * 3;

    public enum Opcode {
        C_JOIN_GAME,            // [NAME:string]
        C_INPUTS,               // [INPUTS:input]

        S_ID,                   // [ID:u8]
        S_NEW_PLAYER,           // [ID:u8][NAME:string]
        S_PLAYER_LEFT,          // [ID:u8]
        S_PLAYERS_LIST,         // {PLAYERS:[ID:u8]}
        S_PUZZLE,               // [TEXTURE:texture][SIZE:v2][SCALE:v2]
        S_PIECES                // {PIECES:[PIECE:pieceState]}
    }

    public interface IPacket {
        public Opcode Opcode { get; }
        public List<byte> Serialize();
    }

    #region Utilities

    public static byte[] hton(this byte[] bytes) {
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    public static byte[] ntoh(this byte[] bytes) {
        return bytes.hton();
    }

    public static string Print(this byte[] bytes) {
        return BitConverter.ToString(bytes);
    }

    public static string Print(this List<byte> bytes) {
        return BitConverter.ToString(bytes.ToArray());
    }

    #endregion

    #region Serialization

    private static void Serialize_u(List<byte> packet, uint uinteger, int size) {
        byte[] bytes = BitConverter.GetBytes(uinteger);
        if (size > 1) {
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
        }
        Array.Resize(ref bytes, size);
        if (size > 1) {
            Array.Reverse(bytes);
        }
        packet.AddRange(bytes);
    }

    private static void Serialize_f(List<byte> packet, float floater, int size) {
        byte[] bytes = BitConverter.GetBytes(floater);
        if (size > 1) {
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
        }
        Array.Resize(ref bytes, size);
        if (size > 1) {
            Array.Reverse(bytes);
        }
        packet.AddRange(bytes);
    }

    /// <summary>
    /// Concatenate a maximum of 8 bool in a uint8
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="booleans"></param>
    public static void Serialize_b(this List<byte> packet, params bool[] booleans) {
        if (booleans.Length > 8) { throw new ArgumentOutOfRangeException(); }
        uint container = 0;
        for (int i = 0; i < booleans.Length; i++) {
            container |= booleans[i] ? (uint)1 << i : 0;
        }
        packet.Serialize_u8(container);
    }

    public static void Serialize_u8(this List<byte> packet, uint uinteger) {
        Serialize_u(packet, uinteger, (int)SIZE_8);
    }

    public static void Serialize_u16(this List<byte> packet, uint uinteger) {
        Serialize_u(packet, uinteger, (int)SIZE_16);
    }

    public static void Serialize_u32(this List<byte> packet, uint uinteger) {
        Serialize_u(packet, uinteger, (int)SIZE_32);
    }

    public static void Serialize_i8(this List<byte> packet, int integer) {
        Serialize_u(packet, (uint)integer, (int)SIZE_8);
    }

    public static void Serialize_i16(this List<byte> packet, int integer) {
        Serialize_u(packet, (uint)integer, (int)SIZE_16);
    }

    public static void Serialize_i32(this List<byte> packet, int integer) {
        Serialize_u(packet, (uint)integer, (int)SIZE_32);
    }

    public static void Serialize_f32(this List<byte> packet, float floater) {
        Serialize_f(packet, floater, (int)SIZE_32);
    }

    public static void Serialize_str(this List<byte> packet, string value) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        if (bytes.Length > 255) {
            Array.Resize(ref bytes, 255);
        }
        packet.Serialize_u8((uint)bytes.Length);
        packet.AddRange(bytes);
    }

    public static void Serialize_v3(this List<byte> packet, Vector3 value) {
        packet.Serialize_f32(value.x);
        packet.Serialize_f32(value.y);
        packet.Serialize_f32(value.z);
    }

    #endregion

    #region Unserialization

    public static bool[] Unserialize_b(this List<byte> packet, ref int offset) {
        bool[] values = new bool[8];
        uint value = packet.Unserialize_u8(ref offset);
        for (int i = 0; i < 8; i++) {
            values[i] = (value & (1 << i)) != 0;
        }
        return values;
    }

    public static uint Unserialize_u8(this List<byte> packet, ref int offset) {
        uint value = packet[offset];
        offset += (int)SIZE_8;
        return value;
    }

    public static uint Unserialize_u16(this List<byte> packet, ref int offset) {
        byte[] bytes = new byte[(int)SIZE_16];
        packet.CopyTo(offset, bytes, 0, (int)SIZE_16);
        bytes.ntoh();
        uint value = BitConverter.ToUInt16(bytes);
        offset += (int)SIZE_16;
        return value;
    }

    public static uint Unserialize_u32(this List<byte> packet, ref int offset) {
        byte[] bytes = new byte[(int)SIZE_32];
        packet.CopyTo(offset, bytes, 0, (int)SIZE_32);
        bytes.ntoh();
        uint value = BitConverter.ToUInt32(bytes);
        offset += (int)SIZE_32;
        return value;
    }

    public static int Unserialize_i8(this List<byte> packet, ref int offset) {
        return (int)Unserialize_u8(packet, ref offset);
    }

    public static int Unserialize_i16(this List<byte> packet, ref int offset) {
        return (int)Unserialize_u16(packet, ref offset);
    }

    public static int Unserialize_i32(this List<byte> packet, ref int offset) {
        return (int)Unserialize_u32(packet, ref offset);
    }

    public static float Unserialize_f32(this List<byte> packet, ref int offset) {
        byte[] bytes = new byte[(int)SIZE_32];
        packet.CopyTo(offset, bytes, 0, (int)SIZE_32);
        bytes.ntoh();
        float value = BitConverter.ToSingle(bytes);
        offset += (int)SIZE_32;
        return value;
    }

    public static string Unserialize_str(this List<byte> packet, ref int offset) {
        int length = (int)packet.Unserialize_u8(ref offset);
        if (length <= 0)
            return "";
        byte[] bytes = new byte[length];
        packet.CopyTo(offset, bytes, 0, length);
        string value = System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        offset += length;
        return value;
    }

    public static Vector3 Unserialize_v3(this List<byte> packet, ref int offset) {
        Vector3 output;
        output.x = packet.Unserialize_f32(ref offset);
        output.y = packet.Unserialize_f32(ref offset);
        output.z = packet.Unserialize_f32(ref offset);
        return output;
    }

    #endregion

    public static Packet BuildPacket(IPacket packet) {
        List<byte> output = new List<byte>();
        output.Serialize_u8((uint)packet.Opcode);
        output.AddRange(packet.Serialize());

        Packet enetpacket = default(Packet);
        enetpacket.Create(output.ToArray());
        return enetpacket;
    }

    public static bool TestOpcode(List<byte> bytes, Opcode opcode, ref int offset) {
        Opcode target = (Opcode)bytes.Unserialize_u8(ref offset);
        if (opcode != target) { throw new System.ArrayTypeMismatchException(); }
        return opcode != target;
    }
}
