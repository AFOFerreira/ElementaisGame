using funcoesUteis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public int idSlot;

    private Bezier setaAtaque;
    private Canvas canvas;
    private GerenciadorJogo gerenciadorJogo;
    public GerenciadorUI gerenciadorUI;
    public bool possoJogar;
    public TipoJogador donoCampo;
    bool arrastando;
    public bool marcado;
    public Sprite ImgNaoAtivado;
    Sprite tempImg;
    Sprite tempMoldura;

    private void Start()
    {
        marcado = false;
        setaAtaque = GameObject.FindGameObjectWithTag("SetaAtaque").GetComponent<Bezier>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        gerenciadorUI = GerenciadorUI.gerenciadorUI;
        gerenciadorJogo = GerenciadorJogo.instance;
    }

    private void Update()
    {
        VerificaDono();
        if (donoCampo == TipoJogador.PLAYER)
        {
            if (gerenciadorJogo.ultimoCampoAtivado != gerenciadorJogo.slotsCampoP1[idSlot])
            {
                gerenciadorJogo.slotsCampoP1[idSlot].disponivel = true;
            }
        }
        else
        {
            if (gerenciadorJogo.slotsCampoP2[idSlot].tipoSlot != TipoCarta.Elemental && gerenciadorJogo.slotsCampoP2[idSlot].ocupado && !gerenciadorJogo.slotsCampoP2[idSlot].ativado)
            {
                tempImg = gerenciadorJogo.slotsCampoP2[idSlot].cartaGeral.imgCarta;
                tempMoldura = gerenciadorJogo.slotsCampoP2[idSlot].cartaGeral.auxArm.molduraCampo;
                gerenciadorJogo.slotsCampoP2[idSlot].molduraCampo.sprite = ImgNaoAtivado;
                gerenciadorJogo.slotsCampoP2[idSlot].fotocarta.sprite = null;
                gerenciadorJogo.slotsCampoP2[idSlot].fotocarta.color = Color.black;
            }
            else
            {
                gerenciadorJogo.slotsCampoP2[idSlot].molduraCampo.sprite = tempMoldura;
                gerenciadorJogo.slotsCampoP2[idSlot].fotocarta.sprite = tempImg;
                gerenciadorJogo.slotsCampoP2[idSlot].fotocarta.color = Color.white;
            }
        }
    }

    private void VerificaDono()
    {
        if (gerenciadorJogo.turno == donoCampo && !gerenciadorJogo.emBatalha)
        {
            possoJogar = true;
        }
        else if (gerenciadorJogo.defensor == donoCampo && gerenciadorJogo.emBatalha)
        {
            possoJogar = true;
        }
        else
        {
            marcado = false;
            possoJogar = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (possoJogar)
        {
            if (!gerenciadorJogo.emBatalha && gerenciadorJogo.turno == donoCampo)
            {

                if (!gerenciadorJogo.slotsCampoP1[idSlot].ocupado
                    && eventData.pointerDrag != null
                    && eventData.pointerDrag.GetComponent<CartaPrefab>() != null
                    && gerenciadorJogo.slotsCampoP1[idSlot].tipoSlot == TipoCarta.Elemental
                && eventData.pointerDrag.GetComponent<CartaPrefab>().cartaGeral.tipoCarta == TipoCarta.Elemental)
                {
                    if (gerenciadorJogo.JogadasPlayer > 0)
                    {
                        Debug.Log("DROPPPPXXXXXXXXXXXX DROPXXXXXXXXXXXX");
                        //SE NÃO ESTIVER OCUPADO E ELE DROPAR ELEMENTAL
                        if (!gerenciadorJogo.rodandoAnimacao)
                        {
                            gerenciadorJogo.dropElemental(eventData.pointerDrag, idSlot);

                        }

                    }
                    else
                    {
                        gerenciadorUI.MostrarAlerta("Somente uma carta monstro por turno!");
                        gerenciadorJogo.gerenciadorAudio.playNegacao();
                    }
                }
                else if (eventData.pointerDrag != null &&
                              !gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag.GetComponent<CartaPrefab>() != null
                     && eventData.pointerDrag.GetComponent<CartaPrefab>().cartaGeral.tipoCarta != TipoCarta.Elemental
                     && gerenciadorJogo.slotsCampoP1[idSlot].tipoSlot == TipoCarta.AuxArm)
                {
                    if (!gerenciadorJogo.rodandoAnimacao)
                    {
                        Debug.Log("Teste aqui!" + eventData.pointerDrag.GetComponent<CartaPrefab>().cartaGeral.verificaAcoesBool(gerenciadorJogo)+", "+
                            eventData.pointerDrag.GetComponent<CartaPrefab>().cartaGeral.titulo);
                        //if (eventData.pointerDrag.GetComponent<CartaPrefab>().cartaGeral.verificaAcoesBool(gerenciadorJogo))
                        {
                            gerenciadorJogo.dropAuxArm(eventData.pointerDrag, idSlot);
                        }
                        ////else
                        //{
                        //    gerenciadorUI.MostrarAlerta("Nao há elementais para essa carta ser usada!");
                        //    gerenciadorJogo.gerenciadorAudio.playNegacao();
                        //}
                    }
                    else
                    {
                        gerenciadorUI.MostrarAlerta("Não é possivel baixar essa carta!");
                        gerenciadorJogo.gerenciadorAudio.playNegacao();
                    }
                }
                else
                {

                    if (eventData.pointerDrag.GetComponent<CartaPrefab>() == null)
                    {
                        if (!gerenciadorJogo.slotsCampoP1[idSlot].ativado && eventData.pointerDrag.GetComponent<slotCristal>() != null && !gerenciadorJogo.rodandoAnimacao)
                        {
                            gerenciadorJogo.ativarElemental(idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
                            //gerenciadorJogo.photonView.RPC("ativarElemental", Photon.Pun.RpcTarget.AllBufferedViaServer, idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
                        }
                        else
                        {

                            if (eventData.pointerDrag.GetComponent<SlotDrop>() != null && !gerenciadorJogo.rodandoAnimacao && gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot] != gerenciadorJogo.ultimoCampoAtivado)
                            {

                                if (gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].ativado
                                    && gerenciadorJogo.slotsCampoP2[idSlot].ocupado &&
                                    gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel)
                                {
                                    if (gerenciadorJogo.VerificaCampoMarcado(this))
                                    {
                                        gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot, true);
                                        //gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer, eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot);
                                        gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel = false;
                                        gerenciadorJogo.setTurnoBatalha();
                                        gerenciadorJogo.AdicionarCampoMarcado(this);
                                        marcado = true;
                                    }
                                    else
                                    {
                                        gerenciadorJogo.gerenciadorAudio.playNegacao();
                                        gerenciadorUI.MostrarAlerta("Esse campo já está marcado para ataque!");
                                    }
                                }
                                else
                                {
                                    gerenciadorJogo.gerenciadorAudio.playNegacao();
                                    gerenciadorUI.MostrarAlerta("Não é possivel atacar esse campo!");
                                }
                            }
                            else
                            {
                                gerenciadorJogo.gerenciadorAudio.playNegacao();
                                gerenciadorUI.MostrarAlerta("Você precisa esperar o proximo turno para atacar com este elemental!");
                            }
                        }
                    }
                    else
                    {
                        gerenciadorUI.MostrarAlerta("Este campo já esta ocupado!");
                        gerenciadorJogo.gerenciadorAudio.playNegacao();
                    }

                }

            }
            else if (gerenciadorJogo.emBatalha && gerenciadorJogo.defensor == donoCampo)
            {
                Debug.Log("executar acao magicas");
            }

        }
        else
        {
            gerenciadorUI.MostrarAlerta("Não é sua vez!");
            gerenciadorJogo.gerenciadorAudio.playNegacao();
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (possoJogar)
        {
            if (gerenciadorJogo.slotsCampoP1[idSlot].ocupado && gerenciadorJogo.slotsCampoP1[idSlot].ativado
                &&
                !gerenciadorJogo.rodandoAnimacao && gerenciadorJogo.slotsCampoP1[idSlot].disponivel &&
                gerenciadorJogo.slotsCampoP1[idSlot].cartaGeral.tipoCarta == TipoCarta.Elemental
                )
            {
                arrastando = true;
                Debug.Log("pointer down campo");
                //setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(GetComponent<RectTransform>().position / canvas.scaleFactor);
                setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                setaAtaque.slotInicial = idSlot;
                setaAtaque.setaAtiva = true;
            }
            else if (gerenciadorJogo.defensor == donoCampo && gerenciadorJogo.slotsCampoP1[idSlot].cartaGeral.efeitoAo == EfeitoAo.Selecionar)
            {
                arrastando = true;
                Debug.Log("pointer down campo magica");
                //setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(GetComponent<RectTransform>().position / canvas.scaleFactor);
                setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                setaAtaque.slotInicial = idSlot;
                setaAtaque.setaAtiva = true;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        setaAtaque.setaAtiva = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        setaAtaque.slotSobre = idSlot;
        //Debug.Log("Mouse dentro ->" + idSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        setaAtaque.slotSobre = -1;
        //Debug.Log("Mouse fora ->" + idSlot);
    }
}