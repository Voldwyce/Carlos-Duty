using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float maxSpeed = 10f;
    public float airControl = 0.5f;
    public float jumpHeight = 5f;

    private Rigidbody rb;
    private bool isRunning;
    private bool jumping;
    private bool isGrounded = false;
    private int selectedWeapon = 0;
    private Animation _animation;
    private PhotonView playerSetupView;

    public AnimationClip handWalkAnimations;
    public AnimationClip idleAnimation;
    public AnimationClip drawAnimation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animation = GetComponent<Animation>();
        playerSetupView = GetComponent<PhotonView>();
        SelectWeapon(selectedWeapon);
    }

    void Update()
    {
        isRunning = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump");

        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon += 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon -= 1;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon(selectedWeapon);
        }
    }

    void FixedUpdate()
    {
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

        _animation.clip = handWalkAnimations;
        _animation.Play();

        Vector3 targetVelocity = transform.forward * input.y * speed + transform.right * input.x * speed;
        targetVelocity.y = rb.velocity.y;

        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 0.1f);

        if (input.magnitude < 0.5f && isGrounded)
        {
            _animation.clip = idleAnimation;
            _animation.Play();
        }
    }

    void Jump()
    {
        if (jumping && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            isGrounded = false;
        }
    }

    void SelectWeapon(int weaponIndex)
    {
        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, (object)weaponIndex);

        _animation.Stop();
        _animation.Play(drawAnimation.name);

        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == weaponIndex)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
