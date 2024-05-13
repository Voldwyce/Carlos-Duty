using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;

/// <summary>
/// Gestiona la interfaz de usuario del marcador y la actualiza con los puntajes y estadísticas de los jugadores.
/// </summary>
public class Leaderboard : MonoBehaviour
{
    // Referencia al contenedor de jugadores
    public GameObject playersHolder;

    // Opciones de actualización del marcador
    [Header("Options")]
    public float updateRate = 1f;

    // Referencias a los elementos de la interfaz de usuario
    [Header("UI")]
    public GameObject[] slots; // Ranuras para cada jugador en el marcador
    [Space]
    public TextMeshProUGUI[] scoreTexts; // Texto para los puntajes de los jugadores
    public TextMeshProUGUI[] usernameTexts; // Texto para los nombres de usuario de los jugadores
    public TextMeshProUGUI[] kdTexts; // Texto para las estadísticas de asesinatos/muertes de los jugadores

    private void Start()
    {
        // Invocar repetidamente el método Refresh con un retraso inicial y un intervalo de tiempo
        InvokeRepeating(nameof(Refresh), 1f, updateRate);
    }

    // Método para actualizar el marcador
    public void Refresh()
    {
        // Desactivar todas las ranuras del marcador
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Obtener la lista de jugadores ordenada por puntaje descendente
        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;
        // Iterar sobre la lista de jugadores ordenada
        foreach (var player in sortedPlayerList)
        {
            slots[i].SetActive(true); // Activar la ranura del jugador

            // Asignar el nombre de usuario del jugador al texto correspondiente
            if (player.NickName == "")
                player.NickName = "Player";
            usernameTexts[i].text = player.NickName;

            // Asignar el puntaje del jugador al texto correspondiente
            scoreTexts[i].text = player.GetScore().ToString();

            // Obtener y asignar las estadísticas de asesinatos/muertes del jugador al texto correspondiente
            if (player.CustomProperties["kills"] != null)
            {
                kdTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
            }
            else
            {
                kdTexts[i].text = "0/0"; // Si no hay estadísticas disponibles, mostrar 0/0
            }

            i++; // Incrementar el índice de la ranura del jugador
        }
    }

    private void Update()
    {
        // Activar o desactivar el contenedor de jugadores según si se presiona la tecla Tab
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
