using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientModules {
    public class ClStartGame : MonoBehaviour {
        [SerializeField] Button _startButton;

        void Start() {
            _startButton?.onClick.AddListener(_OnClick);
        }

        private void OnDestroy() {
            _startButton?.onClick.RemoveListener(_OnClick);
        }

        private void _OnClick() {
            C_StartGamePacket packet = new C_StartGamePacket();

            ClientCore.Send(packet);
        }
    }
}
