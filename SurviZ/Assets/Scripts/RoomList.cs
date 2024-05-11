using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{

    public static RoomList Instance;

    public GameObject roomManagerGameObject;
    public RoomManager roomManager;

    [Header("UI")]
    public Transform roomListContent;
    public GameObject roomListingPrefab;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string _lobbyName)
    {
        roomManager.roomNameToJoin = _lobbyName;
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        // Precauciones

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = new List<RoomInfo>(cachedRoomList);

                        if (room.RemovedFromList)
                        {
                            newList.RemoveAt(i);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;
                    }
                }
            }
        }

        UpdateUI();

    }


    void UpdateUI()
    {

        foreach (Transform roomItem in roomListContent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListingPrefab, roomListContent);

            roomItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = room.PlayerCount + " /6";

            roomItem.GetComponent<RoomItemButton>().LobbyName = room.Name;
        }

    }

    public void JoinLobbyByName(string _name)
    {

        roomManager.roomNameToJoin = _name;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);

    }

}
