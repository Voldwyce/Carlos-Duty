using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;
    public GameObject camera;
    public string username;

    public TextMeshPro usernameText;

    public Transform TPWeapon;

    public void IsLocalPlayer()
    {
        TPWeapon.gameObject.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);

    }

    [PunRPC]
    public void SetTPWeapon(int _weaponIndex)
    {
        foreach (Transform _weapon in TPWeapon)
        {
            _weapon.gameObject.SetActive(false);
        }

        TPWeapon.GetChild(_weaponIndex).gameObject.SetActive(true);
    }

    [PunRPC]
    public void SetUsername(string _username)
    {
        username = _username;

        usernameText.text = username;
    }


    public void SetHashes()
    {

        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash.Add("kills", RoomManager.instance.kills);
            hash.Add("deaths", RoomManager.instance.deaths);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
        }
    }

};
