using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int x;
    public int y;
    public Color colour;

    Rigidbody2D rbd;

    void Start()
    {
        rbd = GetComponentInParent<Rigidbody2D>();
        colour = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        x = Mathf.RoundToInt(transform.position.x - 0.5f);
        y = Mathf.RoundToInt(transform.position.y - 0.5f);
        colour = GetComponent<SpriteRenderer>().color;
    }
}
