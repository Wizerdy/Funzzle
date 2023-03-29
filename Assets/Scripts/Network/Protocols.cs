using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public static class Protocols {
    public const uint SIZE_8 = 1;
    public const uint SIZE_16 = 2;
    public const uint SIZE_32 = 4;
    public const uint SIZE_V3 = SIZE_32 * 3;

    public enum Opcode {
        C_JOIN_GAME,            // [NAME:string]
        C_INPUTS,               // [INPUTS:input]
        C_PUZZLE_IMAGE,         // [TEXTURE:texture]
        C_PUZZLE,               // [SIZE:v2][SCALE:v2]
        C_START_GAME,           // 

        S_ID,                   // [ID:u8]
        S_NEW_PLAYER,           // [ID:u8][NAME:string]
        S_PLAYER_LEFT,          // [ID:u8]
        S_PLAYERS_LIST,         // [COUNT:u8]{PLAYERS:[ID:u8][NAME:string]}
        S_PUZZLE_IMAGE,         // [TEXTURE:texture]
        S_PUZZLE,               // [SIZE:v2][SCALE:v2]
        S_WHOLE_PUZZLE,         // [COUNT:u16]{PIECES:[ID:u32][GATES:bool][POSITION:v2i]}
        S_PIECES,               // [COUNT:u16]{PIECES:[PHYSIC:PhysicState]}
        S_START_GAME            // 
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

    public static void Serialize_v2(this List<byte> packet, Vector2 value) {
        packet.Serialize_f32(value.x);
        packet.Serialize_f32(value.y);
    }

    public static void Serialize_v3(this List<byte> packet, Vector3 value) {
        packet.Serialize_f32(value.x);
        packet.Serialize_f32(value.y);
        packet.Serialize_f32(value.z);
    }

    public static void Serialize_v4(this List<byte> packet, Vector4 value) {
        packet.Serialize_f32(value.x);
        packet.Serialize_f32(value.y);
        packet.Serialize_f32(value.z);
        packet.Serialize_f32(value.w);
    }

    public static void Serialize_v2i(this List<byte> packet, Vector2Int value) {
        packet.Serialize_i32(value.x);
        packet.Serialize_i32(value.y);
    }

    public static void Serialize_v3i(this List<byte> packet, Vector3Int value) {
        packet.Serialize_i32(value.x);
        packet.Serialize_i32(value.y);
        packet.Serialize_i32(value.z);
    }

    public static void Serialize_texture(this List<byte> packet, Texture2D value) {
        byte[] bytes = value.GetRawTextureData();
        packet.Serialize_u8((uint)value.format);
        packet.Serialize_u16((uint)value.width);
        packet.Serialize_u16((uint)value.height);
        packet.Serialize_u32((uint)bytes.Length);
        packet.AddRange(bytes);
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

    public static Vector2 Unserialize_v2(this List<byte> packet, ref int offset) {
        Vector2 output;
        output.x = packet.Unserialize_f32(ref offset);
        output.y = packet.Unserialize_f32(ref offset);
        return output;
    }

    public static Vector3 Unserialize_v3(this List<byte> packet, ref int offset) {
        Vector3 output;
        output.x = packet.Unserialize_f32(ref offset);
        output.y = packet.Unserialize_f32(ref offset);
        output.z = packet.Unserialize_f32(ref offset);
        return output;
    }

    public static Vector4 Unserialize_v4(this List<byte> packet, ref int offset) {
        Vector4 output;
        output.x = packet.Unserialize_f32(ref offset);
        output.y = packet.Unserialize_f32(ref offset);
        output.z = packet.Unserialize_f32(ref offset);
        output.w = packet.Unserialize_f32(ref offset);
        return output;
    }

    public static Vector2Int Unserialize_v2i(this List<byte> packet, ref int offset) {
        Vector2Int output = Vector2Int.zero;
        output.x = packet.Unserialize_i32(ref offset);
        output.y = packet.Unserialize_i32(ref offset);
        return output;
    }

    public static Vector3Int Unserialize_v3i(this List<byte> packet, ref int offset) {
        Vector3Int output = Vector3Int.zero;
        output.x = packet.Unserialize_i32(ref offset);
        output.y = packet.Unserialize_i32(ref offset);
        output.z = packet.Unserialize_i32(ref offset);
        return output;
    }

    public static Texture2D Unserialize_texture(this List<byte> packet, ref int offset) {
        TextureFormat format = (TextureFormat)packet.Unserialize_u8(ref offset);
        int width = (int)packet.Unserialize_u16(ref offset);
        int height = (int)packet.Unserialize_u16(ref offset);
        int length = (int)packet.Unserialize_u32(ref offset);
        byte[] bytes = new byte[length];
        packet.CopyTo(offset, bytes, 0, length);
        offset += length;
        Texture2D texture = new Texture2D(width, height, format, false);
        texture.LoadRawTextureData(bytes);
        texture.Apply();

        return texture;
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

public struct PhysicState {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public void Serialize(List<byte> array) {
        array.Serialize_v3(position);
        array.Serialize_v4(new Vector4(rotation.x, rotation.y, rotation.z, rotation.w));
        array.Serialize_v3(velocity);
        array.Serialize_v3(angularVelocity);
    }

    public static PhysicState Unserialize(List<byte> array, ref int offset) {
        PhysicState physicState = new PhysicState();

        physicState.position = array.Unserialize_v3(ref offset);
        Vector4 rotation = array.Unserialize_v4(ref offset);
        physicState.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        physicState.velocity = array.Unserialize_v3(ref offset);
        physicState.angularVelocity = array.Unserialize_v3(ref offset);
        return physicState;
    }
};