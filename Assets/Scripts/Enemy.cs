using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private bool isFollowing;
    private Vector3 characterPosition;
    private float enemySpeed = 3.0f;
    private float thresholdDistance;

    // Start is called before the first frame update
    void Start() {
        thresholdDistance = gameObject.GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update() {
        if (isFollowing) {
            Debug.Log("Following character...");
            // check the distance and if it's greater than a threshold, turn back isFollowing to False
            float dist = Vector3.Distance(characterPosition, transform.position);
            Debug.Log("Distance between character and enemy is: " + dist);
            if (dist > thresholdDistance) {
                isFollowing = false;
                return;
            }
            
            // obtain the direction of character w.r.t. the enemy
            Vector3 normalizedDir = Vector3.Normalize(new Vector3((characterPosition.x - transform.position.x), (characterPosition.y - transform.position.y), 0));
            
            // check for line-of-sight obstacles
            this.SetAllCollidersStatus(false);
            Collider2D potentialObstacle = Physics2D.Raycast(transform.position, normalizedDir, thresholdDistance).collider;
            this.SetAllCollidersStatus(true);
            if (potentialObstacle != null && potentialObstacle.gameObject.tag != "Player") {
                Debug.Log("Encountered an obstacle... " + potentialObstacle.gameObject.tag);
                isFollowing = false;
                return;
            }

            // make the enemy move towards the character
            float xVel = normalizedDir.x * enemySpeed * Time.deltaTime;
            float yVel = normalizedDir.y * enemySpeed * Time.deltaTime;

            Vector3 move = new Vector3(xVel, yVel, 0);

            if (move != Vector3.zero) {
                transform.Translate(move);
            }
        }
    }

    // called when the other collider is within the trigger distance
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Character detected within trigger radius!!");
            isFollowing = true;
            characterPosition = other.transform.position;
            thresholdDistance += 2 * other.gameObject.GetComponent<CircleCollider2D>().radius;
            Debug.Log("Threshold distance is: " + thresholdDistance.ToString());
        }
    }

    // called on each frame when the other collider is within the trigger distance
    private void OnTriggerStay2D(Collider2D other) {
        // update the character position
        // characterPosition = other.transform.position;
        if (other.gameObject.tag == "Player") {
            characterPosition = other.transform.position;
        }
    }

    private void SetAllCollidersStatus (bool active) {
        foreach(CircleCollider2D c in this.gameObject.GetComponents<CircleCollider2D>()) {
            c.enabled = active;
        }
    }
}