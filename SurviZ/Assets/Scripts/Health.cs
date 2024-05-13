using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Variable para la salud
    public int health;
    // Variable para determinar si este jugador es el jugador local
    public bool isLocalPlayer;

    // Referencia al RectTransform de la barra de salud
    public RectTransform healthBar;
    private float originalHealthSize; // Tamaño original de la barra de salud

    // Interfaz de usuario
    [Header("UI")]
    public TextMeshProUGUI healthText; // Texto para mostrar la salud

    private void Start()
    {
        // Almacenar el tamaño original de la barra de salud al inicio
        originalHealthSize = healthBar.sizeDelta.x;
    }

    // Método RPC para recibir daño
    [PunRPC]
    public void TakeDamage(int _damage)
    {
        // Reducir la salud
        health -= _damage;

        // Actualizar el tamaño de la barra de salud en función de la salud actual
        healthBar.sizeDelta = new Vector2(originalHealthSize * health / 100f, healthBar.sizeDelta.y);

        // Actualizar el texto de salud
        healthText.text = health.ToString();

        // Comprobar si la salud es igual o menor que cero
        if (health <= 0)
        {
            // Si es el jugador local, hacer respawn y actualizar las estadísticas de muerte
            if (isLocalPlayer)
            {
                RoomManager.instance.Respawn();
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }

            // Destruir el objeto
            Destroy(gameObject);
        }
    }
}
