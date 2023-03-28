using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

namespace ClientModules {
    public class UITextureSender : MonoBehaviour {
        [SerializeField] TextureSender _textureSender;
        [SerializeField] Button _imageButton;
        [SerializeField] TMP_InputField _widthInput;
        [SerializeField] TMP_InputField _heightInput;

        [SerializeField] float _scaleX = 2f;
        [SerializeField] float _scaleY = 2f;

        int _width = 5;
        int _height = 5;

        Vector2 _referenceSize = Vector2.zero;

        private void Start() {
            _imageButton?.onClick.AddListener(_OnButtonClick);
            _widthInput?.onSubmit.AddListener(_OnWidthChange);
            _heightInput?.onSubmit.AddListener(_OnHeightChange);
            _referenceSize = _imageButton.image.rectTransform.sizeDelta;
        }

        private void _OnWidthChange(string input) {
            int width = 0;
            if (!Int32.TryParse(input, out width)) { return; }

            _width = width;
            SizeChanged();
        }

        private void _OnHeightChange(string input) {
            int height = 0;
            if (!Int32.TryParse(input, out height)) { return; }

            _height = height;
            SizeChanged();
        }

        private void SizeChanged() {
            _textureSender?.SendSize(_width, _height, _scaleX, _scaleY);
        }

        private void _OnButtonClick() {
            Texture2D texture = ImageExplorer();
            if (texture == null) { return; }
            //List<byte> bytes = new List<byte>();
            //int offset = 0;
            //Protocols.Serialize_texture(bytes, texture);
            //texture = bytes.Unserialize_texture(ref offset);
            //Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            //SetImage(sprite);

            _textureSender?.SendTexture(texture);
        }

        private void SetImage(Sprite sprite) {
            _imageButton.image.sprite = sprite;

            Vector2 imageSize = Vector2.zero;
            if (sprite.rect.width > sprite.rect.height) {
                imageSize.x = _referenceSize.x;
                imageSize.y = imageSize.x * sprite.textureRect.height / sprite.textureRect.width;
            } else {
                imageSize.y = _referenceSize.y;
                imageSize.x = imageSize.y * sprite.textureRect.width / sprite.textureRect.height;
            }

            _imageButton.image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageSize.x);
            _imageButton.image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageSize.y);
        }

        private Texture2D ImageExplorer() {
            string path = UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "png");

            Texture2D texture = new Texture2D(1, 1);
            if (path.Length != 0) {
                texture.LoadImage(File.ReadAllBytes(path));
            }
            return texture;
        }
    }
}
