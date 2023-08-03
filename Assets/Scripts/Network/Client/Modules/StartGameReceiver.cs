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
            switch (opcode) {
                case Protocols.Opcode.S_IS_READY:
                    C_ReadyPacket packet = new C_ReadyPacket();

                    ClientCore.Send(packet);
                    break;
                case Protocols.Opcode.S_START_GAME:
                    SceneLoader.LoadScene(_nextScene);
                    break;
            }
        }
    }
}