using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // Variable estática para acceder a este script desde otros scripts
    public static MouseLook instance;

    // Configuración pública visible en el inspector de Unity
    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180); // Límites de rotación en grados
    public bool lockCursor = true; // Determina si el cursor del ratón debe estar bloqueado
    [Space]
    private Vector2 sensitivity = new Vector2(2, 2); // Sensibilidad del ratón
    [Space]
    public Vector2 smoothing = new Vector2(3, 3); // Suavidad del movimiento del ratón

    [Header("First Person")]
    public GameObject characterBody; // Cuerpo del personaje asociado a la cámara

    // Direcciones objetivo para la rotación de la cámara y el cuerpo del personaje
    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    // Variables para almacenar la posición del ratón
    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;

    private Vector2 mouseDelta; // Cambio en la posición del ratón en el cuadro actual

    [HideInInspector]
    public bool scoped; // Variable oculta para determinar si la cámara está en modo de zoom

    void Start()
    {
        instance = this; // Asigna esta instancia como la instancia actual de MouseLook

        // Establece la dirección objetivo de la cámara a su orientación inicial
        targetDirection = transform.localRotation.eulerAngles;

        // Establece la dirección objetivo del cuerpo del personaje a su estado inicial
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
        
        // Bloquea el cursor si lockCursor está activado
        if (lockCursor)
            LockCursor();
    }

    // Método para bloquear el cursor del ratón
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Oculta y bloquea el cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Convierte la dirección objetivo en cuaterniones
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Obtiene la entrada del ratón sin suavizar para una lectura más limpia
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Escala la entrada según la sensibilidad y la suaviza
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpola el movimiento del ratón en el tiempo para aplicar suavidad
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Calcula la posición absoluta del ratón
        _mouseAbsolute += _smoothMouse;

        // Limita la rotación en los ejes x e y
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        // Aplica la rotación a la cámara y al cuerpo del personaje si está presente
        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}
