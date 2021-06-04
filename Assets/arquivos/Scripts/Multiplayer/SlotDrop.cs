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
    private bool possoJogar;


    private void Start()
    {
        setaAtaque = GameObject.FindGameObjectWithTag("SetaAtaque").GetComponent<Bezier>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        gerenciadorJogo = GerenciadorJogo.instance;
    }

    private void Update()
    {
        if (gerenciadorJogo.turno == gerenciadorJogo.slotsCampoP1[idSlot].donoCampo)
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
        if (possoJogar && gerenciadorJogo.JogadasPlayer >0)
        {
            Debug.Log("DROPPPPXXXXXXXXXXXX DROPXXXXXXXXXXXX");
            //SE NÃO ESTIVER OCUPADO E ELE DROPAR ELEMENTAL
            if (eventData.pointerDrag != null && !gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag.GetComponent<CartaPrefab>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
            {
                gerenciadorJogo.dropElemental(eventData.pointerDrag, idSlot);
                
            }
            //SE TENTAR DROPAR ELEMENTAL COM ELE OCUPADO
            if (eventData.pointerDrag != null && gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag.GetComponent<CartaPrefab>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
            {
                gerenciadorJogo.gerenciadorAudio.playNegacao();
            }
        }else if(possoJogar && gerenciadorJogo.JogadasPlayer ==0)
        {
            Debug.Log("Somente uma carta monstro por turno");
            gerenciadorJogo.gerenciadorAudio.playNegacao();
        }
        else
        {
            Debug.Log("Não posso jogar, não é minha vez");
            gerenciadorJogo.gerenciadorAudio.playNegacao();

        }
        ////ATIVAR ELEMENTAL
        if (eventData.pointerDrag != null && !gerenciadorJogo.slotsCampoP1[idSlot].ativado && eventData.pointerDrag.GetComponent<slotCristal>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            gerenciadorJogo.ativarElemental(idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
            //gerenciadorJogo.photonView.RPC("ativarElemental", Photon.Pun.RpcTarget.AllBufferedViaServer, idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
        }

        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<SlotDrop>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            if (gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].ativado
                && gerenciadorJogo.slotsCampoP2[idSlot].ocupado &&
                gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel)
            {
                gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot);
                //gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer, eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot);
                gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel = false;
            }
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gerenciadorJogo.slotsCampoP1[idSlot].ocupado && gerenciadorJogo.slotsCampoP1[idSlot].ativado
            && gerenciadorJogo.turnoLocal &&
            !gerenciadorJogo.rodandoAnimacao &&
            gerenciadorJogo.slotsCampoP1[idSlot].disponivel)
        {
            Debug.Log("pointer down campo");
            //setaAtaque.initialPoint = Camera.main.ScreenToWorldPoint(GetComponent<RectTransform>().position/canvas.scaleFactor);
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
