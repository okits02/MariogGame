using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Collider2D capsuleCollider;
    private Player player;


    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public int score = 0;  
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    public TextMeshProUGUI scoreText;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<Collider2D>();
        player = GetComponent<Player>();
        score = 0;
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
        capsuleCollider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable()
    {
        rb.isKinematic = true;
        capsuleCollider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        HorizontalMovement();

        grounded = rb.Raycast(Vector2.down);

        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();

        scoreText.text = "Score: " + score.ToString(); 

        if (player != null && player.IsFireState())
        {
            PlayerAttack();
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rb.MovePosition(position);
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        if (rb.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0f;
        }

        if (velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            jumping = true;
        }
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Quaternion fireballRotation;
            Vector2 direction;

            if (transform.eulerAngles.y == 0)
            {
                fireballRotation = Quaternion.Euler(0, 0, 0);
                direction = Vector2.right;
                Debug.Log("Bắn sang phải");
            }
            else
            {
                fireballRotation = Quaternion.Euler(0, 180, 0);
                direction = Vector2.left;
                Debug.Log("Bắn sang trái");
            }

            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballRotation);

            Fireball fireballScript = fireball.GetComponent<Fireball>();
            if (fireballScript == null)
            {
                fireballScript = fireball.AddComponent<Fireball>();
            }
            fireballScript.Initialize(direction, fireballSpeed);
            Debug.Log($"Fireball created with speed: {fireballSpeed}, direction: {direction}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y > 0.5f)
            {
                velocity.y = jumpForce / 2f;
                jumping = true;

                score += 100;
                Debug.Log("Score: " + score);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
        {
            score += 50;
            Destroy(collision.gameObject);
            Debug.Log("Power-up collected! Score: " + score);
        }
    }
}
