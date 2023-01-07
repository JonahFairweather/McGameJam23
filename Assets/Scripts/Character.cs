using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    private float characterSpeed = 5.0f;
    private bool hasSnowball = true;
    private bool facingRight = true;
    [SerializeField] private Snowball snowball;
    private float snowballSpeed = 5.0f;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        Move();
        Fire();
    }

    private void Move() {
        // obtain the character velocity
        float xVel = Input.GetAxis("Horizontal") * characterSpeed * Time.deltaTime;
        float yVel = Input.GetAxis("Vertical") * characterSpeed * Time.deltaTime;

        // update the direction in which the character is facing
        this.facingRight = xVel != 0 ? xVel > 0 : this.facingRight;

        // move the character
        Vector3 move = new Vector3(xVel, yVel, 0);
        if (move != Vector3.zero) {
            transform.Translate(move);
        }
    }

    private void Fire() {
        bool isFire = Input.GetMouseButtonDown(0);
        if (isFire && this.hasSnowball) {
            // get the spawn offset
            float epsilon = 0.05f;
            float colliderRadius = this.gameObject.GetComponent<CircleCollider2D>().radius;
            float snowballRadius = this.snowball.gameObject.GetComponent<CircleCollider2D>().radius;
            Vector3 offset = new Vector3((colliderRadius + snowballRadius + epsilon), 0, 0);

            // get the snowball spawn position
            Vector3 snowballSpawnPosition;
            if (facingRight) {
                snowballSpawnPosition = this.transform.position + offset;
            } else {
                snowballSpawnPosition = this.transform.position - offset;
            }

            // get the mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // only allow shooting in the direction the character is facing
            if ((this.facingRight && mousePosition.x < snowballSpawnPosition.x) ||  // character facing right
                (!this.facingRight && mousePosition.x > snowballSpawnPosition.x))  // character facing left
                    return;

            // get the direction to shoot the snowball in
            Vector3 shootDirection = mousePosition - snowballSpawnPosition;
            shootDirection = Vector3.Normalize(shootDirection);

            // FIRE !!
            Snowball snowballInstance = GameObject.Instantiate(snowball, snowballSpawnPosition, Quaternion.identity);
            snowballInstance.GetComponent<Rigidbody2D>().velocity = shootDirection * this.snowballSpeed;

            // set hasSnowball to false to indicate that the character used the snowball it made.
            this.hasSnowball = false;
        }
    }
}
