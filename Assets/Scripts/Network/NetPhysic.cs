using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPhysic : MonoBehaviour {
    [SerializeField] Rigidbody _rb;

    public Vector3 Position { get => _rb.transform.position; set => _rb.transform.position = value; }
    public Quaternion Rotation { get => _rb.transform.rotation; set => _rb.transform.rotation = value; }
    public Vector3 Velocity { get => _rb.velocity; set => _rb.velocity = value; }
    public Vector3 AngularVelocity { get => _rb.angularVelocity; set => _rb.angularVelocity = value; }

    public void SetPhysic(Vector3 position, Vector4 rotation, Vector3 velocity, Vector3 angularVelocity) {
        Position = position;
        Rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        Velocity = velocity;
        AngularVelocity = angularVelocity;
    }
}
