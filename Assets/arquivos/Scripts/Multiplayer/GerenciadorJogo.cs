using Cysharp.Threading.Tasks;
using DG.Tweening;
using funcoesUteis;
using MEC;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GerenciadorJogo : MonoBehaviourPunCallbacks
{
    #region Set variables
    //###-> VARIAVEIS PRIVADAS AQUI <-###//
    private const byte DROP_ELEMENTAL_INIMIGO = 0;
    private const byte EXECUTA_ATAQUE = 1;
    private const byte ATIVAR_ELEMENTAL = 3;
    private const byte TROCA_TURNO = 4;
    private const byte EXECUTA_ATAQUE_DIRETO = 5;
    private const byte EXECUTA_ATAQUE_CAMPO = 6;

    public Sprite resultadoVoce, resultadoOponente;
    //-------------------------------------------------
    public GameObject panelSorteio;
    public Image imgAnimSorteio;
    public Image imgAnimMoeda;
    public Image imgResultado;

    [Header("GERENCIADORES")]
    public GerenciadorUI gerenciadorUI;
    public AudioBase gerenciadorAudio;
    public static GerenciadorJogo instance;
    public IA ia;

    [Header("DECK TOTAL")]
    public List<CartaGeral> deckTotal = new List<CartaGeral>();

    [Header("CARTAS PLAYER")]
    public List<CartaGeral> deckPlayer = new List<CartaGeral>();
    public List<CartaGeral> deckMaoPlayer = new List<CartaGeral>();
    public List<CartaGeral> cemiterioPlayer = new List<CartaGeral>();
    public SlotCampo ultimoCampoJogado = new SlotCampo();
    public SlotCampo ultimoCampoAtivado = new SlotCampo();

    [Header("CARTA")]
    public List<Sprite> brilhoCarta;
    public GameObject cartaPrefab;
    public GameObject panelCartas;

    [Header("VERIFICADORES")]
    public bool rodandoAnimacao = false;
    public bool movimentandoCarta = false;
    public bool turnoLocal = false;
    public bool EmJogo;
    public bool turnoDefesa;
    public int JogadasPlayer = 1;
    public bool p1 = false;
    public bool emBatalha = false;
    public bool ataqueDireto = false;
    public bool executarAnimAtaque = false;
    public bool aguardarAnimAtaque = false;
    public Image animAuxArm;
    public bool ExecutandoAnimacao { get; private set; }

    [Header("CAMPOS")]
    public List<SlotCampo> slotsCampoP1;
    public List<SlotCampo> slotsCampoP2;
    public List<SlotDrop> camposMarcados;
    public Sprite imgDropElementalPos;

    [Header("ATAQUES")]
    public List<Sprite> marcadoresAtaque;
    [SerializeField] public int[,] ataques = new int[3, 2];
    public int contagemAtaques = 0;

    [Header("CRONOMETRO")]
    public float tempoTurno = 60f, tempoBatalha = 30f;
    public float tempoCronometro = 0f;

    [Header("SELETORES(ENUMERATORS)")]
    public TipoJogador turno;
    public TipoJogador defensor;
    public TipoJogador selecionando;
    public TipoFase faseAtual;
    public TipoPartida tipoPartida;

    [Header("FASE")]
    public int fase;
    private bool rodandoAnimacaoATAQUE = false;
    #endregion

    #region UnityDefaults
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        ia = IA.instance;
        gerenciadorAudio = AudioBase._instance;
        gerenciadorUI = GerenciadorUI.gerenciadorUI;
        gerenciadorAudio.playMusicaGameplay();
        Timing.RunCoroutine(iniciarObjetos());
        if (tipoPartida == TipoPartida.LOCAL)
        {
            turnoLocal = true;
        }
        JogadasPlayer = 1;
        //gerenciadorUI.trocaBtnTurno(turnoLocal ? 1 : 0);
        var c = deckPlayer.Where(x => x.tipoCarta == TipoCarta.Auxiliar && x.idCarta == 21).FirstOrDefault();
        if (c != null)
        {
            passarParaMaoEspecifica(c);
        }

        c = deckPlayer.Where(x => x.tipoCarta == TipoCarta.Auxiliar).FirstOrDefault();
        if (c != null)
        {
            passarParaMaoEspecifica(c);
        }
        else
            Debug.Log("nao entrado");

        for (int i = 0; i < 5; i++)
        {
            int id = UnityEngine.Random.Range(0, deckPlayer.Count - 1);
            passarParaMao(id, false);
        }
        sorteio();

        fase = 1;
        tempoCronometro = tempoTurno;
    }



    void Update()
    {
        if (EmJogo)
        {
            //SetDefensor();
            if (!emBatalha)
            {
                var t = Cronometro();
                if (t)
                {
                    PassaTurno();
                }
            }
            else
            {

                if (!aguardarAnimAtaque)
                {
                    if (!executarAnimAtaque)
                    {

                        var t = Cronometro();
                        if (t)
                        {
                            gerenciadorUI.atualizaCronometro(0f);
                            PassaFaseBatalha();
                        }
                    }
                    else
                    {
                        gerenciadorUI.atualizaCronometro(0f);
                        PassaFaseBatalha();
                    }

                }
            }
        }
    }
    #endregion

    #region Turno
    public void btnTrocaTurno()
    {
        if (turno == TipoJogador.PLAYER && !rodandoAnimacao)
        {
            PassaTurno();
        }
    }
    public void trocaTurno(bool turno)
    {
        gerenciadorAudio.playTrocaturnoprincipal();
        turnoLocal = !turnoLocal;
        gerenciadorUI.trocaBtnTurno(turnoLocal ? 1 : 0);
        tempoCronometro = tempoTurno;

        PassaTurno();
    }
    public void PassaTurno()
    {

        gerenciadorAudio.playTrocaturnoprincipal();
        slotsCampoP1.ForEach(x => x.marcado = false);
        slotsCampoP2.ForEach(x => x.marcado = false);
        contagemAtaques = 0;
        faseAtual = TipoFase.MONSTRO;
        tempoCronometro = tempoTurno;
        aguardarAnimAtaque = false;
        emBatalha = false;
        executarAnimAtaque = false;
        gerenciadorUI.HabilitarAlertaBatalha(false, null);
        if (turno == TipoJogador.IA)
        {
            LimpaMagicas(TipoJogador.IA);
            ReexecutarMagicas(TipoJogador.PLAYER);
            defensor = TipoJogador.IA;
            gerenciadorUI.trocaBtnTurno(1);
            JogadasPlayer++;
            turno = TipoJogador.PLAYER;
            p1 = true;
            StartCoroutine(gerenciadorUI.AnimacaoTrocarBandeiraTurno(true));
            ia.possoJogar = false;
            ultimoCampoJogado = null;
            ultimoCampoAtivado = null;
            sortearDeck(true);
            selecionando = TipoJogador.PLAYER;
            
        }
        else
        {
            LimpaMagicas(TipoJogador.PLAYER);
            ReexecutarMagicas(TipoJogador.IA);
            ia.SetDefaults();
            defensor = TipoJogador.PLAYER;
            p1 = false;
            gerenciadorUI.trocaBtnTurno(0);
            StartCoroutine(gerenciadorUI.AnimacaoTrocarBandeiraTurno(false));
            turno = TipoJogador.IA;
            selecionando = TipoJogador.IA;
        }
        RemoverCampoMarcado();

    }
    public void setTurnoBatalha()
    {
        if (!emBatalha)
        {
            tempoCronometro = tempoBatalha;
            if (turno == TipoJogador.IA)
            {
                selecionando = TipoJogador.IA;
                gerenciadorUI.HabilitarAlertaBatalha(true, "Prepare-se para reagir ao ataque!");
                //gerenciadorUI.HabilitarBtnPronto();
                ia.possoJogar = true;
            }
            else
            {
                selecionando = TipoJogador.PLAYER;
                gerenciadorUI.HabilitarBtnPronto();
                gerenciadorUI.HabilitarAlertaBatalha(true, "Selecione os elementais que deseja atacar!");
            }
            faseAtual = TipoFase.DEFESA;
            emBatalha = true;
        }

    }
    #endregion

    #region cronometro
    private bool Cronometro()
    {
        bool b;
        if (tempoCronometro >= 0f)
        {
            tempoCronometro -= 1f * Time.deltaTime;
            gerenciadorUI.atualizaCronometro(tempoCronometro);
            b = false;
        }
        else
        {
            b = true;
        }
        return b;
    }
    public void rpcZeraCronometro()
    {
        tempoCronometro = 1f;
    }
    #endregion

    #region DragDropCarta
    public void startDragCarta()
    {
        int id = 0;
        foreach (var slot in slotsCampoP1)
        {
            if (!slot.ocupado)
            {
                DOTween.Restart("slot" + id);
            }

            id++;
        }
    }
    public void startDragCarta(TipoCarta tipoCarta)
    {
        int id = 0;
        foreach (var slot in slotsCampoP1)
        {
            if (!slot.ocupado)
            {
                if ((tipoCarta == TipoCarta.Elemental) && (id <= 2))
                {
                    DOTween.Restart("slot" + id);
                }
                else if (((tipoCarta == TipoCarta.Auxiliar) || (tipoCarta == TipoCarta.Armadilha)) && (id >= 3))
                {
                    DOTween.Restart("slot" + id);
                }
            }
            id++;
        }
    }
    public void endDragCarta()
    {
        int id = 0;
        foreach (var slot in slotsCampoP1)
        {
            if (!slot.ocupado)
            {
                DOTween.Pause("slot" + id);
                slot.imgElementalCampo.DOFade(0, 1);
            }

            id++;
        }
    }
    public void dropElemental(GameObject cartaPrefabLocal, int idSlot)
    {
        slotsCampoP1[idSlot].ocupado = true;
        DOTween.Pause("slot" + idSlot);
        CartaPrefab prefabScript = cartaPrefabLocal.GetComponent<CartaPrefab>();
        CartaGeral cartaGeral = prefabScript.cartaGeral;
        deckMaoPlayer.Remove(cartaGeral);

        slotsCampoP1[idSlot].imgAnimAtivar.ZeraAlfa();
        slotsCampoP1[idSlot].imgElementalCampo.ZeraAlfa();

        gerenciadorAudio.playCartaBaixando();
        JogadasPlayer = 0;
        Sequence surgirElemental = DOTween.Sequence().SetId("ativarElemental");
        surgirElemental.Append(cartaPrefabLocal.GetComponent<CanvasGroup>().DOFade(0, 0));
        surgirElemental.AppendCallback(() =>
        {
            //destruir apos animação
            Destroy(cartaPrefabLocal);
        });
        surgirElemental.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(slotsCampoP1[idSlot].imgAnimAtivar, cartaGeral.elemento.animCriar, false, 6,
                false, () => { slotsCampoP1[idSlot].imgAnimAtivar.DOFade(0, .3f); });
        });
        surgirElemental.AppendInterval(1);
        surgirElemental.AppendCallback(() =>
        {
            //AQUI
            gerenciadorAudio.playInvocacaoCriatura(cartaGeral.elemento.idElemento);
        });
        surgirElemental.Join(slotsCampoP1[idSlot].imgAnimAtivar.DOFade(1, 1f));
        surgirElemental.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(slotsCampoP1[idSlot].imgElementalCampo, cartaGeral.animCampo, true, 4, false,
                null, "P1" + idSlot.ToString());
            surgirElemental.Append(slotsCampoP1[idSlot].imgElementalCampo.DOFade(1, 1f));
        });

        gerenciadorUI.slotDetalhesP1[idSlot].setarInformações(cartaGeral);
        gerenciadorUI.slotDetalhesP1[idSlot].alteraArco(1);
        slotsCampoP1[idSlot].cartaGeral = cartaPrefabLocal.GetComponent<CartaPrefab>().cartaGeral;
        ultimoCampoJogado = slotsCampoP1[idSlot];

        //chama a função dropInimigo no segundo jogador
        //object[] valoresRede = new object[] { cartaGeral.idCarta, idSlot };
        //PhotonNetwork.RaiseEvent(DROP_ELEMENTAL_INIMIGO, valoresRede, Photon.Realtime.RaiseEventOptions.Default, SendOptions.SendUnreliable);
        //photonView.RPC("dropElementalInimigo", Photon.Pun.RpcTarget.AllBufferedViaServer, cartaGeral.idCarta, idSlot);
    }
    public void dropElementalInimigo(int idCarta, int idSlot)
    {
        if (!turnoLocal)
        {
            CartaGeral cartaGeral = deckTotal[idCarta];

            slotsCampoP2[idSlot].ocupado = true;

            slotsCampoP2[idSlot].imgAnimAtivar.ZeraAlfa();
            slotsCampoP2[idSlot].imgElementalCampo.ZeraAlfa();

            Sequence surgirElemental = DOTween.Sequence().SetId("ativarElementalInimigo");
            surgirElemental.AppendCallback(() =>
            {
                FuncoesUteis.animacaoImagem(slotsCampoP2[idSlot].imgAnimAtivar, cartaGeral.elemento.animCriar,
                    false, 6, false, () => { slotsCampoP2[idSlot].imgAnimAtivar.DOFade(0, .3f); });
            });
            surgirElemental.Join(slotsCampoP2[idSlot].imgAnimAtivar.DOFade(1, .5f));
            surgirElemental.AppendCallback(() =>
            {
                FuncoesUteis.animacaoImagem(slotsCampoP2[idSlot].imgElementalCampo, cartaGeral.animCampo, true, 4,
                    false, null, "P2" + idSlot.ToString());
            });
            surgirElemental.Join(slotsCampoP2[idSlot].imgElementalCampo.DOFade(1, .5f));

            gerenciadorUI.slotDetalhesP2[idSlot].setarInformações(cartaGeral);
            gerenciadorUI.slotDetalhesP2[idSlot].alteraArco(1);
        }
    }
    public void dropAuxArm(GameObject cartaPrefabLocal, int idSlot)
    {

        Debug.Log(idSlot);
        gerenciadorAudio.playCartaBaixando();
        slotsCampoP1[idSlot].ocupado = true;
        slotsCampoP1[idSlot].disponivel = true;
        DOTween.Pause("slot" + idSlot);
        CartaPrefab prefabScript = cartaPrefabLocal.GetComponent<CartaPrefab>();
        CartaGeral cartaGeral = prefabScript.cartaGeral;
        slotsCampoP1[idSlot].cartaGeral = cartaGeral;
        deckMaoPlayer.Remove(cartaGeral);
        if (cartaGeral.efeitoAo == EfeitoAo.Dropar)
        {
            cartaGeral.executaAcoes(this, null, false);
            cartaGeral.qtdTurnos--;
            slotsCampoP1[idSlot].ativado = true;
        }

        slotsCampoP1[idSlot].imgAnimAtivar.ZeraAlfa();
        slotsCampoP1[idSlot].imgElementalCampo.ZeraAlfa();
        slotsCampoP1[idSlot].imgElementalCampo.ZeraAlfa();

        slotsCampoP1[idSlot].canvasGroup.ZeraAlfa();


        //slotsCampoP1[idSlot].mask.sprite = cartaGeral.auxArm.mask;
        slotsCampoP1[idSlot].molduraCampo.sprite = cartaGeral.auxArm.molduraCampo;
        slotsCampoP1[idSlot].fotocarta.sprite = cartaGeral.imgCarta;



        Sequence surgirAuxArm = DOTween.Sequence().SetId("surgirAuxArm");
        surgirAuxArm.Append(cartaPrefabLocal.GetComponent<CanvasGroup>().DOFade(0, 0));
        surgirAuxArm.Append(slotsCampoP1[idSlot].canvasGroup.DOFade(1, .3f));
        surgirAuxArm.AppendCallback(() =>
        {
            Destroy(cartaPrefabLocal);
        });


    }
    public void dropAuxArm(CartaGeral cartaGeral, int idSlot, TipoJogador jogador)
    {
        
        Debug.Log(idSlot);
        gerenciadorAudio.playCartaBaixando();
        slotsCampoP2[idSlot].ocupado = true;
        slotsCampoP2[idSlot].ativado = true;
        slotsCampoP2[idSlot].disponivel = true;
        DOTween.Pause("slot" + idSlot);
        deckMaoPlayer.Remove(cartaGeral);
        slotsCampoP2[idSlot].cartaGeral = cartaGeral;
        slotsCampoP2[idSlot].imgAnimAtivar.ZeraAlfa();
        slotsCampoP2[idSlot].imgElementalCampo.ZeraAlfa();
        slotsCampoP2[idSlot].imgElementalCampo.ZeraAlfa();

        //-----------TESTAR JUNTO COM A IA-------------//
        if (cartaGeral.efeitoAo == EfeitoAo.Dropar)
        {
            cartaGeral.executaAcoes(this, null, true);
            cartaGeral.qtdTurnos--;
        }

        slotsCampoP2[idSlot].canvasGroup.ZeraAlfa();

        //slotsCampoP1[idSlot].mask.sprite = cartaGeral.auxArm.mask;
        slotsCampoP2[idSlot].molduraCampo.sprite = cartaGeral.auxArm.molduraCampo;
        slotsCampoP2[idSlot].fotocarta.sprite = cartaGeral.imgCarta;

        Sequence surgirAuxArm = DOTween.Sequence().SetId("surgirAuxArm");
        surgirAuxArm.Append(slotsCampoP2[idSlot].canvasGroup.DOFade(1, .3f));

    }

    #endregion

    #region ataque
    public void executaAtaque(int idAtacante, int idInimigo, bool p1)
    {

        //salva no vetor de ataques quem atacou/foi atacado para posteriormente rodar as animações
        ataques[contagemAtaques, 0] = idAtacante;
        ataques[contagemAtaques, 1] = idInimigo;

        Debug.Log(ataques[contagemAtaques, 0] + "-" + ataques[contagemAtaques, 1] + "/" + contagemAtaques);

        //incrementa o indice dos ataques
        contagemAtaques++;

        //aumenta 5 segundo no tempo após ataque
        //tempoCronometro += 5;

        //adicionar marcadores no campo
        alteraMarcadoresAtaque(idAtacante, idInimigo, p1);

        if (turnoLocal)
        {
            //object[] valoresRede = new object[] { idAtacante, idInimigo, false };
            //PhotonNetwork.RaiseEvent(EXECUTA_ATAQUE_CAMPO, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
        }
    }
    public void executaAtaqueLocal(int idAtacante, int idInimigo, bool P1)
    {
        int vida;

        if (P1)
        {
            gerenciadorUI.inicioAtaque(gerenciadorUI.slotDetalhesP1[idAtacante].cartaGeral,
                gerenciadorUI.slotDetalhesP2[idInimigo].cartaGeral, P1);
            vida = gerenciadorUI.slotDetalhesP2[idInimigo].cartaGeralTemp.defesa -=
                gerenciadorUI.slotDetalhesP1[idAtacante].cartaGeralTemp.ataque;
        }
        else
        {
            gerenciadorUI.inicioAtaque(gerenciadorUI.slotDetalhesP1[idInimigo].cartaGeral,
                gerenciadorUI.slotDetalhesP2[idAtacante].cartaGeral, P1);
            vida = gerenciadorUI.slotDetalhesP1[idInimigo].cartaGeralTemp.defesa -=
                gerenciadorUI.slotDetalhesP2[idAtacante].cartaGeralTemp.ataque;
        }

        Debug.Log(vida.ToString());

        if (vida <= 0)
        {
            morteElemental(P1, idInimigo);
        }
        else
        {
            gerenciadorUI.slotDetalhesP2[idInimigo].atualizarInformações();
        }
    }
    private void executaAtaqueDiretoLocal(int subVida, bool p)
    {
        if (p)
        {
            //object[] valoresRede = new object[] { subVida, false };
            //PhotonNetwork.RaiseEvent(EXECUTA_ATAQUE_DIRETO, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);

            gerenciadorAudio.playDanoDiretoFeminino();
        }
        else
        {
            gerenciadorAudio.playDanoDiretoMasculino();
        }

        gerenciadorUI.AtaqueDireto(p, subVida);
    }
    public void executaAtaqueDiretoIA(int subVida)
    {
        gerenciadorAudio.playDanoDiretoMasculino();
        gerenciadorUI.AtaqueDireto(false, subVida);
    }
    public void fimAtaqueDireto(int subVida, bool P1)
    {
        //verificar se morreu
        int vidaAtual = gerenciadorUI.slotsPlayer[P1 ? 1 : 0].vida - subVida;

        if (vidaAtual <= 0)
        {
            EmJogo = false;
            gerenciadorUI.slotsPlayer[P1 ? 1 : 0].alteraVida(0);
            gerenciadorUI.animVitoriaDerrota(P1);
            //PhotonNetwork.Disconnect();
        }
        else
        {
            gerenciadorUI.slotsPlayer[P1 ? 1 : 0].alteraVida(vidaAtual);
        }
    }
    public void alteraMarcadoresAtaque(int idAtacante, int idInimigo, bool P1)
    {
        if (P1) //se jogador 1 está atacando
        {
            slotsCampoP1[idAtacante].imgAnimAtaque.sprite = marcadoresAtaque[1];
            slotsCampoP1[idAtacante].imgAnimAtaque.DOFade(1, .5f);
            //----------------------------------------------
            if (idInimigo < 3)
            {
                slotsCampoP2[idInimigo].imgAnimAtaque.sprite = marcadoresAtaque[2];
                slotsCampoP2[idInimigo].imgAnimAtaque.DOFade(1, .5f);
            }
        }
        else
        {
            slotsCampoP2[idAtacante].imgAnimAtaque.sprite = marcadoresAtaque[1];
            slotsCampoP2[idAtacante].imgAnimAtaque.DOFade(1, .5f);
            slotsCampoP2[idAtacante].marcado = true;
            //----------------------------------------------
            if (idInimigo < 3)
            {
                slotsCampoP1[idInimigo].imgAnimAtaque.sprite = marcadoresAtaque[2];
                slotsCampoP1[idInimigo].imgAnimAtaque.DOFade(1, .5f);
                slotsCampoP1[idInimigo].marcado = true;
            }
        }
    }
    void zeraAtaques()
    {
        for (int i = 0; i <= 2; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                ataques[i, j] = -1;
            }

            slotsCampoP1[i].imgAnimAtaque.sprite = marcadoresAtaque[0];
            slotsCampoP2[i].imgAnimAtaque.sprite = marcadoresAtaque[0];
        }
    }
    #endregion

    #region Animacoes
    public static IEnumerator<float> rodaAtaqueCampo(bool p1)
    {
        Debug.Log("Rodando ataque campo");

        GerenciadorJogo gerenciadorJogo =
            GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
        gerenciadorJogo.rodandoAnimacaoATAQUE = true;
        Sequence s;
        gerenciadorJogo.gerenciadorUI.DesabilitarBtnPronto();
        gerenciadorJogo.gerenciadorUI.HabilitarAlertaBatalha(false, null);
        SlotCampo atacanteCampo, inimigoCampo;
        SlotElemental atacanteUI, inimigoUI;

        for (int i = 0; i <= 2; i++)
        {
            if (gerenciadorJogo.ataques[i, 0] > -1 && gerenciadorJogo.ataques[i, 1] > -1)
            {
                if (p1)
                {
                    atacanteCampo = gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 0]];
                    atacanteUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP1[gerenciadorJogo.ataques[i, 0]];
                }
                else
                {
                    atacanteCampo = gerenciadorJogo.slotsCampoP2[gerenciadorJogo.ataques[i, 0]];
                    atacanteUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP2[gerenciadorJogo.ataques[i, 0]];
                }
                if (gerenciadorJogo.ataques[i, 1] < 3)
                {
                    if (p1)
                    {
                        inimigoCampo = gerenciadorJogo.slotsCampoP2[gerenciadorJogo.ataques[i, 1]];
                        inimigoUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP2[gerenciadorJogo.ataques[i, 1]];
                    }
                    else
                    {
                        inimigoCampo = gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 1]];
                        inimigoUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP1[gerenciadorJogo.ataques[i, 1]];
                    }

                    // PARTE 1 ANIMAÇÃO
                    s = DOTween.Sequence();
                    s.Append(gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 0]].imgAnimAtaque.DOFade(0, .5f));
                    s.AppendCallback(() => FuncoesUteis.animacaoImagem(atacanteCampo.imgAnimAtaque,
                        atacanteUI.cartaGeral.elemento.animAtaque1, false, 5));
                    s.Join(atacanteCampo.imgAnimAtaque.DOFade(1, .5f));
                    s.AppendInterval(0.5f);
                    s.Append(atacanteCampo.imgAnimAtaque.DOFade(0, .5f));

                    gerenciadorJogo.gerenciadorAudio.playAtaqueElemental(atacanteUI.cartaGeral.elemento.idElemento);
                    //----------------------Anim parte 2------------------------
                    if (inimigoCampo.ocupado)
                    {
                        s.Append(inimigoCampo.imgAnimAtaque.DOFade(0, .5f));
                        s.AppendCallback(() => FuncoesUteis.animacaoImagem(inimigoCampo.imgAnimAtaque,
                            atacanteUI.cartaGeral.elemento.animAtaque2, false, 5));

                        s.Join(inimigoCampo.imgAnimAtaque.DOFade(1, .5f));
                        s.AppendInterval(1.1f);
                        s.Append(inimigoCampo.imgAnimAtaque.DOFade(0, .5f));

                        yield return Timing.WaitForSeconds(5);

                        int vida = inimigoUI.cartaGeralTemp.defesa -= atacanteUI.cartaGeralTemp.ataque;

                        if (vida <= 0)
                        {
                            gerenciadorJogo.morteElemental(p1, gerenciadorJogo.ataques[i, 1]);
                            gerenciadorJogo.executaAtaqueDiretoLocal(Mathf.Abs(vida), p1);
                            yield return Timing.WaitForSeconds(3);
                        }
                        else
                        {
                            inimigoUI.atualizarInformações();
                        }
                    }
                }
                else // ATAQUE DIRETO
                {
                    if (p1)
                    {
                        atacanteCampo = gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 0]];
                        atacanteUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP1[gerenciadorJogo.ataques[i, 0]];
                    }
                    else
                    {
                        atacanteCampo = gerenciadorJogo.slotsCampoP2[gerenciadorJogo.ataques[i, 0]];
                        atacanteUI = gerenciadorJogo.gerenciadorUI.slotDetalhesP2[gerenciadorJogo.ataques[i, 0]];
                    }

                    // PARTE 1 ANIMAÇÃO
                    s = DOTween.Sequence();
                    s.Append(gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 0]].imgAnimAtaque.DOFade(0, .5f));
                    s.AppendCallback(() => FuncoesUteis.animacaoImagem(atacanteCampo.imgAnimAtaque,
                        atacanteUI.cartaGeral.elemento.animAtaque1, false, 6));
                    s.Join(atacanteCampo.imgAnimAtaque.DOFade(1, .5f));
                    s.AppendInterval(1.1f);
                    s.Append(atacanteCampo.imgAnimAtaque.DOFade(0, .5f));
                    yield return Timing.WaitForSeconds(3);

                    gerenciadorJogo.executaAtaqueDiretoLocal(atacanteUI.cartaGeralTemp.ataque, p1);
                    yield return Timing.WaitForSeconds(3);
                }
            }

            if (gerenciadorJogo.gerenciadorUI.slotsPlayer[0].vida <= 0 ||
                gerenciadorJogo.gerenciadorUI.slotsPlayer[1].vida <= 0)
            {
                break;
            }
        }

        for (int i = 0; i <= 2; i++)
        {
            if (gerenciadorJogo.slotsCampoP1[i].ativado)
            {
                gerenciadorJogo.slotsCampoP1[i].disponivel = true;
            }

            if (gerenciadorJogo.slotsCampoP2[i].ativado)
            {
                gerenciadorJogo.slotsCampoP2[i].disponivel = true;
            }
        }

        if (gerenciadorJogo.turnoLocal)
        {
            //chama a função de trocar de turno
            //gerenciadorJogo.trocaTurno(false);
            //object[] valoresRede = new object[] { true };
            //PhotonNetwork.RaiseEvent(TROCA_TURNO, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
            //gerenciadorJogo.photonView.RPC("trocaTurno", Photon.Pun.RpcTarget.AllBufferedViaServer, true);
        }
        gerenciadorJogo.rodandoAnimacaoATAQUE = false;
        gerenciadorJogo.zeraAtaques();
        gerenciadorJogo.PassaTurno();


    }
    public void rodaAnimSorteio()
    {
        panelSorteio.SetActive(true);
        StartCoroutine(gerenciadorUI.AnimacaoBandeiraSorteio(p1));
    }
    #endregion

    #region controle de campo
    public void LimpaMagicas(TipoJogador tipoJogador)
    {
        foreach (var item in VerificaCampoOcupadoMagicas(tipoJogador))
        {
            if (item.cartaGeral.qtdTurnos <= 0)
            {
                if (tipoJogador == TipoJogador.PLAYER)
                {
                    slotsCampoP1[item.idCampo].canvasGroup.ZeraAlfa();
                    slotsCampoP1[item.idCampo].disponivel = true;
                    slotsCampoP1[item.idCampo].ativado = false;
                    slotsCampoP1[item.idCampo].ocupado = false;
                }
                else
                {

                    slotsCampoP2[item.idCampo].canvasGroup.ZeraAlfa();
                    slotsCampoP2[item.idCampo].disponivel = true;
                    slotsCampoP2[item.idCampo].ativado = false;
                    slotsCampoP2[item.idCampo].ocupado = false;
                }
            }
        }

    }

    public void ReexecutarMagicas(TipoJogador tipoJogador)
    {
        var carta = VerificaCampoOcupadoMagicas(tipoJogador).Where(x => x.cartaGeral.tipoEfeito == TipoEfeito.Temporario).ToList();
        if (carta.Count > 0)
        {
            foreach (var obj in carta)
            {
                if (obj.cartaGeral.qtdTurnos > 0)
                {
                    obj.cartaGeral.executaAcoes(this);
                    obj.cartaGeral.qtdTurnos--;
                }
                else
                {
                    LimpaMagicas(tipoJogador);
                }
            }
        }
    }
    public List<SlotCampo> VerificaCampoDisponivelMagicas(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<SlotCampo>();
        if (tipoJogador == TipoJogador.PLAYER)
        {
            foreach (var obj in slotsCampoP1)
            {
                if (!obj.ocupado == true && obj.tipoSlot == TipoCarta.AuxArm)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (!obj.ocupado && obj.tipoSlot == TipoCarta.AuxArm)
                {
                    CamposVazios.Add(obj);
                }
            }
        }

        return CamposVazios;
    }
    public List<SlotCampo> VerificaCampoOcupadoMagicas(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<SlotCampo>();
        if (tipoJogador == TipoJogador.PLAYER)
        {
            foreach (var obj in slotsCampoP1)
            {
                if (obj.ocupado && obj.tipoSlot == TipoCarta.AuxArm)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (obj.ocupado && obj.tipoSlot == TipoCarta.AuxArm)
                {
                    CamposVazios.Add(obj);
                }
            }
        }

        return CamposVazios;
    }
    public List<SlotCampo> VerificaCampoDisponivelMonstros(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<SlotCampo>();
        if (tipoJogador == TipoJogador.PLAYER)
        {
            foreach (var obj in slotsCampoP1)
            {
                if (!obj.ocupado && obj.tipoSlot == TipoCarta.Elemental)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (!obj.ocupado && obj.tipoSlot == TipoCarta.Elemental)
                {
                    CamposVazios.Add(obj);
                }
            }
        }

        return CamposVazios;
    }
    public List<SlotCampo> VerificaCampoOcupadoMonstros(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<SlotCampo>();
        if (tipoJogador == TipoJogador.PLAYER)
        {
            foreach (var obj in slotsCampoP1)
            {
                if (obj.ocupado && obj.tipoSlot == TipoCarta.Elemental)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (obj.ocupado && obj.tipoSlot == TipoCarta.Elemental)
                {
                    CamposVazios.Add(obj);
                }
            }
        }

        return CamposVazios;
    }
    public bool VerificaCampoMarcado(SlotDrop campo)
    {
        if (campo.donoCampo == TipoJogador.PLAYER)
        {
            if (camposMarcados.Where(x => x == campo).ToList().Count > 0)
                return false;
        }
        return true;
    }
    public void AdicionarCampoMarcado(SlotDrop campo)
    {
        camposMarcados.Add(campo);
        Debug.Log("Campo marcado!");
    }
    public void RemoverCampoMarcado()
    {
        camposMarcados.Clear();
        Debug.Log("Campos marcados limpos");
    }
    #endregion

    #region ativacao
    public void ativarElemental(int idSlotElemental, int idSlotCristal)
    {

        if (idSlotCristal == gerenciadorUI.slotDetalhesP1[idSlotElemental].cartaGeralTemp.elemento.idElemento)
        {
            if (gerenciadorUI.slotsCristais[idSlotCristal].qtdCristais >=
                gerenciadorUI.slotDetalhesP1[idSlotElemental].cartaGeralTemp.cristais)
            {
                gerenciadorUI.slotsCristais[idSlotCristal]
                    .usarCristais(gerenciadorUI.slotDetalhesP1[idSlotElemental].cartaGeralTemp.cristais);
                gerenciadorUI.slotDetalhesP1[idSlotElemental].alteraArco(2);
                slotsCampoP1[idSlotElemental].ativado = true;
                ultimoCampoAtivado = slotsCampoP1[idSlotElemental];
                //object[] valoresRede = new object[] { idSlotElemental, idSlotCristal, false };
                //PhotonNetwork.RaiseEvent(ATIVAR_ELEMENTAL, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
            }
            else
            {
                gerenciadorUI.MostrarAlerta("Faltam cristais para ativar esse elemental!");
                gerenciadorAudio.playNegacao();
            }
        }
        object[] valoresRede = new object[] { idSlotElemental, idSlotCristal, false };
        //PhotonNetwork.RaiseEvent(ATIVAR_ELEMENTAL, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
    }
    public void ativarElementalAI(int idSlotElemental)
    {
        gerenciadorUI.slotDetalhesP2[idSlotElemental].alteraArco(2);
        slotsCampoP2[idSlotElemental].ativado = true;

    }
    #endregion

    #region Controle de jogo

    private void passarParaMaoEspecifica(CartaGeral c, bool rodaAnimacao = true)
    {

        if (deckMaoPlayer.Count >= 7)
        {
            gerenciadorUI.MostrarAlerta("Sua mao está cheia.");
            gerenciadorAudio.playNegacao();
        }
        else
        {
            deckPlayer.Remove(c);
            GameObject _cartaPrefab = Instantiate(cartaPrefab, panelCartas.transform);
            CartaPrefab scriptPrefab = _cartaPrefab.GetComponent<CartaPrefab>();
            scriptPrefab.atributos(c);
            if (rodaAnimacao)
            {
                scriptPrefab.aparecerAnimacaoZoom();
            }
            else
            {
                scriptPrefab.aparecerAnimacaoFade();
            }
            deckMaoPlayer.Add(c);
        }

    }
    public void sorteio()
    {
        int valor = UnityEngine.Random.Range(0, 100);
        if (valor >= 0 && valor <= 49)
        {
            turno = TipoJogador.PLAYER;
            defensor = TipoJogador.IA;
            selecionando = turno;
            gerenciadorUI.trocaBtnTurno(1);
            p1 = true;
        }
        else
        {
            p1 = false;
            turno = TipoJogador.IA;
            defensor = TipoJogador.PLAYER;
            selecionando = turno;
            ia.possoJogar = true;
            ia.tempoAtual = 0f;
            gerenciadorUI.trocaBtnTurno(0);
            
        }

        rodaAnimSorteio();
    }
    public void morteElemental(bool P1, int idElemental)
    {
        gerenciadorAudio.playCriaturaMorrendo();

        if (P1) //PLAYER 1 ESTA ATACANDO P2 MORREU
        {
            gerenciadorUI.slotDetalhesP2[idElemental].morte();
            FuncoesUteis.killCorroutines("P2" + idElemental.ToString());
            slotsCampoP2[idElemental].imgElementalCampo.DOFade(0, 1);
            slotsCampoP2[idElemental].ocupado = false;
            slotsCampoP2[idElemental].ativado = false;
            slotsCampoP2[idElemental].disponivel = false;
        }
        else //PLAYER 1 FOI ATACADO E MORREU - JOGADOR LOCAL
        {
            cemiterioPlayer.Add(new CartaGeral(gerenciadorUI.slotDetalhesP1[idElemental].cartaGeral));
            gerenciadorUI.slotDetalhesP1[idElemental].morte();
            FuncoesUteis.killCorroutines("P1" + idElemental.ToString());
            slotsCampoP1[idElemental].imgElementalCampo.DOFade(0, .5f).OnComplete(() =>
            {
                slotsCampoP1[idElemental].imgElementalCampo.sprite = imgDropElementalPos;
            });
            slotsCampoP1[idElemental].ocupado = false;
            slotsCampoP1[idElemental].ativado = false;
            slotsCampoP1[idElemental].disponivel = false;
        }
        foreach (SlotCampo slot in VerificaCampoOcupadoMagicas((!P1) ? TipoJogador.PLAYER : TipoJogador.IA))
        {
            if (slot.cartaGeral.efeitoAo == EfeitoAo.MorteElemental)
            {
                slot.cartaGeral.executaAcoes(this);
            }
        }
    }
    public void passarParaMao(int id, bool rodaAnimacao = true)
    {
        if (deckMaoPlayer.Count >= 7)
        {
            gerenciadorUI.MostrarAlerta("Sua mao está cheia.");
            gerenciadorAudio.playNegacao();
        }
        else
        {
            CartaGeral carta = deckPlayer[id];
            deckPlayer.RemoveAt(id);
            GameObject _cartaPrefab = Instantiate(cartaPrefab, panelCartas.transform);
            CartaPrefab scriptPrefab = _cartaPrefab.GetComponent<CartaPrefab>();
            scriptPrefab.atributos(carta);
            if (rodaAnimacao)
            {
                scriptPrefab.aparecerAnimacaoZoom();
            }
            else
            {
                scriptPrefab.aparecerAnimacaoFade();
            }
            deckMaoPlayer.Add(carta);
        }
    }
    public IEnumerator<float> iniciarObjetos()
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////AUXILIARES/////INICIO/////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Acoes> acaoBarrarAgua = new List<Acoes>();

        acaoBarrarAgua.Add(new Acoes(
            AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        false, // Jogador 1 (true) ou Jogador 2(false)
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoBarrarFogo = new List<Acoes>();

        //acaoBarrarFogo.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                false, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa, // (ataque/defesa)
        //                -4, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { fogo },
        //                false
        //            })
        // );


        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        27, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/barrarFogo/barrarFogoCarta"),//Foto carta
        //        "Barrar[fogo]", // Titulo
        //        "Cause -4 de dano direto em todos os Elementais do tipo fogo no campo", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarFogo/barrarFogoSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //         1,
        //        TipoEfeito.Temporario,
        //        acaoBarrarFogo // lista de ações
        //        )

        //    );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoBarrarTerra = new List<Acoes>();

        //acaoBarrarTerra.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                false, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa, // (ataque/defesa)
        //                -4, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { terra },
        //                false
        //            })
        // );


        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        28, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/barrarTerra/barrarTerraCarta"),//Foto carta
        //        "Barrar[terra]", // Titulo
        //        "Cause -4 de dano direto em todos os Elementais do tipo terra no campo", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarTerra/barrarTerraSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //         1,
        //        TipoEfeito.Temporario,
        //        acaoBarrarTerra // lista de ações
        //        )

        //    );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoBarrarAr = new List<Acoes>();

        //acaoBarrarAr.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                false, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa, // (ataque/defesa)
        //                -4, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { ar },
        //                false
        //            })
        // );


        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        28, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/barrarAr/barrarArCarta"),//Foto carta
        //        "Barrar[ar]", // Titulo
        //        "Cause -4 de dano direto em todos os Elementais do tipo ar no campo", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/barrarAr/barrarArSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Dropar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoBarrarAr // lista de ações
        //        )

        //    );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //List<Acoes> acaoPoseidon = new List<Acoes>();

        //acaoPoseidon.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque, // (ataque/defesa)
        //                10, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { agua },
        //                false
        //            })
        // );

        //acaoPoseidon.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                5,// valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { agua },
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        17, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/pocaoDePoseidon/pocaoDePoseidonCarta"),//Foto carta
        //        "Poçao de Poseidon", // Titulo
        //        "Concede +10  de ATAQUE e +5 de DEFESA a um Elemental de ÁGUA", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDePoseidon/bpocaoDePoseidonSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoPoseidon // lista de ações
        //        )
        //    );


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //List<Acoes> acaoGaia = new List<Acoes>();

        //acaoGaia.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque, // (ataque/defesa)
        //                10, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { terra },
        //                false
        //            })
        // );

        //acaoGaia.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                5,// valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { terra },
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        18, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeGaia/pocaoDeGaiaCarta"),//Foto carta
        //        "Poçao de Gaia", // Titulo
        //        "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de TERRA", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeGaia/pocaoDeGaiaSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoGaia // lista de ações
        //        )

        //    );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoZeus = new List<Acoes>();

        //acaoZeus.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque, // (ataque/defesa)
        //                10, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { ar },
        //                false
        //            })
        // );

        //acaoZeus.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                5,// valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { ar },
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        19, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeZeus/pocaoDeZeusCarta"),//Foto carta
        //        "Poçao de Zeus", // Titulo
        //        "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de AR", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeZeus/pocaoDeZeusSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoZeus // lista de ações
        //        )

        //    );


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoHefesto = new List<Acoes>();

        //acaoHefesto.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque, // (ataque/defesa)
        //                10, // valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { fogo },
        //                false
        //            })
        // );

        //acaoHefesto.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesaPorElemento,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                5,// valor para adicionar/remover no ataque ou defesa
        //                new List<ElementoGeral>() { fogo },
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        20, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/pocaoDeHefesto/pocaoDeHefestoCarta"),//Foto carta
        //        "Poçao de Hefesto", // Titulo
        //        "Concede +10 de ATAQUE e +5 de DEFESA a um Elemental de FOGO", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/pocaoDeHefesto/pocaoDeHefestoSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoHefesto // lista de ações
        //        )

        //    );

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //List<Acoes> acaoEmGloria = new List<Acoes>();

        //acaoEmGloria.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesa, //ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque, // (ataque/defesa)
        //                2,
        //                false
        //            })
        // );

        //acaoEmGloria.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.defesa,// (ataque/defesa)
        //                2,
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        20, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/emGloria/emGloriaCarta"),//Foto carta
        //        "Em Glória", // Titulo
        //        "Concede +2 de ATAQUE e DEFESA para um Elemental no campo em cada turno até o fim da partida ou até que destruam esta carta", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/emGloria/emGloriaSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        99,
        //        TipoEfeito.Continuo,
        //        acaoEmGloria // lista de ações
        //        )

        //    );


        ////////////////////////////////////////////////////////////////////////////////////////
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
        List<Acoes> acaoOleandro = new List<Acoes>();
        acaoOleandro.Add(new Acoes(
            AcoesDisponiveis.alterarVidaPlayer,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
                    new List<object>() {
                        null, //gerenciador jogo -- sempre null
                        null, // id -- sempre null
                        false, // Jogador 1 (true) ou Jogador 2(false)
                        -10,// qtdAltedada
                        false
                    })
         );

        deckTotal.Add(
            new CartaGeral(
                TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
                21, //ID da carta
                Resources.Load<Sprite>("imagens/Auxiliares/oleandro/oleandroCarta"),//Foto carta
                "Oleandro", // Titulo
                "Causa -10 de dano direto na vida do guardião opodente.", //Descricao
                Resources.LoadAll<Sprite>("imagens/Auxiliares/oleandro/oleandroSprites").ToList(), //Animação
                auxiliar, // Tipo carta (para pegar as imagens corretas)
                LocalAnim.CampoMeio, // onde será executada a animação? (CampoCartas/CampoMeio)
                EfeitoAo.Dropar, //(Dropar/Selecionar)
                1,
                TipoEfeito.Temporario,
                acaoOleandro // lista de ações
                )
            );


        //////////////////////////////////////////////////////////////////////////////////////
        ///

        //List<Acoes> acaoReforco = new List<Acoes>();
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


        ////////////////////////////////////////////////////////////////////////////////////////
        //List<Acoes> acaoVigor = new List<Acoes>();
        //acaoVigor.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque,// (ataque/defesa)
        //                5,
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        20, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/vigor/vigorCarta"),//Foto carta
        //        "Vigor", // Titulo
        //        "Escolha um Elemental para receber +5 de ataque", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/vigor/vigorSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        1,
        //        TipoEfeito.Temporario,
        //        acaoVigor // lista de ações
        //        )

        //    );


        ////////////////////////////////////////////////////////////////////////////////////////


        //List<Acoes> acaoFuria = new List<Acoes>();
        //acaoFuria.Add(new Acoes(
        //    AcoesDisponiveis.alterarAtaqueDefesa,//ação (alterarAtaqueDefesaPorElemento/alterarAtaqueDefesa)
        //            new List<object>() {
        //                null, //gerenciador jogo -- sempre null
        //                null, // id -- sempre null
        //                true, // Jogador 1 (true) ou Jogador 2(false)
        //                ParametroAlvo.ataque,// (ataque/defesa)
        //                4,
        //                false
        //            })
        // );

        //deckTotal.Add(
        //    new CartaGeral(
        //        TipoCarta.Auxiliar,//Tipo de carta (Auxiliar/Armadilha)
        //        20, //ID da carta
        //        Resources.Load<Sprite>("imagens/Auxiliares/furia/furiaCarta"),//Foto carta
        //        "Fúria", // Titulo
        //        "Escolha um Elemental para receber +5 de ataque", //Descricao
        //        Resources.LoadAll<Sprite>("imagens/Auxiliares/furia/furiaSprites").ToList(), //Animação
        //        auxiliar, // Tipo carta (para pegar as imagens corretas)
        //        LocalAnim.CampoCartas, // onde será executada a animação? (CampoCartas/CampoMeio)
        //        EfeitoAo.Selecionar, //(Dropar/Selecionar)
        //        2,
        //        TipoEfeito.Temporario,
        //        acaoFuria // lista de ações
        //        )

        //    );


        //////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////AUXILIARES/////FIM////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        deckPlayer = new List<CartaGeral>(deckTotal);

        brilhoCarta = Resources.LoadAll<Sprite>("imagens/UI/brilhoCarta").ToList();
        yield return Timing.WaitForOneFrame;


        Sequence slot1Piscando = DOTween.Sequence().SetId("slot0").SetRecyclable();
        slot1Piscando.Append(slotsCampoP1[0].imgElementalCampo.DOFade(.5f, .4f));
        slot1Piscando.Append(slotsCampoP1[0].imgElementalCampo.DOFade(.7f, .4f));
        slot1Piscando.SetLoops(-1, LoopType.Yoyo);
        slot1Piscando.Pause();

        Sequence slot2Piscando = DOTween.Sequence().SetId("slot1").SetRecyclable();
        slot2Piscando.Append(slotsCampoP1[1].imgElementalCampo.DOFade(.5f, .4f));
        slot2Piscando.Append(slotsCampoP1[1].imgElementalCampo.DOFade(.7f, .4f));
        slot2Piscando.SetLoops(-1, LoopType.Yoyo);
        slot2Piscando.Pause();


        Sequence slot3Piscando = DOTween.Sequence().SetId("slot2").SetRecyclable();
        slot3Piscando.Append(slotsCampoP1[2].imgElementalCampo.DOFade(.5f, .4f));
        slot3Piscando.Append(slotsCampoP1[2].imgElementalCampo.DOFade(.7f, .4f));
        slot3Piscando.SetLoops(-1, LoopType.Yoyo);
        slot3Piscando.Pause();

        Sequence slot4Piscando = DOTween.Sequence().SetId("slot3").SetRecyclable();
        slot4Piscando.Append(slotsCampoP1[3].imgElementalCampo.DOFade(.5f, .4f));
        slot4Piscando.Append(slotsCampoP1[3].imgElementalCampo.DOFade(.7f, .4f));
        slot4Piscando.SetLoops(-1, LoopType.Yoyo);
        slot4Piscando.Pause();

        Sequence slot5Piscando = DOTween.Sequence().SetId("slot4").SetRecyclable();
        slot5Piscando.Append(slotsCampoP1[4].imgElementalCampo.DOFade(.5f, .4f));
        slot5Piscando.Append(slotsCampoP1[4].imgElementalCampo.DOFade(.7f, .4f));
        slot5Piscando.SetLoops(-1, LoopType.Yoyo);
        slot5Piscando.Pause();

        Sequence slot6Piscando = DOTween.Sequence().SetId("slot5").SetRecyclable();
        slot6Piscando.Append(slotsCampoP1[5].imgElementalCampo.DOFade(.5f, .4f));
        slot6Piscando.Append(slotsCampoP1[5].imgElementalCampo.DOFade(.7f, .4f));
        slot6Piscando.SetLoops(-1, LoopType.Yoyo);
        slot6Piscando.Pause();


        foreach (var slot in slotsCampoP1)
        {

            slot.imgElementalCampo.ZeraAlfa();
            slot.imgAnimAtivar.ZeraAlfa();
            slot.imgAnimAtaque.ZeraAlfa();

            if (slot.tipoSlot == TipoCarta.AuxArm)
            {
                slot.canvasGroup.ZeraAlfa();
            }

        }

        foreach (var slot in slotsCampoP2)
        {

            slot.imgElementalCampo.ZeraAlfa();
            slot.imgAnimAtivar.ZeraAlfa();
            slot.imgAnimAtaque.ZeraAlfa();

            if (slot.tipoSlot == TipoCarta.AuxArm)
            {
                slot.canvasGroup.ZeraAlfa();
            }

        }


        zeraAtaques();

        //REMOVER AO TESTAR MULTIPLAYER
        //REMOVER AO TESTAR MULTIPLAYER
        //REMOVER AO TESTAR MULTIPLAYER
        //dropElementalInimigo(Random.Range(0, deckTotal.Count - 1), 0);
        //dropElementalInimigo(Random.Range(0, deckTotal.Count - 1), 1);
        //dropElementalInimigo(Random.Range(0, deckTotal.Count - 1), 2);

    }
    public List<Sprite> cortarSpritesheet(Texture2D spritesheet, int largura, int altura, int qtdFrames)
    {
        List<Sprite> cortados = new List<Sprite>();
        int framesCortados = 0;
        int colunas = (spritesheet.width / largura) - 1;
        int linhas = (spritesheet.height / altura) - 1;

        for (int i = linhas; i >= 0; i--)
        {
            for (int j = 0; j <= colunas; j++)
            {
                if (framesCortados < qtdFrames)
                {
                    //Debug.Log("i:" + i +"/J:" + j);
                    Sprite newSprite = Sprite.Create(spritesheet, new Rect(j * largura, i * altura, largura, altura),
                        new Vector2(0.5f, 0.5f));
                    cortados.Insert(framesCortados, newSprite);

                    framesCortados++;
                }
            }
        }

        return cortados;
    }
    void FaseDeBatalha()
    {

        executarAnimAtaque = true;
        aguardarAnimAtaque = true;
        Timing.RunCoroutine(rodaAtaqueCampo(p1));
    }
    public void sortearDeck(bool rodaAnimacao)
    {
        if (deckPlayer.Count > 0 && !rodandoAnimacao && !movimentandoCarta && turnoLocal)
        {
            int id = UnityEngine.Random.Range(0, deckPlayer.Count - 1);
            passarParaMao(id, rodaAnimacao);
        }
    }
    public void JogarCarta(CartaGeral cartaGeral, int idSlot)
    {

        slotsCampoP2[idSlot].ocupado = true;

        slotsCampoP2[idSlot].imgAnimAtivar.ZeraAlfa();
        slotsCampoP2[idSlot].imgElementalCampo.ZeraAlfa();

        Sequence surgirElemental = DOTween.Sequence().SetId("ativarElementalInimigo");
        surgirElemental.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(slotsCampoP2[idSlot].imgAnimAtivar, cartaGeral.elemento.animCriar,
                false, 6, false, () => { slotsCampoP2[idSlot].imgAnimAtivar.DOFade(0, .3f); });
        });
        surgirElemental.Join(slotsCampoP2[idSlot].imgAnimAtivar.DOFade(1, .5f));
        surgirElemental.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(slotsCampoP2[idSlot].imgElementalCampo, cartaGeral.animCampo, true, 6,
                false, null, "P2" + idSlot.ToString());
        });
        surgirElemental.Join(slotsCampoP2[idSlot].imgElementalCampo.DOFade(1, .5f));

        gerenciadorUI.slotDetalhesP2[idSlot].setarInformações(cartaGeral);
        gerenciadorUI.slotDetalhesP2[idSlot].alteraArco(1);
        slotsCampoP2[idSlot].cartaGeral = cartaGeral;
    }
    public void SetDefensor()
    {
        defensor = turno == TipoJogador.PLAYER ? TipoJogador.IA : TipoJogador.PLAYER;
    }
    public void PassaFaseBatalha()
    {
        if (emBatalha)
        {
            if (defensor == selecionando)
            {
                if (!aguardarAnimAtaque)
                {
                    if (!executarAnimAtaque)
                    {
                        gerenciadorUI.atualizaCronometro(0f);
                        FaseDeBatalha();
                    }
                }
            }
            else
            {
                tempoCronometro = tempoBatalha;
                if (selecionando == TipoJogador.PLAYER)
                {
                    selecionando = TipoJogador.IA;
                    ia.possoJogar = true;
                    gerenciadorUI.DesabilitarBtnPronto();
                    gerenciadorUI.HabilitarAlertaBatalha(true, "Selecione os elementais que deseja atacar!");
                }
                else
                {
                    gerenciadorUI.HabilitarAlertaBatalha(true, "Prepare-se para reagir ao ataque!");
                    gerenciadorUI.HabilitarBtnPronto();
                    selecionando = TipoJogador.PLAYER;
                }
            }
        }
    }
    public void ExecutarAcaoBatalha()
    {
        executarAnimAtaque = true;
    }
    public void EmAnimacao(bool b)
    {
        ExecutandoAnimacao = b;
    }
    #endregion

    #region NaoUtilizado
    /* VERSÃO ANTIGA, COM EFEITO CINEMATOGRAFICO
      public void executaAtaque(int idAtacante, int idInimigo)
    {
        //chma a função ataqueInimigo no segundo jogador
        executaAtaqueLocal(idAtacante, idInimigo, true);
        object[] valoresRede = new object[] { idAtacante, idInimigo, false };
        PhotonNetwork.RaiseEvent(EXECUTA_ATAQUE, valoresRede, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);

    }*/
    #region descartado no modo single
    //private void OnEnable()
    //{
    //    if (tipoPartida == TipoPartida.MULTIPLAYER)
    //    {
    //        base.OnEnable();
    //        PhotonNetwork.NetworkingClient.EventReceived += eventoRecebidoViaRede;
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (tipoPartida == TipoPartida.MULTIPLAYER)
    //    {
    //        base.OnDisable();
    //        PhotonNetwork.NetworkingClient.EventReceived -= eventoRecebidoViaRede;
    //    }
    //}

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    if (tipoPartida == TipoPartida.MULTIPLAYER)
    //    {
    //        base.OnPlayerLeftRoom(otherPlayer);
    //        //se o outro jogador sair chama tela de vitoria
    //        gerenciadorUI.animVitoriaDerrota(true);
    //    }
    //}

    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    if (tipoPartida == TipoPartida.MULTIPLAYER)
    //    {
    //        base.OnDisconnected(cause);
    //        //se minha net caiu chama tela de derrota
    //        if (!gerenciadorUI.panelVitoriaDerrota.gameObject.activeSelf)
    //        {
    //            gerenciadorUI.animVitoriaDerrota(false);
    //        }

    //    }
    //}

    //private void eventoRecebidoViaRede(EventData obj)
    //{
    //    if (obj.Code == DROP_ELEMENTAL_INIMIGO)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        int idCarta = (int)valoresRede[0];
    //        int idSlot = (int)valoresRede[1];

    //        //dropElementalInimigo(idCarta, idSlot);
    //    }

    //    if (obj.Code == EXECUTA_ATAQUE)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        int idAtacante = (int)valoresRede[0];
    //        int idInimigo = (int)valoresRede[1];
    //        bool P1 = (bool)valoresRede[2];
    //        executaAtaqueLocal(idAtacante, idInimigo, P1);
    //    }

    //    if (obj.Code == ATIVAR_ELEMENTAL)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        int idSlotElemental = (int)valoresRede[0];
    //        int idSlotCristal = (int)valoresRede[1];
    //        bool P1 = (bool)valoresRede[2];
    //        //ativarElemental(idSlotElemental, idSlotCristal, P1);
    //    }

    //    if (obj.Code == TROCA_TURNO)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        bool turno = (bool)valoresRede[0];
    //        //trocaTurno(turno);
    //    }

    //    if (obj.Code == EXECUTA_ATAQUE_DIRETO)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        int subVida = (int)valoresRede[0];
    //        bool P1 = (bool)valoresRede[1];
    //        executaAtaqueDiretoLocal(subVida);
    //    }

    //    if (obj.Code == EXECUTA_ATAQUE_CAMPO)
    //    {
    //        object[] valoresRede = (object[])obj.CustomData;
    //        int idAtacante = (int)valoresRede[0];
    //        int idInimigo = (int)valoresRede[1];
    //        //executaAtaque(idAtacante, idInimigo);
    //    }
    //}

    #endregion
    #endregion
}