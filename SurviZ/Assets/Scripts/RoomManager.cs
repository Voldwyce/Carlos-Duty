using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable; // Alias para evitar conflicto de nombres con System.Collections.Hashtable
using TMPro; // Namespace necesario para usar TextMeshPro

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject playersHolder;

    public GameObject player; // Prefabricado del jugador
    [Space]
    public Transform[] spawnPoints; // Puntos de aparición para los jugadores

    [Space]
    public GameObject roomCam; // Cámara de la sala

    [Space]
    public GameObject nameUi; // Interfaz de usuario para ingresar nombre de usuario
    public GameObject connectUi; // Interfaz de usuario para conectarse a la sala

    public TextMeshProUGUI timerText; // Referencia al objeto de texto para el temporizador
    private string username = "Player"; // Nombre de usuario por defecto

    public string roomNameToJoin = "Lobby"; // Nombre de la sala por defecto

    [HideInInspector]
    public int kills = 0; // Contador de asesinatos
    [HideInInspector]
    public int deaths = 0; // Contador de muertes
    private float gameStartTime; // Tiempo de inicio del juego
    public float gameTime = 300f; // Duración total del juego en segundos

    // Variable para almacenar el último punto de aparición utilizado por cada jugador
    private Dictionary<int, Transform> lastSpawnPoints = new Dictionary<int, Transform>();

    void Awake()
    {
        instance = this; // Establece la instancia única de RoomManager
    }

    void Start()
    {
        gameStartTime = Time.time; // Establece el tiempo de inicio del juego
    }

    // Método para cambiar el nombre de usuario
    public void ChangeUsername(string _username)
    {
        username = _username;
    }

    // Método para unirse a una sala
    public void JoinRoomButton()
    {
        Debug.Log("Conectando..."); // Mensaje de depuración
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null); // Intenta unirse a la sala con el nombre especificado o la crea si no existe
        nameUi.SetActive(false); // Desactiva la interfaz de usuario para ingresar nombre de usuario
        connectUi.SetActive(true); // Activa la interfaz de usuario para conectar a la sala
    }

    // Llamado cuando el jugador se une a una sala
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Spawning..."); // Mensaje de depuración
        roomCam.SetActive(false); // Desactiva la cámara de la sala
        Respawn(); // Respawn del jugador en la sala
    }

    // Método para respawnear al jugador
    public void Respawn()
    {
        // Obtiene el ID único del jugador local
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        // Selecciona un punto de aparición aleatorio que no sea el mismo que el último utilizado por el jugador
        Transform spawnPoint = GetRandomSpawnPoint(playerID);

        // Instancia al jugador en el punto de aparición seleccionado
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

        // Establece al jugador como local
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();

        // Establece la propiedad isLocalPlayer de la salud del jugador como verdadera
        _player.GetComponent<Health>().isLocalPlayer = true;

        // Establece el nombre de usuario del jugador
        _player.GetComponent<PhotonView>().RPC("SetUsername", RpcTarget.AllBuffered, username);

        // Establece el nombre de usuario del jugador local
        PhotonNetwork.LocalPlayer.NickName = username;

        // Actualiza el registro del último punto de aparición utilizado por el jugador
        lastSpawnPoints[playerID] = spawnPoint;
    }

    // Método para seleccionar un punto de aparición aleatorio que no sea el mismo que el último utilizado por el jugador
    private Transform GetRandomSpawnPoint(int playerID)
    {
        // Selecciona un punto de aparición aleatorio
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Verifica si el jugador tiene un registro del último punto de aparición utilizado
        if (lastSpawnPoints.ContainsKey(playerID))
        {
            // Obtiene el último punto de aparición utilizado por el jugador
            Transform lastSpawnPoint = lastSpawnPoints[playerID];

            // Si el punto de aparición aleatorio es el mismo que el último utilizado por el jugador, selecciona uno diferente
            while (spawnPoint == lastSpawnPoint)
            {
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }
        }

        return spawnPoint;
    }

    // Método para establecer las propiedades personalizadas de los jugadores
    public void SetHashes()
    {
        try
        {
            // Obtiene las propiedades personalizadas del jugador local
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            // Actualiza las propiedades personalizadas con los asesinatos y muertes del jugador
            hash["kills"] = kills;
            hash["deaths"] = deaths;
            // Establece las propiedades personalizadas actualizadas para el jugador local
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
        }
    }

    // Método para actualizar el temporizador del juego
    void Update()
    {
        // Calcula el tiempo restante del juego
        float timeRemaining = gameTime - (Time.time - gameStartTime);
        if (timeRemaining <= 0f)
        {
            StartCoroutine(PauseGame()); // Pausa el juego al finalizar el tiempo
        }
        else
        {
            // Actualiza el texto del temporizador mostrando minutos y segundos restantes
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Método para pausar el juego al finalizar el tiempo
    IEnumerator PauseGame()
    {
        // Activa el objeto contenedor de jugadores para mostrar el leaderboard
        playersHolder.SetActive(true);
        Time.timeScale = 0; // Pausa el tiempo del juego
        yield return new WaitForSecondsRealtime(5); // Espera durante 5 segundos en tiempo real
        Time.timeScale = 1; // Reanuda el tiempo del juego

        // Reinicia los marcadores de asesinatos y muertes de todos los jugadores
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Hashtable hash = player.CustomProperties;
            hash["kills"] = 0;
            hash["deaths"] = 0;
            player.SetCustomProperties(hash);
        }

        gameStartTime = Time.time; // Reinicia el tiempo de inicio del juego
    }
}
