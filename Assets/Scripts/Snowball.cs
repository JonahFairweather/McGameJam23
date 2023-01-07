using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour {
    private Vector3 initialPosition;
    private float maxThrowDistance = 5.0f;

    // Start is called before the first frame update
    void Start() {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        float dist = Vector3.Distance(initialPosition, transform.position);
        if (dist >= maxThrowDistance) {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject.Destroy(this.gameObject);
    }
}
