using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsServer : MonoBehaviour {
    [SerializeField] PlayerInformation _playerInformation;
    [SerializeField] bool _beServer = true;

    private void Awake() {
        if (!_beServer && _playerInformation.IsServer) {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}
