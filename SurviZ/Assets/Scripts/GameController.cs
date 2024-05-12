using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameController : MonoBehaviour
{
    public VideoClip videoClip;
    public Texture2D imageTexture;

    private bool escPressed = false;
    private bool isPlayingVideo = false;

    void OnGUI()
    {
        if (isPlayingVideo)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), imageTexture);
        }
    }

    private float lastEscapeTime = 0f;
    private int escapeCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - lastEscapeTime <= 3f)
            {
                escapeCount++;
            }
            else
            {
                escapeCount = 1;
            }

            lastEscapeTime = Time.time;

            if (escapeCount == 2)
            {
                escPressed = true;
                StartCoroutine(PlayVideoAndQuit());
            }
        }
    }

    IEnumerator PlayVideoAndQuit()
    {

        if (escPressed)
        {
            // Crear un nuevo objeto de reproductor de video
            GameObject videoPlayerObject = new GameObject("VideoPlayer");
            VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

            // Asignar el videoclip al reproductor de video
            videoPlayer.clip = videoClip;

            // Reproducir el video en pantalla completa
            videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            videoPlayer.targetCamera = Camera.main;
            videoPlayer.Play();

            isPlayingVideo = true;

            // Esperar 9 segundos antes de salir
            yield return new WaitForSeconds(9f);

            // Cerrar el juego
            Application.Quit();
        }
    }
}
