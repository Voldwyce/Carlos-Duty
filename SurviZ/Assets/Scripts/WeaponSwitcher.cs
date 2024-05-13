using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public PhotonView playerSetupView; // Referencia al PhotonView del jugador
    public Animation _animation; // Referencia a la animación
    public AnimationClip Draw; // Animación de sacar el arma

    private int selectedWeapon = 0; // Índice del arma seleccionada

    void Start()
    {
        SelectWeapon(); // Selecciona el arma al iniciar
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // Cambio de arma con teclas numéricas
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }

        // Cambio de arma con la rueda del ratón
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon += 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon -= 1;
            }
        }

        // Si el arma seleccionada ha cambiado, se selecciona nuevamente
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    // Método para seleccionar el arma actual
    void SelectWeapon()
    {
        // Establece el arma seleccionada para todos los jugadores
        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, (object)selectedWeapon);

        // Si el índice del arma seleccionada es mayor o igual al número de armas, se ajusta para evitar errores
        if (selectedWeapon >= transform.childCount)
        {
            selectedWeapon = transform.childCount - 1;
        }

        // Detiene y reproduce la animación de sacar el arma
        _animation.Stop();
        _animation.Play(Draw.name);

        int i = 0;

        // Activa o desactiva cada arma en función de si es la seleccionada o no
        foreach (Transform _weapon in transform)
        {
            if (i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);
            }
            else
            {
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
