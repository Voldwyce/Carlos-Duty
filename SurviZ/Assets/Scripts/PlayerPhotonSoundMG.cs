using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPhotonSoundMG : MonoBehaviourPunCallbacks
{
    public AudioSource footstepSource;
    public AudioClip footstepSFX;

    public AudioSource gunShotSource;
    public AudioClip[] allGunShotSFX;

    [PunRPC]
    private void PlayFootstepSFXRPC(float pitch, float volume)
    {
        footstepSource.clip = footstepSFX;
        footstepSource.pitch = pitch;
        footstepSource.volume = volume;
        footstepSource.Play();
    }

    [PunRPC]
    private void PlayGunShotSFXRPC(int index, float pitch, float volume)
    {
        gunShotSource.clip = allGunShotSFX[index];
        gunShotSource.pitch = pitch;
        gunShotSource.volume = volume;
        gunShotSource.Play();
    }

    public void PlayFootstepSFX()
    {
        photonView.RPC("PlayFootstepSFXRPC", RpcTarget.All, UnityEngine.Random.Range(0.7f, 1.2f), UnityEngine.Random.Range(0.2f, 0.35f));
    }

    public void PlayGunShotSFX(int index)
    {
        photonView.RPC("PlayGunShotSFXRPC", RpcTarget.All, index, UnityEngine.Random.Range(0.7f, 1.2f), UnityEngine.Random.Range(0.2f, 0.35f));
    }
}
