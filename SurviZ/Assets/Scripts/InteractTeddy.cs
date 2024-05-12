using UnityEngine;

public class ObjetoInteractivo : MonoBehaviour
{
    public AudioClip sonido;
    private AudioSource audioSource;

    private void Start()
    {
        // Asegúrate de que el objeto tiene un componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = sonido;
    }

    private void Update()
    {
        // Detectar la pulsación de la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Calcular la distancia entre el objeto y la cámara
            float distancia = Vector3.Distance(transform.position, Camera.main.transform.position);
            // Si la distancia es menor que 3 unidades, hacer sonar el audio y desaparecer el objeto
            if (distancia < 3f)
            {
                if (sonido != null)
                {
                    audioSource.Play();
                }
                // Desactivar el objeto después de un breve retraso para permitir que se reproduzca el sonido
                Invoke("DesactivarObjeto", audioSource.clip.length);
            }
        }
    }

    private void DesactivarObjeto()
    {
        gameObject.SetActive(false); // Desactivar el objeto
    }
}
