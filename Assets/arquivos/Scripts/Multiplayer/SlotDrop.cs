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

    private void Start()
    {
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
    }

    private void VerificaDono()
    {
        if (gerenciadorJogo.turno == donoCampo)
        {
            possoJogar = true;
        }
        else
        {
            possoJogar = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (possoJogar)
        {

            if (!gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CartaPrefab>() != null)
            {
                if (gerenciadorJogo.JogadasPlayer > 0)
                {
                    Debug.Log("DROPPPPXXXXXXXXXXXX DROPXXXXXXXXXXXX");
                    //SE NÃO ESTIVER OCUPADO E ELE DROPAR ELEMENTAL
                    if (!gerenciadorJogo.rodandoAnimacao)
                    {
                        gerenciadorJogo.dropElemental(eventData.pointerDrag, idSlot);

                    }
                    //SE TENTAR DROPAR ELEMENTAL COM ELE OCUPADO
                }
                else
                {
                    gerenciadorUI.MostrarAlerta("Somente uma carta monstro por turno!");
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
                                gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot, true);
                                //gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer, eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot);
                                gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel = false;
                                gerenciadorJogo.setTurnoBatalha();
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
        else
        {
            gerenciadorUI.MostrarAlerta("Não é sua vez!");
            gerenciadorJogo.gerenciadorAudio.playNegacao();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gerenciadorJogo.slotsCampoP1[idSlot].ocupado && gerenciadorJogo.slotsCampoP1[idSlot].ativado
             && possoJogar &&
            !gerenciadorJogo.rodandoAnimacao && gerenciadorJogo.slotsCampoP1[idSlot].disponivel
            )
        {
            arrastando = true;
            Debug.Log("pointer down campo");
            //setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(GetComponent<RectTransform>().position / canvas.scaleFactor);
            setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            setaAtaque.slotInicial = idSlot;
            setaAtaque.setaAtiva = true;
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