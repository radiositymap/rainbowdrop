using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour
{
    GameManager gameManager;
    Collider2D[] colliders;
    Rigidbody2D rbd;
    bool hasLanded = false;
    bool hasStarted = false;
    bool isInSky = true;
    bool isRotating = false;

    // Start is called before the first frame update
    void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>();
        rbd = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        rbd.freezeRotation = true;
        hasStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted) {
            return;
        }

        if (hasLanded)
            return;

        if (gameManager.pauseControls)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (gameManager.currLevel >= 3) {
                isRotating = true;
            }
            else {
                transform.Rotate(new Vector3(0, 0, 90), Space.World);
                
            }
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            if (gameManager.currLevel >= 3) {
                isRotating = false;
                transform.eulerAngles = new Vector3(0, 0,
                    Mathf.RoundToInt(transform.eulerAngles.z/90f) * 90f);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position += new Vector3(0, -15 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.position += new Vector3(-15 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.position += new Vector3(15 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) ||
                Input.GetKeyUp(KeyCode.LeftArrow) ||
                Input.GetKeyUp(KeyCode.RightArrow)) {
            transform.position = new Vector3(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z)
            );
        }

        if (gameManager.currLevel >= 3 && isRotating) {
            transform.Rotate(
                new Vector3(0, 0, 1f),
                600f * Time.deltaTime,
                Space.Self);
        }

        if (gameManager.currLevel == 0 && transform.position.x < -8) {
            transform.position = new Vector3(
                -8,
                transform.position.y,
                transform.position.z
            );
        }
        else if (gameManager.currLevel == 0 && transform.position.x > 8) {
            transform.position = new Vector3(
                8,
                transform.position.y,
                transform.position.z
            );

        }
    }

    void OnTriggerExit2D(Collider2D exitCollider) {
        if (exitCollider.CompareTag("Sky")) {
            isInSky = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Block") ||
            collision.collider.CompareTag("Floor")) {
            if (!hasLanded && gameManager.onBlockLanded != null) {
                hasLanded = true;
                if (isInSky) {
                    Debug.Log("game over");
                    hasStarted = false;
                    if (gameManager.onTouchSky != null)
                        gameManager.onTouchSky();
                    return;
                }

                transform.eulerAngles = new Vector3(0, 0,
                    Mathf.RoundToInt(transform.eulerAngles.z/90f) * 90f);

                transform.position = new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y),
                    Mathf.RoundToInt(transform.position.z)
                );
                gameManager.onBlockLanded();
            }
        }
    }
}
