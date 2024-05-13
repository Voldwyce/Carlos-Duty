using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float maxSpeed = 10f;
    [Space]
    public float airControl = 0.5f;
    [Space]
    public float jumpHeight = 5f;
    public float jumpCooldown = 0.5f; // Tiempo de cooldown entre saltos
    public float crouchSpeed = 2f; // Velocidad al agacharse
    public float crouchHeight = 0.5f; // Altura al agacharse

    private Rigidbody rb;
    private bool isRunning;
    private bool jumping;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private float lastJumpTime;

    // Variables para suavizar el movimiento después de la colisión
    private Vector3 smoothVelocity;
    public float smoothTime = 0.1f;

    [Header("Animation")]
    public Animation handAnimation;
    public AnimationClip handWalkAnimations;
    public AnimationClip idleAnimation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detectar entrada del jugador
        isRunning = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump");
        isCrouching = Input.GetKey(KeyCode.C); // Verificar si se está agachando

        // Verificar si se puede saltar nuevamente basado en el tiempo de enfriamiento
        if (Time.time - lastJumpTime >= jumpCooldown && jumping)
        {
            Jump();
            lastJumpTime = Time.time;
        }
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
        // Aplicar movimiento
        Move();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        float speed = isRunning ? sprintSpeed : walkSpeed;
        if (!isGrounded)
            speed *= airControl;

        // Aplicar velocidad reducida si está agachado
        if (isCrouching)
            speed = crouchSpeed;

        handAnimation.clip = handWalkAnimations;
        handAnimation.Play();

        // Calcular la altura del jugador (agachado o no)
        float height = isCrouching ? crouchHeight : 1f;
        transform.localScale = new Vector3(1, height, 1);

        // Aplicar suavizado solo si está en el suelo
        Vector3 targetVelocity = transform.forward * input.y * speed + transform.right * input.x * speed;
        targetVelocity.y = rb.velocity.y;

        if (isGrounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, smoothTime);
        }
        else
        {
            rb.velocity = targetVelocity;
        }

        // Si no hay entrada de movimiento, detener al jugador más rápidamente
        if (input.magnitude < 0.5f && isGrounded)
            rb.velocity *= 0.8f;

        // Reproducir animación de caminar si hay movimiento y está en el suelo
        if (input.magnitude >= 0.5f && isGrounded)
        {
            handAnimation.clip = handWalkAnimations;
            handAnimation.Play();
        }
        // Reproducir animación de inactividad si no hay entrada de movimiento y está en el suelo
        else if (input.magnitude < 0.5f && isGrounded)
        {
            handAnimation.clip = idleAnimation;
            handAnimation.Play();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        isGrounded = false; // Asegurar que no se detecte como en el suelo después del salto
    }
}
