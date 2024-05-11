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

    public int health;
    public bool isLocalPlayer;

    public RectTransform healthBar;
    private float originalHealthSize;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    private void Start()
    {
        originalHealthSize = healthBar.sizeDelta.x;
    }

    [PunRPC]
    public void TakeDamage(int _damage)
    {
        health -= _damage;

        healthBar.sizeDelta = new Vector2(originalHealthSize * health / 100f, healthBar.sizeDelta.y);


        healthText.text = health.ToString();

        if (health <= 0)
        {

            if (isLocalPlayer)
            {
                RoomManager.instance.Respawn();
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }


            Destroy(gameObject);

        }

    }

}
