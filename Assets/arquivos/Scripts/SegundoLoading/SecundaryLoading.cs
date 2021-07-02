using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SecundaryLoading : MonoBehaviour
{
    /// <summary>
    /// Script para carregamento segundario.
    /// </summary>
    
    //variavel para controle do slider e do texto de status
    Slider slider;
    TextMeshProUGUI status_loading;

    //Variavel de controle de operação assincrona
    AsyncOperation async;
    //variavel de controle para o próxima cena
    LoadingState l;

    //variavel de controle de progresso
    float progresso = 0.0f;

    private void Awake()
    {
        //define os valores iniciais para as variaveis e atribui os componentes 
        SetDefaultsValues();

    }
    //quando iniciado em cena
    private void Start()
    {
        //define os valores iniciais para as variaveis e atribui os componentes 
        SetDefaultsValues();
        //chama o metodo para carregar a proxima cena recebendo o parametro vindo do gerenciador
        LoadScreen(l);
    }


    //para a execução de todas as trilhas sonoras do jogo
    #region defaults functions
    void StopAllMusics()
    {
        AudioBase._instance.stopMusicaMenu();
        AudioBase._instance.stopMusicaDerrota();
        AudioBase._instance.stopMusicaGameplay();
        AudioBase._instance.stopMusicaLobby();
        AudioBase._instance.stopMusicaTutorial();
        AudioBase._instance.stopMusicaVitoria();
        AudioBase._instance.stopMusicaTelaBatalha();
    }

    //define os valores iniciais
    private void SetDefaultsValues()
    {
        //define o proximo nivel a ser carregado
        l = Main.Instance.ls;

        //para todas as musicas
        StopAllMusics();

        //referencia os componentes no slider e do status
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        status_loading = GameObject.Find("status").GetComponent<TextMeshProUGUI>();

        //define o progresso para zero, zerando a barra de slider
        progresso = 0;
    }
    #endregion

    #region Loading

    //metodo para carregar tela que recebe um estado do gerenciador
    public void LoadScreen(LoadingState state)
    {
        //verifica o estado e define o proximo nivel
        switch (state)
        {
            case LoadingState.MENU_INICIAL:
                StartCoroutine(LoadingScreen(3));
                break;
            case LoadingState.TUTORIAL1:
                StartCoroutine(LoadingScreen(4));
                break;
            case LoadingState.TUTORIAL2:
                StartCoroutine(LoadingScreen(5));
                break;
            case LoadingState.MULTIPLAYER:
                StartCoroutine(LoadingScreen(6));
                break;
            case LoadingState.TREINO:
                StartCoroutine(LoadingScreen(7));
                break;
        }

    }

    //metodo para carregar
    IEnumerator LoadingScreen(int LVL)
    {
        //recebe o status do SceneManager
        async = SceneManager.LoadSceneAsync(LVL);
        //define a ativação da próxima cena como falsa
        async.allowSceneActivation = false;
        //define o status para salvar os dados
        status_loading.text = "Salvando dados...";
        //***aqui irá salvar os dados da cena anterior***
        //StartCoroutine( Main.Instance.SetDeck());
        //enquanto nao esta pronta o carregamento da cena
        while (async.isDone == false)
        {
            //soma a variavel progresso com o progresso de carregamento informado pela variavel 'async'
            progresso += (float)async.progress;
            //passa o valor para o slider
            slider.value = progresso;
            //espera 5 segundos
            yield return new WaitForSeconds(5);
            //informa ao status que irá carregar a proxima cena
            status_loading.text = "Carregando cena...";
            //espera dois segundos
            yield return new WaitForSeconds(2);
            //ativa a cena
            async.allowSceneActivation = true;
        }
        //retorna nulo
        yield return null;
    }
    #endregion
}
