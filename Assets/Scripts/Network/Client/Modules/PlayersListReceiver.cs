using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ClientModules {
    public class PlayersListReceiver : MonoBehaviour {
        [SerializeField] TextMeshProUGUI _textPrefab = null;
        [SerializeField] Transform _parent = null;

        Dictionary<uint, TextMeshProUGUI> _texts = new Dictionary<uint, TextMeshProUGUI>();

        private void Reset() {
            _parent = transform;
        }

        private void Start() {
            ClientPlayersList.OnNewPlayer += _OnNewPlayer;
            ClientPlayersList.OnPlayerLeft += _OnPlayerLeft;

            Dictionary<uint, Player> players = ClientPlayersList.Instance.Players;
            foreach (var player in players) {
                _OnNewPlayer(player.Value);
            }
        }

        private void OnDestroy() {
            ClientPlayersList.OnNewPlayer -= _OnNewPlayer;
            ClientPlayersList.OnPlayerLeft -= _OnPlayerLeft;
        }

        private void _OnNewPlayer(Player player) {
            TextMeshProUGUI obj = Instantiate(_textPrefab.gameObject, _parent).GetComponent<TextMeshProUGUI>();
            obj.text = player.name;
            _texts.Add(player.id, obj);
        }

        private void _OnPlayerLeft(Player player) {
            if (!_texts.ContainsKey(player.id)) { return; }

            TextMeshProUGUI tmp = _texts[player.id];
            _texts.Remove(player.id);

            Destroy(tmp.gameObject);
            tmp.gameObject.SetActive(false);
        }
    }
}
