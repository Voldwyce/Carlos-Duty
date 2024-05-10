using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class Sway : MonoBehaviour
{

    [Header("Settings")]
    public float swayAmount = 0.09f;
    [Space]
    public float smoothAmount = 3f;

    private Vector3 origin;

    void Start()
    {
        origin = transform.localPosition;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        input.x = Mathf.Clamp(input.x, -swayAmount, swayAmount);
        input.y = Mathf.Clamp(input.y, -swayAmount, swayAmount);

        Vector3 target = new Vector3(-input.x, -input.y, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target + origin, Time.deltaTime * smoothAmount);

    }

}
