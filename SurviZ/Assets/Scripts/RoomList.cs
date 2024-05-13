using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{
    // Instancia estática para acceder desde otras clases
    public static RoomList Instance;

    // Referencia al GameObject y script del administrador de la sala
    public GameObject roomManagerGameObject;
    public RoomManager roomManager;

    [Header("UI")]
    public Transform roomListContent;
    public GameObject roomListingPrefab;

    // Lista para almacenar la lista de salas en caché
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    // Método para cambiar el nombre de la sala a unirse
    public void ChangeRoomToCreateName(string _lobbyName)
    {
        roomManager.roomNameToJoin = _lobbyName;
    }

    // Inicialización
    private void Awake()
    {
        Instance = this;
    }

    // Conexión a Photon cuando el juego inicia
    private IEnumerator Start()
    {
        // Asegurarse de no estar en una sala y desconectado antes de conectar a Photon
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        // Esperar hasta que esté completamente desconectado antes de conectar a Photon
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        // Conectar a Photon utilizando la configuración actual
        PhotonNetwork.ConnectUsingSettings();
    }

    // Llamado cuando se conecta al servidor de Photon
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // Unirse al lobby después de conectarse al servidor
        PhotonNetwork.JoinLobby();
    }

    // Llamado cuando se actualiza la lista de salas
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Si la lista de salas en caché está vacía, simplemente copiar la lista recibida
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            // Si no está vacía, actualizar las entradas de la lista de salas en caché
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

        // Actualizar la interfaz de usuario con la lista de salas actualizada
        UpdateUI();
    }

    // Actualizar la interfaz de usuario con la lista de salas en caché
    void UpdateUI()
    {
        // Eliminar todas las entradas de la lista de salas en la interfaz de usuario
        foreach (Transform roomItem in roomListContent)
        {
            Destroy(roomItem.gameObject);
        }

        // Agregar nuevas entradas para cada sala en la lista de salas en caché a la interfaz de usuario
        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListingPrefab, roomListContent);

            // Actualizar los textos de la entrada con el nombre de la sala y el número de jugadores
            roomItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = room.PlayerCount + " /6";

            // Asignar el nombre de la sala a un componente de botón en la entrada
            roomItem.GetComponent<RoomItemButton>().LobbyName = room.Name;
        }
    }

    // Método para unirse a una sala por nombre
    public void JoinLobbyByName(string _name)
    {
        // Establecer el nombre de la sala a unirse en el administrador de la sala y activar el GameObject del administrador de la sala
        roomManager.roomNameToJoin = _name;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
