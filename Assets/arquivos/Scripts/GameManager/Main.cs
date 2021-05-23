using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    [HideInInspector]
    public static Main Instance;
    [HideInInspector]
    public GameObject UserProfile;
    [HideInInspector]
    public Web web;
    [HideInInspector]
    public AudioBase audioBase;
    [HideInInspector]
    public LoadingState ls;
    [HideInInspector]
    public AdMobManager ad;
    [HideInInspector]
    public config_usuario usuario;
    [HideInInspector]
    public FacebookSetings facebook;
    [HideInInspector]
    public Inventory inventario;

    private GerenciadorBancoLocal banco = new GerenciadorBancoLocal("databaseV2.db");

    public GameObject errorComponnent;
    public GameObject PrefabCarta;

    public List<Monstro> DeckInicial;

    public GerenciadorBancoLocal Banco { get => banco; set => banco = value; }
    
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetDefaults();
    }

    #region Loadings
    public void MainMenu()
    {
        LoadingMenu();
        ls = LoadingState.MENU_INICIAL;
    }

    void LoadingMenu()
    {

        SceneManager.LoadScene(2);
    }
    public void Tutorial1Menu()
    {
        LoadingMenu();
        ls = LoadingState.TUTORIAL1;

    }

    public void Tutorial2Menu()
    {
        LoadingMenu();
        ls = LoadingState.TUTORIAL2;

    }
    public void MultiplayerMenu()
    {
        LoadingMenu();
        ls = LoadingState.MULTIPLAYER;

    }
    #endregion

    public void SetDefaults()
    {
        web = GetComponent<Web>();
        web = GetComponent<Web>();
        audioBase = GetComponent<AudioBase>();
        ad = GetComponent<AdMobManager>();
        usuario = GetComponent<config_usuario>();
        facebook = GetComponent<FacebookSetings>();
        inventario = GetComponent<Inventory>();
    }

    public void ShowError(string msg)
    {
        errorComponnent.GetComponent<Erro_Tela>().MensagemErro(msg);
        Instantiate(errorComponnent, FindObjectOfType<Canvas>().transform);
    }

    public void StopAllMusics()
    {
        audioBase.stopMusicaMenu();
        audioBase.stopMusicaDerrota();
        audioBase.stopMusicaGameplay();
        audioBase.stopMusicaLobby();
        audioBase.stopMusicaTutorial();
        audioBase.stopMusicaVitoria();
        audioBase.stopMusicaTelaBatalha();
    }
}
