using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime = 3f;

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
        Debug.Log($"Fireball initialized - Direction: {direction}, Speed: {speed}");
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Vector3 movement = new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
        transform.position += movement;

        // Debug log position
        if (Time.frameCount % 60 == 0) 
        {
            Debug.Log($"Fireball position: {transform.position}, Speed: {speed}, Direction: {direction}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Fireball hit: {other.gameObject.name}");

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
