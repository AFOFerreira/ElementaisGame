﻿using DG.Tweening;
using funcoesUteis;
using MEC;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for (int i = 0; i < 5; i++)
        {
            int id = Random.Range(0, deckPlayer.Count - 1);
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
            FuncoesUteis.animacaoImagem(slotsCampoP1[idSlot].imgElementalCampo, cartaGeral.animCampo, true, 6, false,
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
                FuncoesUteis.animacaoImagem(slotsCampoP2[idSlot].imgElementalCampo, cartaGeral.animCampo, true, 6,
                    false, null, "P2" + idSlot.ToString());
            });
            surgirElemental.Join(slotsCampoP2[idSlot].imgElementalCampo.DOFade(1, .5f));

            gerenciadorUI.slotDetalhesP2[idSlot].setarInformações(cartaGeral);
            gerenciadorUI.slotDetalhesP2[idSlot].alteraArco(1);
        }
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
                    gerenciadorJogo.gerenciadorAudio.playAtaqueElemental(atacanteUI.cartaGeral.elemento.idElemento);

                    // PARTE 1 ANIMAÇÃO
                    s = DOTween.Sequence();
                    s.Append(gerenciadorJogo.slotsCampoP1[gerenciadorJogo.ataques[i, 0]].imgAnimAtaque.DOFade(0, .5f));
                    s.AppendCallback(() => FuncoesUteis.animacaoImagem(atacanteCampo.imgAnimAtaque,
                        atacanteUI.cartaGeral.elemento.animAtaque1, false, 6));
                    s.Join(atacanteCampo.imgAnimAtaque.DOFade(1, .5f));
                    s.AppendInterval(1.1f);
                    s.Append(atacanteCampo.imgAnimAtaque.DOFade(0, .5f));

                    //----------------------Anim parte 2------------------------
                    if (inimigoCampo.ocupado)
                    {
                        s.Append(inimigoCampo.imgAnimAtaque.DOFade(0, .5f));
                        s.AppendCallback(() => FuncoesUteis.animacaoImagem(inimigoCampo.imgAnimAtaque,
                            atacanteUI.cartaGeral.elemento.animAtaque2, false, 6));
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
    public List<SlotCampo> VerificaCampoDisponivelMagicas(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<SlotCampo>();
        if (tipoJogador == TipoJogador.PLAYER)
        {
            foreach (var obj in slotsCampoP1)
            {
                if (!obj.ocupado == true && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (!obj.ocupado == true && obj.tipoCartaCampo == TipoCarta.MAGICA)
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
                if (obj.ocupado && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (obj.ocupado && obj.tipoCartaCampo == TipoCarta.MAGICA)
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
                if (!obj.ocupado && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (!obj.ocupado && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
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
                if (obj.ocupado && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in slotsCampoP2)
            {
                if (obj.ocupado && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
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
    public void sorteio()
    {
        int valor = Random.Range(0, 100);
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
    IEnumerator<float> iniciarObjetos()
    {
        ElementoGeral fogo = new ElementoGeral(2, "Fogo", Resources.Load<Sprite>("imagens/Elementos/icon_fogo"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_fogo"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraFogo"),
            Resources.LoadAll<Sprite>("imagens/Elementos/fogoAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo2Sprite").ToList());
        ElementoGeral agua = new ElementoGeral(0, "Água", Resources.Load<Sprite>("imagens/Elementos/icon_agua"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_agua"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraAgua"),
            Resources.LoadAll<Sprite>("imagens/Elementos/aguaAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua2Sprite").ToList());
        ElementoGeral terra = new ElementoGeral(1, "Terra", Resources.Load<Sprite>("imagens/Elementos/icon_terra"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_terra"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraTerra"),
            Resources.LoadAll<Sprite>("imagens/Elementos/terraAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra2Sprite").ToList());
        ElementoGeral ar = new ElementoGeral(3, "Ar", Resources.Load<Sprite>("imagens/Elementos/icon_ar"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_ar"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraAr"),
            Resources.LoadAll<Sprite>("imagens/Elementos/arAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar2Sprite").ToList());


        deckTotal.Add(new CartaGeral(0, Resources.Load<Sprite>("imagens/Elementais/01/01-card"),
            Resources.Load<Sprite>("imagens/Elementais/01/01_arco"), "Pandion",
            "Este é o Pandion de fogo uma águia da espécie Imperial Oriental de estágio 1 da classe Guerreiro, seus ataques são rápidos e ferozes.",
            28, 19, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/01/01-sprites").ToList()));
        deckTotal.Add(new CartaGeral(1, Resources.Load<Sprite>("imagens/Elementais/02/02-card"),
            Resources.Load<Sprite>("imagens/Elementais/02/02_arco"), "Pandion",
            "Este é o Pandion de ar uma águia da espécie Harpia de estágio 1 da raça Curandeiro. Suas habilidades são seus chicotes dourados, e claro sua agilidade.",
            19, 22, 1, 1, ar, Resources.LoadAll<Sprite>("imagens/Elementais/02/02-sprites").ToList()));
        deckTotal.Add(new CartaGeral(2, Resources.Load<Sprite>("imagens/Elementais/03/03-card"),
            Resources.Load<Sprite>("imagens/Elementais/03/03_arco"), "Dubhan",
            "Este Elemental é da espécie de Jabutis é muito comum ser encontrado nas matas brasileiras do Nordeste ou Sudeste.",
            24, 56, 1, 2, terra, Resources.LoadAll<Sprite>("imagens/Elementais/03/03-sprites").ToList()));
        deckTotal.Add(new CartaGeral(3, Resources.Load<Sprite>("imagens/Elementais/04/04-card"),
            Resources.Load<Sprite>("imagens/Elementais/04/04_arco"), "Dubhan",
            "Este Dubhan de fogo é da família dos Jabutis, este elemental é um mini vulcão ambulante, seus tentáculos são capazes de prender um predador ou cuspir um forte jato de uma espécie de gás inflamável.",
            59, 44, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/04/04-sprites").ToList()));
        deckTotal.Add(new CartaGeral(4, Resources.Load<Sprite>("imagens/Elementais/05/05-card"),
            Resources.Load<Sprite>("imagens/Elementais/05/05_arco"), "Dubhan",
            "Este Dubhan é o segundo estágio de evolução da espécie de Jabutis de água, com ataque 55 e defesa 37.", 55,
            37, 2, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/05/05-sprites").ToList()));
        deckTotal.Add(new CartaGeral(5, Resources.Load<Sprite>("imagens/Elementais/06/06-card"),
            Resources.Load<Sprite>("imagens/Elementais/06/06_arco"), "Taireth",
            "Taireth é uma mutação dos tubaões brancos, ele vive em zonas tropicais de águas quentes, seu tamanho pode chegar aos 5 metros e a pesar 200Kg.",
            25, 12, 1, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/06/06-sprites").ToList()));
        deckTotal.Add(new CartaGeral(6, Resources.Load<Sprite>("imagens/Elementais/07/07-card"),
            Resources.Load<Sprite>("imagens/Elementais/07/07_arco"), "Taireth",
            "Taireth de terra é da família do Tubarão-tigre, vive em águas tropicais, em suas costas este Elemental carrega uma espécie de coral hipnótico paralisante, que serve para facilitar a caça.",
            17, 30, 1, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/07/07-sprites").ToList()));
        deckTotal.Add(new CartaGeral(7, Resources.Load<Sprite>("imagens/Elementais/08/08-card"),
            Resources.Load<Sprite>("imagens/Elementais/08/08_arco"), "Caedin",
            "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiu controlar muito bem a terra e pedras ao seu redor.",
            21, 36, 1, 1, terra, Resources.LoadAll<Sprite>("imagens/Elementais/08/08-sprites").ToList()));
        deckTotal.Add(new CartaGeral(8, Resources.Load<Sprite>("imagens/Elementais/09/09-card"),
            Resources.Load<Sprite>("imagens/Elementais/09/09_arco"), "Caedin",
            "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiru controlar muito bem a terra e pedras ao seu redor.",
            21, 5, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/09/09-sprites").ToList()));
        deckTotal.Add(new CartaGeral(9, Resources.Load<Sprite>("imagens/Elementais/10/10-card"),
            Resources.Load<Sprite>("imagens/Elementais/10/10_arco"), "Caedin",
            "Caedin de fogo é o segundo estágio da evolução de um cachorro do elemento fogo. Agora, Caedin tem uma calda a mais, seu corpo tem uma inversão de cores e seus olho grande quantia de calor.",
            49, 38, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/10/10-sprites").ToList()));
        deckTotal.Add(new CartaGeral(10, Resources.Load<Sprite>("imagens/Elementais/11/11-card"),
            Resources.Load<Sprite>("imagens/Elementais/11/11_arco"), "Gork",
            "Gorki tem habilidades para controlar as plantas terrestres, ele consegue fazer as flores soltarem venenos, as raízes saírem do chão e fazer com que as gramas cresçam muito alto e rapidamente.",
            19, 31, 1, 1, terra, Resources.LoadAll<Sprite>("imagens/Elementais/11/11-sprites").ToList()));
        deckTotal.Add(new CartaGeral(11, Resources.Load<Sprite>("imagens/Elementais/12/12-card"),
            Resources.Load<Sprite>("imagens/Elementais/12/12_arco"), "Gork",
            "Este Primata do elemento fogo em estágio 1 da raça Guerreiro com ataque 29 e defesa 20, possui poderes vulcânicos.",
            29, 20, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/12/12-sprites").ToList()));
        deckTotal.Add(new CartaGeral(12, Resources.Load<Sprite>("imagens/Elementais/13/13-card"),
            Resources.Load<Sprite>("imagens/Elementais/13/13_arco"), "Gaeron",
            "Gaeron de fogo é está em seu primeiro estágio evolutivo, sua maior habilidade são os disparos de pequenas bolas de chamas. Em sua cabeça e coluna é possível ver alguns ossos saltando de seu corpo.",
            33, 5, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/13/13-sprites").ToList()));
        deckTotal.Add(new CartaGeral(13, Resources.Load<Sprite>("imagens/Elementais/14/14-card"),
            Resources.Load<Sprite>("imagens/Elementais/14/14_arco"), "Gaeron",
            "Este Elemental felino em estágio de evolução 2 possui suas habilidades e amaduras melhoradas e fortificadas.",
            69, 48, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/14/14-sprites").ToList()));
        deckTotal.Add(new CartaGeral(14, Resources.Load<Sprite>("imagens/Elementais/15/15-card"),
            Resources.Load<Sprite>("imagens/Elementais/15/15_arco"), "Gaeron",
            "Evolução de estágio 2 do felino de terra da classe Tanque e possui uma forte defesa decorrente de sua armadura evoluida.",
            38, 57, 1, 2, terra, Resources.LoadAll<Sprite>("imagens/Elementais/15/15-sprites").ToList()));

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

        foreach (var slot in slotsCampoP1)
        {
            slot.imgElementalCampo.ZeraAlfa();
            slot.imgAnimAtivar.ZeraAlfa();
            slot.imgAnimAtaque.ZeraAlfa();
        }

        foreach (var slot in slotsCampoP2)
        {
            slot.imgElementalCampo.ZeraAlfa();
            slot.imgAnimAtivar.ZeraAlfa();
            slot.imgAnimAtaque.ZeraAlfa();
        }
        zeraAtaques();
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
            int id = Random.Range(0, deckPlayer.Count - 1);
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