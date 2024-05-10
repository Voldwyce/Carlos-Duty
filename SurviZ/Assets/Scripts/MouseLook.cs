using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float Sensitivity {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)]
    [SerializeField] float sensitivity = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)]
    [SerializeField] float yRotationLimit = 88f;
    [SerializeField] float zoomSensitivity = 0.5f;
    [SerializeField] float zoomAmount = 20f;
    [SerializeField] float runSpeedMultiplier = 2f;

    Vector2 rotation = Vector2.zero;
    float originalSensitivity;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalSensitivity = sensitivity;
    }

    void Update() {
        // Zoom al hacer clic derecho
        if (Input.GetMouseButton(1)) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomAmount, Time.deltaTime * 2);
        } else {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 2);
        }

        // Aumentar el fov al hacer shift
        if (Input.GetKey(KeyCode.LeftShift)) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomAmount, Time.deltaTime / 2);
        } else {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 2);
        }

        // Rotación horizontal
        float mouseX = Input.GetAxis(xAxis) * sensitivity;
        rotation.y += mouseX;
        transform.localRotation = Quaternion.Euler(0, rotation.y, 0);

        // Rotación vertical pero solo de la cámara
        float mouseY = -Input.GetAxis(yAxis) * sensitivity;
        rotation.x += mouseY;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
    }

    // Método para cambiar la sensibilidad del ratón
    public void ChangeSensitivity(float newSensitivity = -1f) {
        Sensitivity = (newSensitivity == -1f) ? originalSensitivity : newSensitivity;
    }
}
