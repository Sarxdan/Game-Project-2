using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        print("test");
    }

    private void OnCollisionEnter(Collision other) {
        print("test");
    }
}
