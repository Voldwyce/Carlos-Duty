using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using Photon.Realtime;

public class Weapon : MonoBehaviour
{
    // Variables públicas para el daño, la cámara, y la cadencia de fuego
    public int damage;
    public Camera camera;
    public float fireRate;

    // Efecto visual de impacto
    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire; // Tiempo hasta el próximo disparo

    // Munición
    [Header("Ammo")]
    public int mag = 5; // Munición actual en el cargador
    public int ammo = 30; // Munición total disponible
    public int magAmmo = 30; // Capacidad del cargador

    // Sonido
    [Header("SFX")]
    public int shootSFXIndex;
    public PlayerPhotonSoundMG playerPhotonSoundMG;
    public AudioClip reloadSound;

    // Interfaz de usuario
    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    // Animación
    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    // Retroceso
    [Header("Recoil")]
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

    public GameObject ammoBox; // Referencia al GameObject de la caja de munición

    void Start()
    {
        // Configuración inicial
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        originalPos = transform.localPosition;
        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {
        // Pausar el juego
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }

        // Reanudar el juego
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1;
        }

        // Actualizar UI
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        // Control de disparo
        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && animation.isPlaying == false)
        {
            Debug.Log("Fire");
            nextFire = 1 / fireRate;
            ammo--;
            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
            Fire();
        }

        // Recargar
        if (Input.GetKeyDown(KeyCode.R) && mag > 0)
        {
            Reload();
        }

        // Retroceso
        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }
    }

    // Método para recargar
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

    // Método para disparar
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
                    PhotonNetwork.LocalPlayer.AddScore(1);
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, (object)damage);
            }
        }
    }

    // Método para retroceder
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

    // Método para recuperarse del retroceso
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

    // Método para recoger munición de la caja
    public void RecogerMunición(int cantidad)
    {
        // Agregar la munición al cargador
        mag += cantidad;
        // Asegurarse de que la cantidad de munición en el cargador no supere el límite
        mag = Mathf.Min(mag, magAmmo);
        // Desactivar la caja de munición después de recoger la munición
        if (ammoBox != null)
        {
            ammoBox.SetActive(false);
        }
    }
}
