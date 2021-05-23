using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MEC;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ExitGames.Client.Photon;
using funcoesUteis;

public class MultiplayerLauncher : MonoBehaviourPunCallbacks
{
    public const byte RODA_ANIM_SORTEIO = 2;

    #region Public Fields
    public GameObject imgPlayer1;
    public GameObject imgPlayer2;
    public GameObject imgCarregando;
    public Button btnBuscar;
    public Button btnCancelar;
    //------------------------------------------------
    public List<Sprite> animSorteio = new List<Sprite>();
    public List<Sprite> animMoeda1 = new List<Sprite>();
    public List<Sprite> animMoeda2 = new List<Sprite>();

    public Sprite resultadoVoce, resultadoOponente;
    //-------------------------------------------------
    public GameObject panelSorteio;
    public Image imgAnimSorteio;
    public Image imgAnimMoeda;
    public Image imgResultado;

    ////ATENÇÃO!!
    ///adicionado gerenciadorAudio remover antes de juntar
    ///
    public AudioBase gerenciadorAudio;



    #endregion

    #region Private Serializable Fields
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;



    #endregion


    #region Private Fields


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";


    #endregion


    #region MonoBehaviour CallBacks




    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        ////ATENÇÃO!!
        ///adicionado gerenciadorAudio remover antes de juntar
        ///
        gerenciadorAudio = Main.Instance.audioBase;

        //gerenciadorAudio.playMusicaTelaBatalha();


        imgPlayer1.SetActive(false);
        imgPlayer2.SetActive(false);
        imgCarregando.SetActive(false);
        btnCancelar.interactable = true;


        animSorteio.AddRange(Resources.LoadAll<Sprite>("imagens/Sorteio/SorteioSprite1").ToList());
        animSorteio.AddRange(Resources.LoadAll<Sprite>("imagens/Sorteio/SorteioSprite2").ToList());
        animSorteio.AddRange(Resources.LoadAll<Sprite>("imagens/Sorteio/SorteioSprite3").ToList());

        animMoeda1.AddRange(Resources.LoadAll<Sprite>("imagens/Sorteio/MoedaSprite1").ToList());
        animMoeda2.AddRange(Resources.LoadAll<Sprite>("imagens/Sorteio/MoedaSprite2").ToList());


    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += eventoRecebidoViaRede;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= eventoRecebidoViaRede;
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    private void eventoRecebidoViaRede(EventData obj)
    {

        if (obj.Code == RODA_ANIM_SORTEIO)
        {
            rodaAnimSorteio();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                //ATENÇÃO - RODA A ANIMAÇÃO DA MOEDA E CHAMA NOS OUTROS CELULARES
                
                object[] valoresRede = new object[] { 0, 0 };
                PhotonNetwork.RaiseEvent(RODA_ANIM_SORTEIO, null, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                rodaAnimSorteio();
            }
        }
    }


    #endregion



    #region Public Methods


    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        Debug.Log("inicio conect");
        imgPlayer1.SetActive(true);
        imgCarregando.SetActive(true);
        btnBuscar.interactable = false;
        btnCancelar.interactable = true;

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Conectado! entrando em sala aleatoria");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("Não conectado! Conectando servidor");
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    //public void Disconect(string level)
    //{
    //    Timing.RunCoroutine(chamaLevel(level));
    //}
    public void Disconect()
    {
        imgPlayer1.SetActive(false);
        imgPlayer2.SetActive(false);
        imgCarregando.SetActive(false);
        btnCancelar.interactable = true;
        btnBuscar.interactable = true;
        PhotonNetwork.Disconnect();
    }

    public IEnumerator<float> chamaLevel(string level)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return Timing.WaitForOneFrame;
        SceneManager.LoadScene(level);
    }

    public void rodaAnimSorteio()
    {
        //ATENÇÃO - inicia o carregamento da cena
        AsyncOperation carregamentoCena = SceneManager.LoadSceneAsync("MultiplayerBatalha");
        carregamentoCena.allowSceneActivation = false;

        List<Sprite> animMoedaCompleta = animMoeda1;
        if (PhotonNetwork.IsMasterClient)
        {
            imgResultado.sprite = resultadoVoce;
        }
        else
        {
            animMoedaCompleta.AddRange(animMoeda2);
            imgResultado.sprite = resultadoOponente;
        }


        imgResultado.ZeraAlfa();
        imgAnimMoeda.ZeraAlfa();
        panelSorteio.GetComponent<Image>().color = new Color(panelSorteio.GetComponent<Image>().color.r, panelSorteio.GetComponent<Image>().color.g, panelSorteio.GetComponent<Image>().color.b, 0);
        panelSorteio.SetActive(true);

        Sequence s = DOTween.Sequence();
        s.Append(panelSorteio.GetComponent<Image>().DOFade(1, .3f));
        s.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(imgAnimSorteio, animSorteio, false, 6, false, () =>
            {

                carregamentoCena.allowSceneActivation = true; //ATENÇÃO - TROCA DE CENA QUANDO TERMINA A ANIMAÇÃO

            });
        });
        s.AppendInterval(1);
        s.Append(imgAnimMoeda.DOFade(1, .2f));
        s.AppendCallback(() =>
        {
            Main.Instance.StopAllMusics();
            FuncoesUteis.animacaoImagem(imgAnimMoeda, animMoedaCompleta, false, 6);
            gerenciadorAudio.playMoedaSorteio();
        });
        s.AppendInterval(2.2f);
        s.Append(imgResultado.DOFade(1, .2f));
        s.AppendInterval(2.5f);
        s.Append(imgResultado.DOFade(0, .2f));
        s.Join(imgAnimMoeda.DOFade(0, .2f));

    }


    #endregion
}