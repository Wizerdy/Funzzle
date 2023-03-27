using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ClientModules {
    public class DisplayPlayerName : MonoBehaviour {
        [SerializeField] TextMeshProUGUI _text;

        private void Reset() {
            _text = GetComponent<TextMeshProUGUI>();
        }

        void Start() {
            _text.text = PlayerInformation.Name;
        }
    }
}
