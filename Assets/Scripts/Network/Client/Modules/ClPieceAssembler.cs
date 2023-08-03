using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientModules {
    public class ClPieceAssembler : MonoBehaviour {


        void Start() {
            ClientCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ClientCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {

        }
    }
}