using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using Photon.Realtime;

public class Weapon : MonoBehaviour
{

    public int damage;
    public Camera camera;
    public float fireRate;

    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire;

    [Header("Ammo")]
    public int mag = 5;

    public int ammo = 30;
    public int magAmmo = 30;

    [Header("SFX")] public int shootSFXIndex;
    public PlayerPhotonSoundMG playerPhotonSoundMG;
    public AudioClip reloadSound;


    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    [Header("Recoil")]
    // [Range(0, 1)]
    // public float recoilPercent = 0.3f;

    [Range(0, 2)]
    public float recoverPercent = 0.7f;
    [Space]
    public float recoilUp = 1f;
    public float recoilBack = 0f;


    private Vector3 originalPos;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    public bool recovering;

    void Start()
    {

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        originalPos = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {

       // Si pulsamos la tecla p se pausa el juego
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;

        }

        // Si pulsamos la tecla o se reanuda el juego
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        if (nextFire > 0)
            nextFire -= Time.deltaTime;


        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && animation.isPlaying == false)
        {
            // Log
            Debug.Log("Fire");
            nextFire = 1 / fireRate;
            ammo--;

            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;

            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && mag > 0)
        {
            Reload();
        }

        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }
    }

    void Reload()
    {

        animation.Play(reload.name);
        playerPhotonSoundMG.gunShotSource.clip = reloadSound;
        playerPhotonSoundMG.gunShotSource.Play();


        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

    }

    void Fire()
    {

        recoiling = true;
        recovering = false;

        playerPhotonSoundMG.PlayGunShotSFX(shootSFXIndex);

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if (hit.transform.gameObject.GetComponent<Health>())
            {

                PhotonNetwork.LocalPlayer.AddScore(damage);

                if (damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    // PhotonNetwork.LocalPlayer.AddScore(1);
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, (object)damage);
            }
        }

    }

    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPos.x, originalPos.y + recoilUp, originalPos.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPosition = originalPos;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }

}