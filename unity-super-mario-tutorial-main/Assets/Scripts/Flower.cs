using System.Collections;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public float riseDistance = 2f;
    public float riseSpeed = 1f;
    private bool isRising = false;
    private bool isPlayerNearby = false;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - riseDistance, transform.position.z);
    }

    private void Update()
    {
        if (isPlayerNearby && !isRising)
        {
            StartCoroutine(RiseUp());
        }
    }

    private IEnumerator RiseUp()
    {
        isRising = true;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseDistance;

        float elapsedTime = 0f;
        while (elapsedTime < riseSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / riseSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        isRising = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.Hit();
                Debug.Log("Player hit by flower!");
            }
        }
    }
}
