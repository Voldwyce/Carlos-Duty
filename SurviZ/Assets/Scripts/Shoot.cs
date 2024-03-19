using UnityEngine;

public class MovimientoPersona : MonoBehaviour
{
    public float speed = 30000f; // Velocidad de movimiento
    public float maxSpeed = 4f;
    public float escalaRotacion = 60;

    public GameObject ModeloBala; // La bala que se copiará. Lo normal es tenerla no activa (cuadrillo al lado del nombre del objeto)
    public float velocidadBala = 30;
    public float distOrigenDisparo = 1;
    public float alturaDisparo = 1;

    private void Update()
    {
        // Obtener las entradas del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0)) // Si se hace clic izquierdo
        {
            Disparar();
        }

        if (horizontalInput != 0)
        {
            // Rotar
            transform.Rotate(Vector3.up, horizontalInput * escalaRotacion * Time.deltaTime);
        }
    }

    private void Disparar()
    {
        // Se dispara una bala
        GameObject bala = Instantiate(ModeloBala, transform.position + transform.forward * distOrigenDisparo, transform.rotation);
        bala.transform.position = new Vector3(bala.transform.position.x, alturaDisparo, bala.transform.position.z);
        bala.GetComponent<Rigidbody>().velocity = transform.forward * velocidadBala;
        bala.SetActive(true);
        Destroy(bala, 10f);
    }
}