using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float maxSpeed = 10f;
    [Space]
    public float airControl = 0.5f;
    [Space]
    public float jumpHeight = 5f;

    private Rigidbody rb;
    private bool isRunning;
    private bool jumping;
    private bool isGrounded = false;

    // Variables para suavizar el movimiento después de la colisión
    private Vector3 smoothVelocity;
    public float smoothTime = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detectar entrada del jugador
        isRunning = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump");
    }

    private void OnCollisionStay(Collision collision)
    {
        // Verificar si el jugador está en el suelo
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void FixedUpdate()
    {
        // Aplicar movimiento y salto
        Move();
        Jump();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        float speed = isRunning ? sprintSpeed : walkSpeed;
        if (!isGrounded)
            speed *= airControl;

        Vector3 targetVelocity = transform.forward * input.y * speed + transform.right * input.x * speed;
        targetVelocity.y = rb.velocity.y;

        // Aplicar suavizado
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, smoothTime);

        // Si no hay entrada de movimiento, detener al jugador más rápidamente
        if (input.magnitude < 0.5f && isGrounded)
            rb.velocity *= 0.8f;
    }

    void Jump()
    {
        if (jumping && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            isGrounded = false; // Asegurar que no se detecte como en el suelo después del salto
        }
    }
}
