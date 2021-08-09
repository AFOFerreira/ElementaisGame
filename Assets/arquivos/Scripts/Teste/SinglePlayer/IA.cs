using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using funcoesUteis;
using System.Threading.Tasks;


public class IA : MonoBehaviour
{
    #region Set Variables
    int qtdTurnos = 0;
    int jogadasMonstro = 1;
    GerenciadorJogo gerenciadorJogo;
    public float tempoAtual = 0;
    public static IA instance;
    int jogadas = 0;
    int idUltimoCristal;

    [Header("Cristais")]
    //0-agua, 1-fogo, 2-terra, 3- ar
    public int[] cristais = new int[4];

    [Header("Campos")]
    public List<SlotCampo> camposParaJogarMonstro = new List<SlotCampo>();
    public List<SlotCampo> camposAtacantes = new List<SlotCampo>();
    public List<SlotCampo> camposParaAtacar = new List<SlotCampo>();
    public List<SlotCampo> camposParaJogarMagicas = new List<SlotCampo>();
    public List<SlotCampo> camposOcupadosMagicas = new List<SlotCampo>();
    public List<SlotCampo> camposOcupadosMonstros = new List<SlotCampo>();
    public SlotCampo campoSelecionado;
    public SlotCampo ultimoCartaJogada;

    [Header("Deck")]
    public List<CartaGeral> deckTotal = new List<CartaGeral>();
    public List<CartaGeral> cemiterio = new List<CartaGeral>();
    public List<CartaGeral> mao = new List<CartaGeral>();

    [Header("Controladores")]
    public bool possoJogar, gCristal, defendendo;
    public TipoJogador tipoIA;
    public TipoFase fase;
    public float tempoPensamento = 0;
    int rng, tempRng;
    // Start is called before the first frame update
    #endregion

    #region UNITY DEFAULTS
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        SetDefaults();
        gCristal = false;
        gerenciadorJogo = GerenciadorJogo.instance;
        Timing.RunCoroutine(iniciarObjetos());

        VerificaCampos();
        VerificaMeuTurno();
        var c = deckTotal.Where(x => x.tipoCarta == TipoCarta.Auxiliar).FirstOrDefault();
        if (c != null)
        {
            passarParaMaoEspecifica(c);
        }
        for (int i = 0; i < 5; i++)
        {
            int id = Random.Range(0, deckTotal.Count - 1);
            passarParaMao(id);
        }

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("IA=>" + gerenciadorJogo.ExecutandoAnimacao);
        if (gerenciadorJogo.EmJogo)
        {
            //VerificaMeuTurno();
            if (gerenciadorJogo.emBatalha && possoJogar)
            {
                var t = 5;
                defendendo = gerenciadorJogo.turno == TipoJogador.PLAYER;
                var selecionando = gerenciadorJogo.selecionando == TipoJogador.IA;
                if (defendendo && selecionando)
                {
                    if (Delay(t))
                    {
                        Debug.Log("Nao tenho como defender ainda.");
                        gerenciadorJogo.PassaFaseBatalha();
                    }
                }
                else if (!defendendo && selecionando)
                {
                    Debug.Log("Estou Selecionando meus elementais para atacar.");

                    if (Delay(t))
                    {
                        Debug.Log("Pronto.");
                        gerenciadorJogo.PassaFaseBatalha();
                    }
                }
                else
                {
                    Debug.Log("Estou esperando a acao do meu oponente.");
                }

            }
            else if (possoJogar && !gerenciadorJogo.emBatalha)
            {
                VerificaCampos();
                fase = gerenciadorJogo.faseAtual;
                if (fase == TipoFase.MONSTRO)
                {
                    PensaJogadaMonstro();
                }
                else if (fase == TipoFase.MAGICA)
                {
                    Debug.Log("Fase Magica");
                    PensaJogadaMagica();
                }
                qtdTurnos++;
            }
            else
            {
                Debug.Log(tipoIA + ": Não posso jogar, não é a minha vez!!");
                campoSelecionado = null;
                jogadasMonstro = 1;

                //DO NOTHING!
            }
        }
    }
    #endregion

    #region Deck
    public void passarParaMao(int id)
    {
        if (mao.Count < 7)
        {
            CartaGeral carta = deckTotal[id];
            deckTotal.RemoveAt(id);
            mao.Add(carta);
        }
    }
    public void passarParaMaoEspecifica(CartaGeral c)
    {
        if (mao.Count < 7)
        {

            deckTotal.Remove(c);
            mao.Add(c);
        }
    }
    public void sortearDeck()
    {
        if (deckTotal.Count > 0)
        {
            int id = Random.Range(0, deckTotal.Count - 1);
            passarParaMao(id);
            gCristal = false;
        }
    }

    IEnumerator<float> iniciarObjetos()
    {


        ElementoGeral fogo = new ElementoGeral(2, "Fogo", Resources.Load<Sprite>("imagens/Elementos/icon_fogo"), Resources.Load<Sprite>("imagens/Elementos/modelo_fogo"), Resources.Load<Sprite>("imagens/Elementos/bandeiraFogo"), Resources.LoadAll<Sprite>("imagens/Elementos/fogoAtivar").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo1Sprite").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo2Sprite").ToList());
        ElementoGeral agua = new ElementoGeral(0, "Água", Resources.Load<Sprite>("imagens/Elementos/icon_agua"), Resources.Load<Sprite>("imagens/Elementos/modelo_agua"), Resources.Load<Sprite>("imagens/Elementos/bandeiraAgua"), Resources.LoadAll<Sprite>("imagens/Elementos/aguaAtivar").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua1Sprite").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua2Sprite").ToList());
        ElementoGeral terra = new ElementoGeral(1, "Terra", Resources.Load<Sprite>("imagens/Elementos/icon_terra"), Resources.Load<Sprite>("imagens/Elementos/modelo_terra"), Resources.Load<Sprite>("imagens/Elementos/bandeiraTerra"), Resources.LoadAll<Sprite>("imagens/Elementos/terraAtivar").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra1Sprite").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra2Sprite").ToList());
        ElementoGeral ar = new ElementoGeral(3, "Ar", Resources.Load<Sprite>("imagens/Elementos/icon_ar"), Resources.Load<Sprite>("imagens/Elementos/modelo_ar"), Resources.Load<Sprite>("imagens/Elementos/bandeiraAr"), Resources.LoadAll<Sprite>("imagens/Elementos/arAtivar").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar1Sprite").ToList(), Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar2Sprite").ToList());


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                01, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/01/01_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Pandion",//Titulo da carta
                "Este é o Pandion de fogo uma águia da espécie Imperial Oriental de estágio 1 da classe Guerreiro, seus ataques são rápidos e ferozes.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/01/01_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/01/01_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                3, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                02, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/02/02_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Pandion",//Titulo da carta
                "Este ó o Pandion de ar uma águia da espécie Harpia de estágio 1 da raça Curandeiro. Suas habilidades são seus chocotes dourados e claro sua agilidade. ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/02/02_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/02/02_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                2, //Ataque
                3, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                ar //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                03, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/03/03_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Dubhan",//Titulo da carta
                "Este Elemental é da espécie de Jabutis é muito comum ser encontrado nas matas brasileiras do Nordeste ou Sudeste   ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/03/03_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/03/03_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                3, //Ataque
                8, //Defesa
                1, //Cristais
                2, //Evolução(nivel)
                terra //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                04, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/04/04_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Dubhan",//Titulo da carta
                "Este Dubhan de fogo é da familía dos Jabutis, este elemental é um mini vulcão embulante, seus tentáculos são capases de prender um predador ou cuspir um frte jatos de uma eséci de gás inflamável.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/04/04_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/04/04_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                12, //Ataque
                6, //Defesa
                2, //Cristais
                3, //Evolução(nivel)
                fogo //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                05, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/05/05_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Dubhan",//Titulo da carta
                "Este Dubhan é o segundo estágio de evolução da espécie de Jabutis de água, com ataque 55 e defesa 37",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/05/05_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/05/05_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                6, //Ataque
                12, //Defesa
                4, //Cristais
                3, //Evolução(nivel)
                agua //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                06, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/06/06_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Taireth",//Titulo da carta
                "Taireth é uma mutação dos tubaões brancos, ele vive em zonas tropicais de águas quentes, seu tamanho pode chegar aos 5 metros e a pesar 200Kg. ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/06/06_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/06/06_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                3, //Ataque
                4, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                agua //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                07, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/07/07_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Taireth",//Titulo da carta
                "Taireth de terra é da família do Tubarão-tigre, vive em águas tropicais, em suas costas este Elemental carrega uma espécie de coral hipnótico paralisante, que serve para facilitar a caça.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/07/07_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/07/07_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                1, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                08, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/08/08_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Caedin",//Titulo da carta
                "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiu controlar muito bem a terra e pedras ao seu redor",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/08/08_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/08/08_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                1, //Ataque
                5, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra //Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                09, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/09/09_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Caedin",//Titulo da carta
                "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiru controlar muito bem a terra e pedras ao seu redor.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/09/09_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/09/09_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                4, //Ataque
                1, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                10, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/10/10_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Caedin",//Titulo da carta
                "Caedin de fogo é o segundo estágio da evolução de um cachorro do elemento fogo. Agora, Caedin tem uma calda a mais, seu corpo tem uma inversão de cores e seus olho grande quantia de calor. ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/10/10_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/10/10_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                8, //Ataque
                4, //Defesa
                2, //Cristais
                2, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                11, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/11/11_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Gork",//Titulo da carta
                "Gorki tem habilidades para controlar as plantas terrestres, ele consegue fazer as flores soltarem venenos, as raízes saírem do chão e fazer com que as gramas cresçam muito alto e rapidamente.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/11/11_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/11/11_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                2, //Ataque
                5, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                12, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/12/12_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Gork",//Titulo da carta
                "Este Primata do elemento fogo em estágio 1 da raça Guerreiro com ataque 29 e defesa 20, possui poderes vulcânicos ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/12/12_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/12/12_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                4, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                13, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/13/13_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Gaeron",//Titulo da carta
                "Gaeron de fogo é está em seu primeiro estágio evolutivo, sua maio habilidade são os disparos de pequenas bolas de chamas. Em sua cabeça e colunas é possível ver alguns ossos saltando de seu corpo. ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/13/13_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/13/13_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                2, //Ataque
                1, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                14, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/14/14_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Gaeron",//Titulo da carta
                "Gaeron	Tigure	Felino	Fogo	2	Guerreiro	2	6	3		14	Este Elemental felino em estágio de evolução 2 possui suas habilidades e amaduras melhoradas e fortificadas ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/14/14_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/14/14_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                6, //Ataque
                3, //Defesa
                2, //Cristais
                2, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                15, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/15/15_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Gaeron",//Titulo da carta
                "Evolução de estágio 2 do felino de terra da classe Tanque e possui uma forte defesa decorrente de sua armadura evoluida",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/15/15_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/15/15_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                5, //Ataque
                8, //Defesa
                2, //Cristais
                2, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                16, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/16/16_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "keigar",//Titulo da carta
                "Metade do seu corpo é coberto por uma espessa casca de pedra o que faz dele um predador blindado e, ao mesmo tempo agressivo por natureza ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/16/16_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/16/16_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                2, //Ataque
                5, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                17, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/17/17_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Farmos",//Titulo da carta
                "Farmos de terra está em seu segundo estágio evolutivo, agora ele tem o triplo do tamanho, o bulbo em suas costas se abriu e virou uma bela planta que expele um forte veneno estonteante.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/17/17_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/17/17_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                5, //Ataque
                7, //Defesa
                2, //Cristais
                2, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                18, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/18/18_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Farmos",//Titulo da carta
                "Elemental de estágio 2 da classe Guerreiro e possui a habilidade de controlar o fogo mas não possui muita defesa",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/18/18_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/18/18_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                6, //Ataque
                4, //Defesa
                1, //Cristais
                2, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                19, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/19/19_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Jargonus",//Titulo da carta
                "Este é um Elemental do tipo água sua evolução é proveniente do jacaré. Na suas costas, ele conta com espinhos reluzentes e muito venenosos ",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/19/19_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/19/19_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                5, //Ataque
                4, //Defesa
                2, //Cristais
                2, //Evolução(nivel)
                agua//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                20, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/20/20_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Urlain",//Titulo da carta
                "Urlain de terra é Elemental em estágio 1 da classe dos Urcatos, sua defesa é estremamente forte, porem seu ataque nem tanto. Sua mior habilidade é dontrolar as raizes e galhos de arvores ao seu redor.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/20/20_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/20/20_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                1, //Ataque
                4, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                21, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/21/21_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Urlain",//Titulo da carta
                "Este Elemetal é da família Urcatos mas do tipo elemento fogo",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/21/21_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/21/21_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                5, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                22, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/22/22_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Jain",//Titulo da carta
                "Jay é um pássaro da família dos corvos e pode pareces dócil, mas sua personalidade é agressiva e pode facilmente se tornar um carnívoro quando necessário.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/22/22_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/22/22_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                1, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                ar//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                23, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/23/23_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Jain",//Titulo da carta
                "Jay de fogo também é um pássaro da família dos corvos, sua personalidade é ainda mais agressiva e pode facilmente se tornar um carnívoro quando se sente ameaçado.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/23/23_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/23/23_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                4, //Ataque
                1, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                24, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/24/24_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Conin",//Titulo da carta
                "Este Elemental do tipo terra é da família dos roedores e pode ser muito ágil",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/24/24_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/24/24_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                1, //Ataque
                5, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                terra//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                25, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/25/25_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Conin",//Titulo da carta
                "Elemental do tipo fogo e da família dos roedores, possui habilidades de rapidez e controle de fogo",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/25/25_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/25/25_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                4, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                fogo//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        deckTotal.Add(new CartaGeral(
                TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
                26, // id da carta
                Resources.Load<Sprite>("imagens/Elementais/26/26_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
                "Ardus",//Titulo da carta
                "Ardus é uma vespa do elemento ar, sua maior habilidade é de atacar coordenado em conjunto. Ela também emite um zumbido muito alto que pode atordoar os oponentes.",//Descricao
                Resources.LoadAll<Sprite>("imagens/Elementais/26/26_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
                Resources.Load<Sprite>("imagens/Elementais/26/26_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
                3, //Ataque
                2, //Defesa
                1, //Cristais
                1, //Evolução(nivel)
                ar//Elemento (fogo, agua, terra, ar)
         ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //deckTotal.Add(new CartaGeral(
        //        TipoCarta.Elemental, //tipo de carta (Elemental/Auxiliar/Armadilha)
        //        27, // id da carta
        //        Resources.Load<Sprite>("imagens/Elementais/27/27_card"),// foto da carta (manter os nomes só mudar pra ficar igual o ID)
        //        "Nagini",//Titulo da carta
        //        "Nagini de terra é da espécie de serpentes atheris hispida, sua picada pode ser mortal e sua defesa física em forma de armadura de madeira é absurdamente forte. As escamas de madeira pontudas e os olhos verdes ipnotizantes ajudam a serpente a ficar assustadora.",//Descricao
        //        Resources.LoadAll<Sprite>("imagens/Elementais/27/27_sprites").ToList(),//Animação do campo (manter os nomes só mudar pra ficar igual o ID)
        //        Resources.Load<Sprite>("imagens/Elementais/27/27_arco"),//Foto da UI Topo (manter os nomes só mudar pra ficar igual o ID)
        //        2, //Ataque
        //        4, //Defesa
        //        1, //Cristais
        //        1, //Evolução(nivel)
        //        terra//Elemento (fogo, agua, terra, ar)
        // ));
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        AuxArmGeral armadilha = new AuxArmGeral(Resources.Load<Sprite>("imagens/Armadilhas/molduraArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/iconArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/maskArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/armMolduraCampo"));
        AuxArmGeral auxiliar = new AuxArmGeral(Resources.Load<Sprite>("imagens/Auxiliares/molduraAuxiliar"), Resources.Load<Sprite>("imagens/Auxiliares/iconAuxiliar"), Resources.Load<Sprite>("imagens/auxiliares/maskAuxiliar"), Resources.Load<Sprite>("imagens/auxiliares/auxMolduraCampo"));

        //deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 15, Resources.Load<Sprite>("imagens/Armadilhas/bloqueio/bloqueioCarta"), "Bloqueio", "Bloqueia o efeito e destrói a carta Auxiliar do oponente assim que for ativada.", Resources.LoadAll<Sprite>("imagens/Armadilhas/bloqueio/bloqueioSprites").ToList(), armadilha, "", null));
        //deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 16, Resources.Load<Sprite>("imagens/Armadilhas/diaDeSorte/diaDeSorteCarta"), "Dia de Sorte", "Se seu Elemental for destruído neste turno, aumente em +20 de ATAQUE um Elemental aleatório do seu campo.", Resources.LoadAll<Sprite>("imagens/Armadilhas/diaDeSorte/diaDeSorteSprites").ToList(), armadilha, "", null));
        //deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 17, Resources.Load<Sprite>("imagens/Armadilhas/duploSentido/duploSentidoCarta"), "Duplo Sentido", "O ATAQUE equivalente do Elemental atacante também atinge os pontos de vida do oponente.", Resources.LoadAll<Sprite>("imagens/Armadilhas/duploSentido/duploSentidoSprites").ToList(), armadilha, "", null));
        //deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 18, Resources.Load<Sprite>("imagens/Armadilhas/escudoDeVida/escudoDeVidaCarta"), "Escudo de Vida", "Absorve o ataque do elemental e converte em pontos de vida para o jogador.", Resources.LoadAll<Sprite>("imagens/Armadilhas/escudoDeVida/escudoDeVidaSprites").ToList(), armadilha, "", null));
        //deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 19, Resources.Load<Sprite>("imagens/Armadilhas/espelho/espelhoCarta"), "Espelho", "Cria um espelho no campo devolvendo o ataque para o Elemental que esta atacando.", Resources.LoadAll<Sprite>("imagens/Armadilhas/espelho/espelhoSprites").ToList(), armadilha, "", null));

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////AUXILIARES/////INICIO/////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoBarrarAgua = new List<Acoes>();

        acaoBarrarAgua.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa, // (ataque/defesa)
                        -4, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { agua },
                        false
                    })
         );


        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                26, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/barrarAgua/barrarAguaCarta"),//Foto carta
                "Barrar[agua]", // Titulo
                "Cause -4 de dano direto em todos os Elementais do tipo Água no campo", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarAgua/barrarAguaSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Dropar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoBarrarAgua // lista de ações
                )

            );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoBarrarFogo = new List<Acoes>();

        acaoBarrarFogo.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        false, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa, // (ataque/defesa)
                        -4, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { fogo },
                        false
                    })
         );


        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                27, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/barrarFogo/barrarFogoCarta"),//Foto carta
                "Barrar[fogo]", // Titulo
                "Cause -4 de dano direto em todos os Elementais do tipo fogo no campo", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarFogo/barrarFogoSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Dropar, //(Dropar/Selecionar)
                 1,
                TipoEfeito.Temporario,
                acaoBarrarFogo // lista de ações
                )

            );

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoBarrarTerra = new List<Acoes>();

        acaoBarrarTerra.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        false, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa, // (ataque/defesa)
                        -4, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { terra },
                        false
                    })
         );


        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                28, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/barrarTerra/barrarTerraCarta"),//Foto carta
                "Barrar[terra]", // Titulo
                "Cause -4 de dano direto em todos os Elementais do tipo terra no campo", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarTerra/barrarTerraSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Dropar, //(Dropar/Selecionar)
                 1,
                TipoEfeito.Temporario,
                acaoBarrarTerra // lista de ações
                )

            );

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoBarrarAr = new List<Acoes>();

        acaoBarrarAr.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        false, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa, // (ataque/defesa)
                        -4, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { ar },
                        false
                    })
         );


        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                28, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/barrarAr/barrarArCarta"),//Foto carta
                "Barrar[ar]", // Titulo
                "Cause -4 de dano direto em todos os Elementais do tipo ar no campo", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarAr/barrarArSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Dropar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoBarrarAr // lista de ações
                )

            );

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        List<Acoes> acaoPoseidon = new List<Acoes>();

        acaoPoseidon.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque, // (ataque/defesa)
                        10, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { agua },
                        false
                    })
         );

        acaoPoseidon.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa,// (ataque/defesa)
                        5,// valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { agua },
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                17, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/pocaoDePoseidon/pocaoDePoseidonCarta"),//Foto carta
                "Poçao de Poseidon", // Titulo
                "Concede +10  de ATAQUE e +5 de DEFESA a um Elemental de ÁGUA", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDePoseidon/bpocaoDePoseidonSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoPoseidon // lista de ações
                )
            );


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        List<Acoes> acaoGaia = new List<Acoes>();

        acaoGaia.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque, // (ataque/defesa)
                        10, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { terra },
                        false
                    })
         );

        acaoGaia.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa,// (ataque/defesa)
                        5,// valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { terra },
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                18, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeGaia/pocaoDeGaiaCarta"),//Foto carta
                "Poçao de Gaia", // Titulo
                "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de TERRA", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeGaia/pocaoDeGaiaSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoGaia // lista de ações
                )

            );

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoZeus = new List<Acoes>();

        acaoZeus.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque, // (ataque/defesa)
                        10, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { ar },
                        false
                    })
         );

        acaoZeus.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa,// (ataque/defesa)
                        5,// valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { ar },
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                19, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeZeus/pocaoDeZeusCarta"),//Foto carta
                "Poçao de Zeus", // Titulo
                "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de AR", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeZeus/pocaoDeZeusSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoZeus // lista de ações
                )

            );


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoHefesto = new List<Acoes>();

        acaoHefesto.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque, // (ataque/defesa)
                        10, // valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { fogo },
                        false
                    })
         );

        acaoHefesto.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa,// (ataque/defesa)
                        5,// valor para adicionar/remover no ataque ou defesa
                        new List<ElementoGeral>() { fogo },
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                20, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeHefesto/pocaoDeHefestoCarta"),//Foto carta
                "Poçao de Hefesto", // Titulo
                "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de FOGO", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeHefesto/pocaoDeHefestoSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoHefesto // lista de ações
                )

            );

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoEmGloria = new List<Acoes>();

        acaoEmGloria.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesa, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque, // (ataque/defesa)
                        2,
                        false
                    })
         );

        acaoEmGloria.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.defesa,// (ataque/defesa)
                        2,
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                20, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/emGloria/emGloriaCarta"),//Foto carta
                "Em Glória", // Titulo
                "Concede +2 de ATAQUE e DEFESA para um Elemental no campo em cada turno até o fim da partida ou até que destruam esta carta", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/emGloria/emGloriaSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                99,
                TipoEfeito.Continuo,
                acaoEmGloria // lista de ações
                )

            );


        //////////////////////////////////////////////////////////////////////////////////////
        //List<Acoes> acaoCaracasana = new List<Acoes>();
        //acaoCaracasana.Add(new Acoes(
        //    AcoesDisponiveis.alterarVidaPlayer,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                false, // Jogador 1 (true) ou Jogador 2(false)
        //                -6, // qtdAltedada
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        21, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/caracasana/caracasanaCarta"),//Foto carta
        //        "Caracasana", // Titulo
        //        "Causa -6 de dano direto na vida do oponente, durante dois turnos seguidos.", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/caracasana/caracasanaSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoMeio, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //        2,
        //        TipoEfeito.Temporario,
        //        acaoCaracasana // lista de ações
        //        )
        //    );


        //////////////////////////////////////////////////////////////////////////////////////
        ///
        //////////////////////////////////////////////////////////////////////////////////////
        //List < Acoes > acaoOleandro = new List<Acoes>();
        //acaoOleandro.Add(new Acoes(
        //    AcoesDisponiveis.alterarVidaPlayer,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //               false, // Jogador 1 (true) ou Jogador 2(false)
        //                -10,// qtdAltedada
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        21, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/oleandro/oleandroCarta"),//Foto carta
        //        "Oleandro", // Titulo
        //        "Causa -10 de dano direto na vida do guardião opodente.", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/oleandro/oleandroSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoMeio, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoOleandro // lista de ações
        //        )
        //    );


        //////////////////////////////////////////////////////////////////////////////////////
        ///

        //List < Acoes > acaoReforco = new List<Acoes>();
        //acaoReforco.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                5,
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        20, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/reforco/reforcoCarta"),//Foto carta
        //        "Reforço", // Titulo
        //        "Aumenta em +5 sua defesa durante 3 turnos", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/reforco/reforcoSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //        3,
        //        TipoEfeito.Temporario,
        //        acaoReforco // lista de ações
        //        )

        //    );


        //////////////////////////////////////////////////////////////////////////////////////
        List<Acoes> acaoVigor = new List<Acoes>();
        acaoVigor.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque,// (ataque/defesa)
                        5,
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                20, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/vigor/vigorCarta"),//Foto carta
                "Vigor", // Titulo
                "Escolha um Elemental para receber +5 de ataque", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/vigor/vigorSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoVigor // lista de ações
                )

            );


        //////////////////////////////////////////////////////////////////////////////////////


        List<Acoes> acaoFuria = new List<Acoes>();
        acaoFuria.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        true, // Jogador 1 (true) ou Jogador 2(false)
                        ParametroAlvo.ataque,// (ataque/defesa)
                        4,
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                20, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/furia/furiaCarta"),//Foto carta
                "Fúria", // Titulo
                "Escolha um Elemental para receber +5 de ataque", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/furia/furiaSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Selecionar, //(Dropar/Selecionar)
                2,
                TipoEfeito.Temporario,
                acaoFuria // lista de ações
                )

            );


        ////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////AUXILIARES/////FIM////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        yield return Timing.WaitForOneFrame;
    }

    #endregion

    #region Turno
    void VerificaMeuTurno()
    {
        if (tipoIA == gerenciadorJogo.turno)
        {
            possoJogar = true;
        }
        else
            possoJogar = false;
    }

    #endregion

    #region Padrao
    void VerificaCampos()
    {
        camposParaJogarMagicas = gerenciadorJogo.VerificaCampoDisponivelMagicas(tipoIA);
        camposOcupadosMagicas = gerenciadorJogo.VerificaCampoOcupadoMagicas(tipoIA);
        camposParaJogarMonstro = gerenciadorJogo.VerificaCampoDisponivelMonstros(tipoIA);
        camposOcupadosMonstros = gerenciadorJogo.VerificaCampoOcupadoMonstros(tipoIA);
    }
    bool Delay(float t)
    {
        bool b = false;
        tempoAtual += 1f * Time.deltaTime;
        if (tempoAtual >= t)
        {
            b = true;
            tempoAtual = 0f;
        }
        return b;

    }
    public void SetDefaults()
    {
        jogadas = 1;
        rng = 0;
        tempRng = 0;
        tempoAtual = 0f;
        possoJogar = true;
        sortearDeck();
        GanharCristais();
    }
    public void GanharCristais()
    {

        var cristal = Random.Range(0, cristais.Length);
        var cristalQtd = 1;

        cristais[cristal] += cristalQtd;
        gCristal = false;
    }
    bool JogarCartaNoCampo(SlotCampo campo)
    {
        bool b = false;
        campoSelecionado = campo;
        Debug.Log("Pensando...");
        CartaGeral carta = PensaCartaMonstro();
        if (carta != null)
        {
            gerenciadorJogo.JogarCarta(carta, campoSelecionado.idCampo);
            mao.Remove(carta);
            jogadasMonstro--;
            carta = null;
            ultimoCartaJogada = campoSelecionado;
            b = true;
        }
        else
        {
            b = false;
        }

        return b;
    }
    bool JogarCartaMagicaNoCampo(SlotCampo campo)
    {
        bool b = false;
        campoSelecionado = campo;
        Debug.Log("Pensando...");
        CartaGeral carta = PensaCartaAuxiliar();
        if (carta != null)
        {
            gerenciadorJogo.dropAuxArm(carta, campoSelecionado.idCampo, tipoIA);
            mao.Remove(carta);
            carta = null;
            Debug.Log("Joguei uma carta auxiliar");
            ultimoCartaJogada = campoSelecionado;
            b = true;
        }


        return b;
    }
    IEnumerator AtivarELemental(SlotCampo elementalAtacante)
    {

        yield return new WaitForSeconds(2f);
        switch (elementalAtacante.cartaGeral.elemento.nomeElemento)
        {
            case "Água":
                if (cristais[0] >= elementalAtacante.cartaGeral.cristais)
                {
                    cristais[0] -= elementalAtacante.cartaGeral.cristais;
                    gerenciadorJogo.ativarElementalAI(elementalAtacante.idCampo);
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
                else
                {
                    Debug.Log("Nao tenho cristais suficientes");
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
            case "Fogo":
                if (cristais[1] >= elementalAtacante.cartaGeral.cristais)
                {
                    cristais[1] -= elementalAtacante.cartaGeral.cristais;
                    gerenciadorJogo.ativarElementalAI(elementalAtacante.idCampo);
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
                else
                {
                    Debug.Log("Nao tenho cristais suficientes");
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }

            case "Terra":
                if (cristais[2] >= elementalAtacante.cartaGeral.cristais)
                {
                    cristais[2] -= elementalAtacante.cartaGeral.cristais;
                    gerenciadorJogo.ativarElementalAI(elementalAtacante.idCampo);
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
                else
                {
                    Debug.Log("Nao tenho cristais suficientes");
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
            case "Ar":
                if (cristais[3] >= elementalAtacante.cartaGeral.cristais)
                {
                    cristais[3] -= elementalAtacante.cartaGeral.cristais;
                    gerenciadorJogo.ativarElementalAI(elementalAtacante.idCampo);
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
                else
                {
                    Debug.Log("Nao tenho cristais suficientes");
                    gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    break;
                }
        }
    }
    #endregion

    #region Estrategia

    #region Elementais
    void PensaJogadaMonstro()
    {
        var camposDisponiveis = camposParaJogarMonstro.Where(x => (!x.ocupado && x.tipoSlot == TipoCarta.Elemental)).ToList();
        var camposAtivados = camposOcupadosMonstros.Where(x => (x.ativado)).ToList();
        camposAtacantes = camposAtivados;
        var camposParaAtivar = camposOcupadosMonstros.Where(x => (!x.ativado)).ToList();
        var camposAlvos = gerenciadorJogo.slotsCampoP1.Where(x => (x.ocupado)).ToList();
        camposParaAtacar = camposAlvos;
        SlotCampo campoParaJogar = CampoEcolhidoJogar(camposDisponiveis);
        SlotCampo campoParaAtivar = CampoEcolhidoAtivar(camposParaAtivar);
        var t = Random.Range(2, tempoPensamento);
        if (rng <= 0)
        {
            rng = Rng();
        }
        Debug.Log(rng);

        if (Delay(t))
        {
            if (rng == 1 && campoParaJogar != null)
            {
                Debug.Log("A");
                if (tempRng == 0)
                    tempRng = Random.Range(1, 4);
                Debug.Log(tempRng);
                if (!campoParaJogar.ocupado)
                {
                    if (jogadasMonstro == 1)
                    {
                        if (JogarCartaNoCampo(campoParaJogar))
                        {
                            StartCoroutine(AtivarELemental(campoParaJogar));
                        }

                    }
                    else
                    {
                        StartCoroutine(AtivarELemental(campoParaJogar));
                    }
                }
            }
            else if (rng == 2 && campoParaAtivar != null)
            {
                Debug.Log("B");
                if (campoParaAtivar != null)
                {
                    StartCoroutine(AtivarELemental(campoParaAtivar));
                }
                else
                {
                    return;
                }
            }
            else if (rng == 3 && camposAtivados.Count > 0)
            {
                Debug.Log("C");
                if (CombinacaoAtaque(camposAlvos, camposAtivados))
                {
                    gerenciadorJogo.setTurnoBatalha();
                }
                else
                {
                    return;
                }
            }


        }
    }
    CartaGeral PensaCartaMonstro()
    {
        var carta = new CartaGeral();
        var tempMao = mao.Where(x => x.tipoCarta == TipoCarta.Elemental).ToList();
        if (tempMao.Count > 0)
        {
            foreach (var obj in tempMao)
            {
                switch (obj.elemento.nomeElemento)
                {
                    case "Água":
                        if (cristais[0] >= obj.cristais)
                        {
                            carta = obj;
                            break;

                        }
                        else
                        {
                            Debug.Log("Nao tenho cristais suficientes, pegando carta aleatoria");
                            carta = tempMao[Random.Range(0, tempMao.Count)];

                        }
                        break;

                    case "Fogo":
                        if (cristais[1] >= obj.cristais)
                        {
                            carta = obj;
                            break;
                        }
                        else
                        {
                            Debug.Log("Nao tenho cristais suficientes, pegando carta aleatoria");
                            carta = tempMao[Random.Range(0, tempMao.Count)];

                        }
                        break;

                    case "Terra":
                        if (cristais[2] >= obj.cristais)
                        {
                            carta = obj;
                            break;
                        }
                        else
                        {
                            Debug.Log("Nao tenho cristais suficientes, pegando carta aleatoria");
                            carta = tempMao[Random.Range(0, tempMao.Count)];
                        }
                        break;
                    case "Ar":
                        if (cristais[3] >= obj.cristais)
                        {
                            carta = obj;
                            break;
                        }
                        else
                        {
                            Debug.Log("Nao tenho cristais suficientes, pegando carta aleatoria");
                            carta = tempMao[Random.Range(0, tempMao.Count)];
                        }
                        break;
                }


            }
        }
        return carta;
    }
    #endregion

    #region Magicas
    CartaGeral PensaCartaAuxiliar()
    {
        var carta = new CartaGeral();
        var tempMao = mao.Where(x => x.tipoCarta == TipoCarta.Auxiliar).ToList();
        if (tempMao.Count > 0)
        {
            foreach (var obj in tempMao)
            {
                carta = obj;
            }
        }
        else
        {
            carta = null;
        }
        return carta;
    }
    void PensaJogadaMagica()
    {
        var camposDisponiveis = camposParaJogarMagicas.Where(x => (!x.ocupado && x.tipoSlot == TipoCarta.AuxArm)).ToList();
        var camposAtivados = camposOcupadosMagicas.Where(x => (x.ativado)).ToList();
        camposAtacantes = camposAtivados;
        var camposParaAtivar = camposOcupadosMagicas.Where(x => (!x.ativado)).ToList();
        //var camposAlvos = gerenciadorJogo.slotsCampoP1.Where(x => (x.ocupado)).ToList();
        //camposParaAtacar = camposAlvos;
        SlotCampo campoParaJogar = CampoEcolhidoJogarMagicas(camposDisponiveis);
        SlotCampo campoParaAtivar = CampoEcolhidoAtivarMagicas(camposParaAtivar);
        var t = Random.Range(2, tempoPensamento);

        if (campoParaJogar != null)
        {
            var v = JogarCartaMagicaNoCampo(campoParaJogar);

            if (Delay(3))
            {
                Debug.Log("IA=>" + gerenciadorJogo.ExecutandoAnimacao);
                if (gerenciadorJogo.ExecutandoAnimacao == false)
                {
                    Debug.Log("Esperando");

                    Debug.Log("OK");
                    gerenciadorJogo.PassaTurno();

                }
                //else
                //{
                //    Debug.Log("PASSOU AQUI");
                //    gerenciadorJogo.PassaTurno();
                //}

            }
        }
        else
        {
            gerenciadorJogo.PassaTurno();
        }
        //else if (rng == 2 && campoParaAtivar != null)
        //{
        //    Debug.Log("B");
        //    if (campoParaAtivar != null)
        //    {
        //        StartCoroutine(AtivarELemental(campoParaAtivar));
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}


    }

    #endregion

    #region Campos
    SlotCampo CampoEcolhidoAtivar(List<SlotCampo> camposParaAtivar)
    {
        return camposParaAtivar.Count > 0 ? camposParaAtivar[Random.Range(0, camposParaAtivar.Count)] : null;
    }
    SlotCampo CampoEcolhidoJogar(List<SlotCampo> camposDisponiveis)
    {

        return camposDisponiveis.Count > 0 ? camposDisponiveis[Random.Range(0, camposDisponiveis.Count)] : null;
    }
    SlotCampo CampoEcolhidoAtivarMagicas(List<SlotCampo> camposParaAtivar)
    {
        return camposParaAtivar.Count > 0 ? camposParaAtivar[Random.Range(0, camposParaAtivar.Count)] : null;
    }
    SlotCampo CampoEcolhidoJogarMagicas(List<SlotCampo> camposDisponiveis)
    {

        return camposDisponiveis.Count > 0 ? camposDisponiveis[Random.Range(0, camposDisponiveis.Count)] : null;
    }
    #endregion

    #region MelhoresCartas
    SlotCampo MelhorCartaEmCampo(List<SlotCampo> cartaAtivas)
    {
        if (cartaAtivas.Count > 0)
        {
            var itemMaxHeight = cartaAtivas.Max(y => y.cartaGeral.ataque);
            var itemsMax = cartaAtivas.Where(x => x.cartaGeral.ataque == itemMaxHeight).First();
            return itemsMax;
        }
        else
            return null;
    }
    SlotCampo MelhorCartaOponente(List<SlotCampo> cartaOponente)
    {
        var rng = Random.Range(0, 1f);

        if (cartaOponente.Count > 0)
        {
            if (rng < 0.75f)
            {
                var itemMaxHeight = cartaOponente.Min(y => y.cartaGeral.defesa);
                var itemsMax = cartaOponente.Where(x => x.cartaGeral.defesa == itemMaxHeight).First();
                return itemsMax;
            }
            else
            {
                return cartaOponente[Random.Range(0, cartaOponente.Count)];
            }
        }
        else
            return null;
    }
    #endregion

    #region RNG
    int Rng()
    {
        var totalCamposNaoAtivados = camposOcupadosMonstros.Where(x => !x.ativado).ToList().Count;
        var totalCamposAtivados = camposOcupadosMonstros.Where(x => x.ativado).ToList().Count;
        var totalCamposDisponiveis = camposParaJogarMonstro.Count;
        var rng = Random.Range(1, 60);
        if (rng <= 20)
        {
            Debug.Log("RNG:Caso-A");
            if (totalCamposDisponiveis > 0 && totalCamposDisponiveis > totalCamposNaoAtivados)
                return 1;
            else if (totalCamposNaoAtivados > 0 && totalCamposNaoAtivados > totalCamposAtivados)
                return 2;
            else
                return 3;
        }
        else if (rng <= 40)
        {
            Debug.Log("RNG:Caso-B");
            if (totalCamposNaoAtivados > 0 && totalCamposNaoAtivados > totalCamposAtivados)
                return 2;
            else if (totalCamposAtivados > 0 && totalCamposAtivados > totalCamposDisponiveis)
                return 3;
            else
                return 1;
        }
        else
        {
            Debug.Log("RNG:Caso-C");
            if (totalCamposAtivados > 0)
                return 3;
            else if (totalCamposDisponiveis > 0 && totalCamposDisponiveis > totalCamposNaoAtivados)
                return 1;
            else
                return 2;
        }


    }
    bool CombinacaoAtaque(List<SlotCampo> camposAlvos, List<SlotCampo> cartas)
    {
        Debug.Log(cartas.Count + " , " + camposAlvos.Count);
        if (camposAlvos.Count > 0)
        {
            for (int i = 0; i < cartas.Count; i++)
            {
                for (int j = 0; j < camposAlvos.Count; j++)
                {
                    if (!cartas[i].marcado && !camposAlvos[j].marcado)
                    {
                        gerenciadorJogo.executaAtaque(cartas[i].idCampo, camposAlvos[j].idCampo, false);
                        return false;
                    }
                }
            }
            Debug.Log("Saiu ataque ao elemental");
        }
        else
        {
            for (int i = 0; i < cartas.Count; i++)
            {
                Debug.Log("Posso atacar diretamente");
                if (!cartas[i].marcado)
                {
                    gerenciadorJogo.executaAtaque(cartas[i].idCampo, 3, false);
                    return false;
                }
            }
            Debug.Log("Saiu ataque direto");
        }

        return true;
    }
    #endregion

    #endregion
}
