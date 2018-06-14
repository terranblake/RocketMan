﻿using UnityEngine;

public class Sticky : MonoBehaviour {
    void OnCollisionEnter(Collision c) {
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = c.rigidbody;
    }
}
