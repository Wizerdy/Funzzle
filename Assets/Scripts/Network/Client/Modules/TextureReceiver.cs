using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClientModules {
    public class TextureReceiver : MonoBehaviour {
        [SerializeField] PuzzleInformation _clientPuzzle;
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _puzzleSize;

        Vector2 _referenceSize = Vector2.zero;

        void Start() {
            ClientCore.OnReceive += _OnReceive;

            _referenceSize = _image.rectTransform.sizeDelta;
        }

        private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
            switch (opcode) {
                case Protocols.Opcode.S_PUZZLE_IMAGE: {
                        S_PuzzleImagePacket packet = S_PuzzleImagePacket.Unserialize(bytes);

                        if (_image != null) {
                            SetImage(Sprite.Create(packet.texture, 
                                new Rect(0f, 0f, packet.texture.width, packet.texture.height), 
                                Vector2.one * 0.5f));

                            _clientPuzzle.Texture = packet.texture;
                        }
                    }
                    break;
                case Protocols.Opcode.S_PUZZLE: {
                        S_PuzzlePacket packet = S_PuzzlePacket.Unserialize(bytes);

                        if (_puzzleSize != null) {
                            _puzzleSize.text = "(" + packet.size.x + "x" + packet.size.y + ")";
                        }

                        _clientPuzzle.Size = packet.size;
                        _clientPuzzle.PieceScale = packet.piecesScale;
                    }
                    break;
            }
        }

        private void SetImage(Sprite sprite) {
            _image.sprite = sprite;

            Vector2 imageSize = Vector2.zero;
            if (sprite.rect.width > sprite.rect.height) {
                imageSize.x = _referenceSize.x;
                imageSize.y = imageSize.x * sprite.textureRect.height / sprite.textureRect.width;
            } else {
                imageSize.y = _referenceSize.y;
                imageSize.x = imageSize.y * sprite.textureRect.width / sprite.textureRect.height;
            }

            _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageSize.x);
            _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageSize.y);
        }
    }
}