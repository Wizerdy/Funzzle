using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientModules {
    public class TextureSender : MonoBehaviour {
        [SerializeField] Texture2D _texture;

        public void SendTexture() {
            SendTexture(_texture);
        }

        public void SendTexture(Texture2D texture) {
            if (texture == null) return;

            C_PuzzleImagePacket packet = new C_PuzzleImagePacket();
            packet.texture = texture;
            ClientCore.Send(packet);
        }

        public void SendSize(int width, int height, float scaleX, float scaleY) {
            C_PuzzlePacket packet = new C_PuzzlePacket();

            packet.size = new Vector2Int(width, height);
            packet.scale = new Vector2(scaleX, scaleY);

            ClientCore.Send(packet);
        }
    }
}
