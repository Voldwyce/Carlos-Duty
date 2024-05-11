using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro; // Añade este namespace

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject playersHolder;

    public GameObject player;
    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUi;
    public GameObject connectUi;

    public TextMeshProUGUI timerText; // Referencia al objeto de texto
    private string username = "Player";

    public string roomNameToJoin = "Lobby";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;
    private float gameStartTime;
    public float gameTime = 300f; // Duración total del juego en segundos

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameStartTime = Time.time;
    }

    public void ChangeUsername(string _username)
    {
        username = _username;
    }

    public void JoinRoomButton()
    {
        Debug.Log("Conectando...");
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        nameUi.SetActive(false);
        connectUi.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Spawning...");
        roomCam.SetActive(false);
        Respawn();
    }

    public void Respawn()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;
        _player.GetComponent<PhotonView>().RPC("SetUsername", RpcTarget.AllBuffered, username);
        PhotonNetwork.LocalPlayer.NickName = username;
    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash.Add("kills", kills);
            hash.Add("deaths", deaths);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
        }
    }

    void Update()
    {
        float timeRemaining = gameTime - (Time.time - gameStartTime);
        if (timeRemaining <= 0f)
        {
            StartCoroutine(PauseGame());
        }
        else
        {
            // Actualiza el texto del temporizador
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    IEnumerator PauseGame()
    {
        playersHolder.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;

        // Reiniciamos el ladeboard y las kd
        playersHolder.SetActive(false);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Hashtable hash = player.CustomProperties;
            hash["kills"] = 0;
            hash["deaths"] = 0;
            player.SetCustomProperties(hash);
        }

        gameStartTime = Time.time;
    }
}
