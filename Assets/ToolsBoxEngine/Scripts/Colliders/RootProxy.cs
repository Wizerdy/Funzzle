using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsBoxEngine {
    public class RootProxy : MonoBehaviour {
        [SerializeField] GameObject _root;

        public GameObject Root => _root;
    }
}