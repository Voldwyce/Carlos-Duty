using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MovimentPersona : MonoBehaviour
{
    public float speed = 30000f; // Velocidad de movimiento
    public float maxSpeed = 4f;
    public float forçaSalt = 5f;
    public float escalaRotacion = 60;

    public GameObject ModeloBala; //es la bala que se copiará. Lo normal es tenerla no activa (cuadrillo al lado del nombre del objeto)
    public float velocidadBala = 30;

    public float distOrigenDisparo = 1;
    public float alturaDisparo = 1;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
   
    void Update() //para moverse modificando la dirección
    {
        // Obtener las entradas del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if( Input.GetKeyDown(KeyCode.B) ) { //con la B se dispara una bala
            GameObject bala = Instantiate(ModeloBala, transform.position + transform.forward * distOrigenDisparo, transform.rotation); //creamos la bala y la posicionamos
            bala.transform.position = new Vector3(bala.transform.position.x, alturaDisparo, bala.transform.position.z ); //la situamos a cierta altura del suelo
            
            bala.GetComponent<Rigidbody>().velocity = transform.forward * velocidadBala; //le damos velocidad
            bala.SetActive(true); //la activamos que se vea
            Destroy(bala, 10f); //al cabo de un tiempo, la matamos

            //podemos ponerle a la bala un script que al tocar ciertos objetos haga algo, como destruirlos, etc.
        }

        if (horizontalInput != 0)
        {
            //rota
            transform.Rotate(Vector3.up, horizontalInput * escalaRotacion * Time.deltaTime);
        }

        if (verticalInput != 0)
        {
            //transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
            rb.AddRelativeForce(Vector3.forward * (speed * Time.deltaTime));
        }
        else
        {
            //rb.velocity = new Vector3( 0f, rb.velocity.y, 0f); //si no hi ha demanda de moviment, la velocitat horitzontal és zero
        }

        /*float velocitatHoritzontal = Mathf.Sqrt((rb.velocity.x* rb.velocity.x) + (rb.velocity.y * rb.velocity.y) );

        if ( velocitatHoritzontal > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed; //màxima velocitat en horitzontal
        }
        else
        {
            if (rb.velocity.magnitude < -maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * -maxSpeed; //màxima velocitat
            }
        }*/
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("¡Colisión con el jugador! " + collision.gameObject.tag);

        switch (collision.gameObject.tag)
        {
            case "cubito":
                Destroy(collision.gameObject, 1);
                break;
        }
    }
    
}