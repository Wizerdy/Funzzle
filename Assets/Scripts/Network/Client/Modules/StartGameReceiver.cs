using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientModules {
    public class StartGameReceiver : MonoBehaviour {
        [SerializeField] int _nextScene = 5;

        void Start() {
            ClientCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ClientCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
            if (opcode != Protocols.Opcode.S_START_GAME) { return; }

            SceneLoader.LoadScene(_nextScene);
        }
    }
}