using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int cantidadDeMunición = 6; // Cantidad de munición que contiene la caja

    void Update()
    {
        // Detectar la pulsación de la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            RecogerMunición();
        }
    }

    void RecogerMunición()
    {
        // Calcular la distancia entre la caja y la cámara
        float distancia = Vector3.Distance(transform.position, Camera.main.transform.position);
        // Si la distancia es menor que 3 unidades, recoger la munición
        if (distancia < 3f)
        {
            // Recoger munición utilizando el componente Weapon del jugador
            Weapon arma = FindObjectOfType<Weapon>(); // Otra forma más eficiente de obtener la referencia del arma
            if (arma != null)
            {
                arma.RecogerMunición(cantidadDeMunición);
            }
        }
    }


}
