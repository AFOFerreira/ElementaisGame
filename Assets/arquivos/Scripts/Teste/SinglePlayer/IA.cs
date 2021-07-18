using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


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
    public List<SlotCampo> campoOcupadosMagicas = new List<SlotCampo>();
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
        Timing.RunCoroutine(iniciarObjetos());
        Timing.RunCoroutine(iniciarObjetos());
        VerificaCampos();
        VerificaMeuTurno();
        for (int i = 0; i < 5; i++)
        {
            int id = Random.Range(0, deckTotal.Count - 1);
            passarParaMao(id);
        }

    }
    // Update is called once per frame
    void Update()
    {
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
                    //if (camposParaJogarMagicas.Count > 0)
                    //{
                    //    for (int i = 0; i < camposParaJogarMagicas.Count; i++)
                    //    {
                    //        campoSelecionado = camposParaJogarMagicas[i];
                    //    }
                    //    Debug.Log(tipoIA + ": Posso jogar no campo: " + campoSelecionado.idCampo);
                    //    if (!campoSelecionado.ocupado)
                    //    {
                    //        Debug.Log("Esperando jogada!");
                    //        var c = Random.Range(0, mao.Count);
                    //        var cartaA = mao[c];
                    //        if (Input.GetButtonDown("Jump"))
                    //        {
                    //            //gerenciadorJogo.JogarCarta(campoSelecionado, cartaA.gameObject);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log(tipoIA + ": Não posso jogar, nao há campos disponiveis!");
                    //    gerenciadorJogo.PassaTurno();
                    //}
                    gerenciadorJogo.PassaTurno();
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


        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 0, Resources.Load<Sprite>("imagens/Elementais/01/01-card"), "Pandion", "Este é o Pandion de fogo uma águia da espécie Imperial Oriental de estágio 1 da classe Guerreiro, seus ataques são rápidos e ferozes.", Resources.LoadAll<Sprite>("imagens/Elementais/01/01-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/01/01_arco"), 28, 19, 1, 1, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 1, Resources.Load<Sprite>("imagens/Elementais/02/02-card"), "Pandion", "Este é o Pandion de ar uma águia da espécie Harpia de estágio 1 da raça Curandeiro. Suas habilidades são seus chicotes dourados, e claro sua agilidade.", Resources.LoadAll<Sprite>("imagens/Elementais/02/02-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/02/02_arco"), 19, 22, 1, 1, ar));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 2, Resources.Load<Sprite>("imagens/Elementais/03/03-card"), "Dubhan", "Este Elemental é da espécie de Jabutis é muito comum ser encontrado nas matas brasileiras do Nordeste ou Sudeste.", Resources.LoadAll<Sprite>("imagens/Elementais/03/03-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/03/03_arco"), 24, 56, 1, 2, terra));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 3, Resources.Load<Sprite>("imagens/Elementais/04/04-card"), "Dubhan", "Este Dubhan de fogo é da família dos Jabutis, este elemental é um mini vulcão ambulante, seus tentáculos são capazes de prender um predador ou cuspir um forte jato de uma espécie de gás inflamável.", Resources.LoadAll<Sprite>("imagens/Elementais/04/04-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/04/04_arco"), 59, 44, 1, 2, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 4, Resources.Load<Sprite>("imagens/Elementais/05/05-card"), "Dubhan", "Este Dubhan é o segundo estágio de evolução da espécie de Jabutis de água, com ataque 55 e defesa 37.", Resources.LoadAll<Sprite>("imagens/Elementais/05/05-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/05/05_arco"), 55, 37, 2, 1, agua));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 5, Resources.Load<Sprite>("imagens/Elementais/06/06-card"), "Taireth", "Taireth é uma mutação dos tubaões brancos, ele vive em zonas tropicais de águas quentes, seu tamanho pode chegar aos 5 metros e a pesar 200Kg.", Resources.LoadAll<Sprite>("imagens/Elementais/06/06-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/06/06_arco"), 25, 12, 1, 1, agua));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 6, Resources.Load<Sprite>("imagens/Elementais/07/07-card"), "Taireth", "Taireth de terra é da família do Tubarão-tigre, vive em águas tropicais, em suas costas este Elemental carrega uma espécie de coral hipnótico paralisante, que serve para facilitar a caça.", Resources.LoadAll<Sprite>("imagens/Elementais/07/07-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/07/07_arco"), 17, 30, 1, 1, agua));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 7, Resources.Load<Sprite>("imagens/Elementais/08/08-card"), "Caedin", "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiu controlar muito bem a terra e pedras ao seu redor.", Resources.LoadAll<Sprite>("imagens/Elementais/08/08-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/08/08_arco"), 21, 36, 1, 1, terra));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 8, Resources.Load<Sprite>("imagens/Elementais/09/09-card"), "Caedin", "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiru controlar muito bem a terra e pedras ao seu redor.", Resources.LoadAll<Sprite>("imagens/Elementais/09/09-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/09/09_arco"), 21, 5, 1, 1, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 9, Resources.Load<Sprite>("imagens/Elementais/10/10-card"), "Caedin", "Caedin de fogo é o segundo estágio da evolução de um cachorro do elemento fogo. Agora, Caedin tem uma calda a mais, seu corpo tem uma inversão de cores e seus olho grande quantia de calor.", Resources.LoadAll<Sprite>("imagens/Elementais/10/10-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/10/10_arco"), 49, 38, 1, 2, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 10, Resources.Load<Sprite>("imagens/Elementais/11/11-card"), "Gork", "Gorki tem habilidades para controlar as plantas terrestres, ele consegue fazer as flores soltarem venenos, as raízes saírem do chão e fazer com que as gramas cresçam muito alto e rapidamente.", Resources.LoadAll<Sprite>("imagens/Elementais/11/11-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/11/11_arco"), 19, 31, 1, 1, terra));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 11, Resources.Load<Sprite>("imagens/Elementais/12/12-card"), "Gork", "Este Primata do elemento fogo em estágio 1 da raça Guerreiro com ataque 29 e defesa 20, possui poderes vulcânicos.", Resources.LoadAll<Sprite>("imagens/Elementais/12/12-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/12/12_arco"), 29, 20, 1, 1, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 12, Resources.Load<Sprite>("imagens/Elementais/13/13-card"), "Gaeron", "Gaeron de fogo é está em seu primeiro estágio evolutivo, sua maior habilidade são os disparos de pequenas bolas de chamas. Em sua cabeça e coluna é possível ver alguns ossos saltando de seu corpo.", Resources.LoadAll<Sprite>("imagens/Elementais/13/13-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/13/13_arco"), 33, 5, 1, 1, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 13, Resources.Load<Sprite>("imagens/Elementais/14/14-card"), "Gaeron", "Este Elemental felino em estágio de evolução 2 possui suas habilidades e amaduras melhoradas e fortificadas.", Resources.LoadAll<Sprite>("imagens/Elementais/14/14-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/14/14_arco"), 69, 48, 1, 2, fogo));
        deckTotal.Add(new CartaGeral(TipoCarta.Elemental, 14, Resources.Load<Sprite>("imagens/Elementais/15/15-card"), "Gaeron", "Evolução de estágio 2 do felino de terra da classe Tanque e possui uma forte defesa decorrente de sua armadura evoluida.", Resources.LoadAll<Sprite>("imagens/Elementais/15/15-sprites").ToList(), Resources.Load<Sprite>("imagens/Elementais/15/15_arco"), 38, 57, 1, 2, terra));

        AuxArmGeral armadilha = new AuxArmGeral(Resources.Load<Sprite>("imagens/Armadilhas/molduraArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/iconArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/maskArmadilha"), Resources.Load<Sprite>("imagens/Armadilhas/armMolduraCampo"));
        AuxArmGeral auxiliar = new AuxArmGeral(Resources.Load<Sprite>("imagens/auxiliares/molduraAuxiliar"), Resources.Load<Sprite>("imagens/auxiliares/iconAuxiliar"), Resources.Load<Sprite>("imagens/auxiliares/maskAuxiliar"), Resources.Load<Sprite>("imagens/auxiliares/auxMolduraCampo"));

        deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 15, Resources.Load<Sprite>("imagens/Armadilhas/bloqueio/bloqueioCarta"), "Bloqueio", "Bloqueia o efeito e destrói a carta Auxiliar do oponente assim que for ativada.", Resources.LoadAll<Sprite>("imagens/Armadilhas/bloqueio/bloqueioSprites").ToList(), armadilha, "", null));
        deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 16, Resources.Load<Sprite>("imagens/Armadilhas/diaDeSorte/diaDeSorteCarta"), "Dia de Sorte", "Se seu Elemental for destruído neste turno, aumente em +20 de ATAQUE um Elemental aleatório do seu campo.", Resources.LoadAll<Sprite>("imagens/Armadilhas/diaDeSorte/diaDeSorteSprites").ToList(), armadilha, "", null));
        deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 17, Resources.Load<Sprite>("imagens/Armadilhas/duploSentido/duploSentidoCarta"), "Duplo Sentido", "O ATAQUE equivalente do Elemental atacante também atinge os pontos de vida do oponente.", Resources.LoadAll<Sprite>("imagens/Armadilhas/duploSentido/duploSentidoSprites").ToList(), armadilha, "", null));
        deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 18, Resources.Load<Sprite>("imagens/Armadilhas/escudoDeVida/escudoDeVidaCarta"), "Escudo de Vida", "Absorve o ataque do elemental e converte em pontos de vida para o jogador.", Resources.LoadAll<Sprite>("imagens/Armadilhas/escudoDeVida/escudoDeVidaSprites").ToList(), armadilha, "", null));
        deckTotal.Add(new CartaGeral(TipoCarta.Auxiliar, 19, Resources.Load<Sprite>("imagens/Armadilhas/espelho/espelhoCarta"), "Espelho", "Cria um espelho no campo devolvendo o ataque para o Elemental que esta atacando.", Resources.LoadAll<Sprite>("imagens/Armadilhas/espelho/espelhoSprites").ToList(), armadilha, "", null));

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
        //camposParaJogarMagicas = gerenciadorJogo.VerificaCampoDisponivelMagicas(tipoIA);
        //campoOcupadosMagicas = gerenciadorJogo.VerificaCampoOcupadoMagicas(tipoIA);
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
    CartaGeral PensaJogadaMagica()
    {
        return null;
    }
    SlotCampo CampoEcolhidoAtivar(List<SlotCampo> camposParaAtivar)
    {
        return camposParaAtivar.Count > 0 ? camposParaAtivar[Random.Range(0, camposParaAtivar.Count)] : null;
    }
    SlotCampo CampoEcolhidoJogar(List<SlotCampo> camposDisponiveis)
    {

        return camposDisponiveis.Count > 0 ? camposDisponiveis[Random.Range(0, camposDisponiveis.Count)] : null;
    }
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
}
