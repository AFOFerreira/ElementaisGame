using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    /// <summary>
    ///script responsável pelo download das informações iniciais do jogo.
    ///responsavel pela criacao do banco de dados.
    /// </summary>

    //referencia para o gerenciador do banco, bem como sua criação.
    GerenciadorBancoLocal _banco = new GerenciadorBancoLocal("databaseV2.db");
    //referencia para a classe responsavel pelos metodos async.
    AsyncOperation async;
    public List<GameObject> cartasLista = new List<GameObject>();
    //referencia aos controladores em cena.
    [Header("Controladores")]
    public GameObject slider;
    public TextMeshProUGUI status;

    [Header("Imagens a serem carregadas")]
    public Image carFogo;
    public Image carAgua;
    public Image carTerra;
    public Image carAr;

    GameObject PrefabCarta;

    //instruções a serem carregadas antes de tudo.
    private void Awake()
    {
        status.text = null;
        slider.SetActive(false);
        PrefabCarta = Main.Instance.PrefabCarta;
    }

    //começo da sequencia de instruções.
    private async void Start()
    {
        //toca a fanfarra de loading.
        Main.Instance.audioBase.playFanfarraLoading();
        //mostra a primeira imagem.
        carFogo.fillAmount = 1;
        //informa que vai realizar um teste de conexão com o servidor.
        status.text = "Conectando ao servidor...";
        var a = await Testarservidor("https://elementaisgame.com/api/carta/read.php");
        var b = await Testarservidor("https://elementaisgame.com/api/elemento/read.php");
        var c = await Testarservidor("https://elementaisgame.com/api/elementais/read.php");
        if (a & b & c)
        {

            //espera 1 segundo.
            await Task.Delay(1000);
            //informa que esta criando o banco local, e começa a criar de fato.
            status.text = "Criando banco local...";
            _banco.CriaBanco();
            //espera 1 segundo.
            await Task.Delay(1000);
            //comeca o download das cartas e informa ao jogador.
            status.text = "Baixando cartas...";
            await FetchCarta("https://elementaisgame.com/api/carta/read.php");
            await FetchElementos("https://elementaisgame.com/api/elemento/read.php");
            await FetchElementais("https://elementaisgame.com/api/elementais/read.php");
            //consulta e cria uma lista das cartas e dos elementos baixados anteriormente.
            status.text = "Consultando cartas";
            var elementos = _banco.ConsultarElementos();
            var cartas = _banco.ConsultarCartas();
            //espera 0,5 segundos.
            await Task.Delay(500);
            //baixa e atualiza as cartas em memoria.
            status.text = "Atualizando elementos";
            foreach (var elemento in elementos)
            {
                Debug.Log("Elemento: " + elemento.nome + "Moldura: " + elemento.moldura);
                await FetchElementoImage(elemento.moldura, elemento);
            }
            status.text = "Atualizando cartas";
            foreach (var carta in cartas)
            {
                await FetchCartaImage(carta.imagemCarta, carta);
            }
            //mostra a segunda imagem.
            carAgua.fillAmount = 1;
            //espera 0,5 segundos.
            await Task.Delay(500);
            //informa que esta comecado a criar o deck
            status.text = "Criando Deck de cartas";
            await CreateDeck();
            //espera 1 segundo.
            await Task.Delay(1000);
            //informa que vai comecar a carregar a cena.
            status.text = "Carregando cena";
            //inicia o loading.
            StartCoroutine(LoadingScreen(1));
        }
        else
        {
            Main.Instance.ShowError("OOPS!!\n parece que houve algum problema com nossa base de dados. \n");
        }
    }

    //metodos para o loading assincrono
    #region Loading
    IEnumerator LoadingScreen(int LVL)
    {
        //recebe o nivel do proximo loading.
        async = SceneManager.LoadSceneAsync(LVL);
        //desativa a proxima cena.
        async.allowSceneActivation = false;
        //enquanto nao estiver conncluido o carregamento.
        while (async.isDone == false)
        {
            //ativa o slider.
            slider.SetActive(true);
            //mostra a terceira imagem.
            carTerra.fillAmount = 1;
            //recebe o progresso.
            float progress = async.progress;
            //recebe o componente slider e informa o progresso.
            slider.GetComponent<Slider>().value = progress;
            //mostra a ultima imagem.
            carAr.fillAmount = 0.9f;
            //se o progresso for igual a 0,9
            if (progress == 0.9f)
                //ativa a proxima cena.
                async.allowSceneActivation = true;
            Debug.Log(progress);
            //encerra a corrotina.
            yield return null;
        }
    }
    #endregion
    //metodos de download das cartas e das imagens
    #region Download Cartas

    async Task<bool> Testarservidor(string url)
    {
        var b = false;
        try
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            await www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                //possivel erro de conexao com a base de dados, verificar o servidor.
                b = false;
            }
            else
            {
                b = true;
            }

        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
        }
        return b;
    }
    async Task FetchCarta(string url)
    {
        try
        {
            UnityWebRequest www = UnityWebRequest.Get(url);

            await Task.Delay(500);
            await www.SendWebRequest();
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.LogError("Erro: " + www.error);
            }
            else
            {
                if (www.isDone)
                {
                    await Task.Delay(500);
                    string resultado = www.downloadHandler.text;
                    JObject objResultado = JObject.Parse(resultado);
                    var resultsTokensList = objResultado["result"];
                    List<CartaAPI> cartasBaixadas = resultsTokensList.Select(p => new CartaAPI
                    {
                        idCarta = (int)p["idCarta"],
                        titulo = (string)p["titulo"],
                        animCampo = (string)p["animCampo"],
                        atualizadoEm = (string)p["atualizadoEm"],
                        descricao = (string)p["descricao"],
                        imagemCarta = (string)p["imagemCarta"],
                        qtdFrames = (string)p["qtdFrames"],
                        raridade = (string)p["raridade"],
                        tamanhoH = (string)p["tamanhoH"],
                        tamanhoV = (string)p["tamanhoV"]
                    }).ToList();

                    for (int i = 0; i < cartasBaixadas.Count; i++)
                    {

                        await Task.Delay(1000);
                        Debug.Log("Inserindo carta: " + cartasBaixadas[i].titulo);
                        _banco.InserirCarta(cartasBaixadas[i]);
                        Debug.Log(cartasBaixadas[i].titulo + " ok");
                        //await Task.Delay(3000);
                    }
                    Debug.Log("CARTAS CRIADAS! :)");

                }
            }

        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message : " + e.Message);

        }


    }
    async Task FetchElementos(string url)
    {
        try
        {
            UnityWebRequest www = UnityWebRequest.Get(url);

            await Task.Delay(500);
            await www.SendWebRequest();
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.LogError("Erro: " + www.error);
                return;
            }
            else
            {
                if (www.isDone)
                {
                    await Task.Delay(500);
                    string resultado = www.downloadHandler.text;
                    JObject objResultado = JObject.Parse(resultado);
                    var resultsTokensList = objResultado["result"];
                    List<ElementoAPI> cartasBaixadas = resultsTokensList.Select(p => new ElementoAPI
                    {
                        animAtaque = (string)p["animAtaque"],
                        animAtaqueQtdFrames = Int32.Parse((string)p["animAtaqueQtdFrames"]),
                        animAtaqueTamanhoH = Int32.Parse((string)p["animAtaqueTamanhoH"]),
                        animAtaqueTamanhoV = Int32.Parse((string)p["animAtaqueTamanhoV"]),
                        animCriarQtdFrames = Int32.Parse((string)p["animCriarQtdFrames"]),
                        animCriar = (string)p["animCriar"],
                        animCriarTamanhoH = Int32.Parse((string)p["animCriarTamanhoH"]),
                        animCriarTamanhoV = Int32.Parse((string)p["animCriarTamanhoV"]),
                        atualizadoEm = (string)p["atualizadoEm"],
                        cor = (string)p["cor"],
                        icone = (string)p["icone"],
                        idElemento = Int32.Parse((string)p["idElemento"]),
                        moldura = (string)p["moldura"],
                        nome = (string)p["nome"]
                    }).ToList();

                    for (int i = 0; i < cartasBaixadas.Count; i++)
                    {
                        await Task.Delay(1000);
                        Debug.Log("Inserindo elemento: " + cartasBaixadas[i].nome);
                        _banco.InserirElemento(cartasBaixadas[i]);
                        Debug.Log("Elemento: " + cartasBaixadas[i].nome + " ok");
                        //await Task.Delay(3000);
                    }
                    Debug.Log("ELEMENTOS CRIADOS! :)");
                }
            }

        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message : " + e.Message);

        }
    }
    async Task FetchElementais(string url)
    {
        try
        {
            UnityWebRequest www = UnityWebRequest.Get(url);

            await Task.Delay(500);
            await www.SendWebRequest();
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.LogError("Erro: " + www.error);

            }
            else
            {
                if (www.isDone)
                {
                    await Task.Delay(500);
                    string resultado = www.downloadHandler.text;
                    JObject objResultado = JObject.Parse(resultado);
                    var resultsTokensList = objResultado["result"];
                    List<ElementalAPI> cartasBaixadas = resultsTokensList.Select(p => new ElementalAPI
                    {
                        ataque = Int32.Parse((string)p["ataque"]),
                        atualizadoEm = (string)p["atualizadoEm"],
                        cristais = Int32.Parse((string)p["cristais"]),
                        defesa = Int32.Parse((string)p["defesa"]),
                        fotoPerfil = (string)p["fotoPerfil"],
                        idCarta = Int32.Parse((string)p["idCarta"]),
                        idElemental = Int32.Parse((string)p["idElemental"]),
                        idElemento = Int32.Parse((string)p["idElemento"]),
                        nivel = Int32.Parse((string)p["nivel"])
                    }).ToList();

                    for (int i = 0; i < cartasBaixadas.Count; i++)
                    {

                        await Task.Delay(1000);
                        Debug.Log("Inserindo elemental: " + cartasBaixadas[i].idElemental);
                        _banco.InserirElemental(cartasBaixadas[i]);
                        Debug.Log(cartasBaixadas[i].idElemental + " ok");
                        //await Task.Delay(3000);
                    }
                    Debug.Log("CARTAS CRIADAS! :)");
                }
            }

        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message : " + e.Message);

        }


    }
    #endregion
    #region Download Imagens
    async Task FetchElementoImage(string url, ElementoAPI elemento)
    {
        try
        {

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();
            if (www.isHttpError || www.isHttpError)
            {
                Main.Instance.ShowError("Ocorreu um erro durante o download das cartas.CODE-01.1-");
            }
            else
            {
                Debug.Log("Elemento: " + elemento.nome + " Moldura: " + elemento.moldura);
                //while (!www.isDone)
                //    progress(www.downloadProgress.ToString());
                byte[] bytes = www.downloadHandler.data;
                ImageManager.instance.SaveImage(elemento.nome + elemento.idElemento, bytes);

            }

        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message : " + e.Message);

        }
    }
    async Task FetchCartaImage(string url, CartaAPI carta)
    {
        try
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(carta.imagemCarta);
            await www.SendWebRequest();
            if (www.isHttpError || www.isHttpError)
            {
                Main.Instance.ShowError("Ocorreu um erro durante o download das cartas.CODE-01.2-");
            }
            else
            {
                Debug.Log("CARTA: " + carta.titulo + "Foto: " + carta.imagemCarta);
                //while (!www.isDone)
                //    progress(www.downloadProgress.ToString());
                byte[] bytes = www.downloadHandler.data;
                ImageManager.instance.SaveImage(carta.titulo + carta.idCarta, bytes);

            }
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Message : " + e.Message);

        }


    }
    #endregion

    //Metodos para criação do deck
    #region Criacao de Deck
    IEnumerator CreateDeck()
    {
        var cartasApi = _banco.ConsultarCartas().ToList();
        cartasApi.ForEach(x =>
        {
            ColecaoCartas c = new ColecaoCartas();
            Debug.Log("carta api: " + x.idCarta.ToString());
            c.Id = x.idCarta;
            c.Nome = x.titulo;
            c.TipoCarta = TipoCarta.MAGICA;
            c.Descricao = x.descricao;
            _banco.InserirCartaColecao(c);
        });
        yield return null;
    }
    #endregion  
}