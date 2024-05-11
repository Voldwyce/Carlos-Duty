using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
 public string LobbyName;

    public void OnButtonPressed()
    {
        RoomList.Instance.JoinLobbyByName(LobbyName);
    }


}
