using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable : IGameObject {
    public void OnPickup();
    public void OnDrop();
}
