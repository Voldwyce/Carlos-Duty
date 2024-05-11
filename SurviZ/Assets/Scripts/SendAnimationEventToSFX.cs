using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimationEventToSFX : MonoBehaviour
{

    public PlayerPhotonSoundMG playerPhotonSoundMG;

    public void PlayFootstepSFX()
    {
        playerPhotonSoundMG.PlayFootstepSFX();
    }
}
