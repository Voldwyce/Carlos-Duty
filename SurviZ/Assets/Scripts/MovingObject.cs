using UnityEngine;

public class MovimientoDerechaIzquierda : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento del objeto
    public float distanciaMaxima = 10f; // Distancia máxima que el objeto puede recorrer

    private float distanciaRecorrida = 0f; // Distancia recorrida por el objeto desde el inicio
    private bool moviendoseDerecha = true; // Indica si el objeto está moviéndose hacia la derecha

    void Update()
    {
        // Calcula la distancia recorrida desde el último frame
        float distanciaFrame = velocidad * Time.deltaTime;

        // Si el objeto está moviéndose hacia la derecha
        if (moviendoseDerecha)
        {
            // Mueve el objeto hacia la derecha
            transform.Translate(Vector3.right * distanciaFrame);

            // Actualiza la distancia recorrida
            distanciaRecorrida += distanciaFrame;

            // Si el objeto alcanza la distancia máxima, cambia de dirección
            if (distanciaRecorrida >= distanciaMaxima)
            {
                moviendoseDerecha = false;
            }
        }
        // Si el objeto está moviéndose hacia la izquierda
        else
        {
            // Mueve el objeto hacia la izquierda
            transform.Translate(Vector3.left * distanciaFrame);

            // Actualiza la distancia recorrida
            distanciaRecorrida -= distanciaFrame;

            // Si el objeto vuelve a su posición inicial, cambia de dirección
            if (distanciaRecorrida <= 0f)
            {
                moviendoseDerecha = true;
            }
        }
    }
}
