using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotElementalDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    public int idSlot;
    private Bezier setaAtaque;
    private Canvas canvas;
    private GerenciadorJogo gerenciadorJogo;

    private void Start()
    {
        setaAtaque = GameObject.FindGameObjectWithTag("SetaAtaque").GetComponent<Bezier>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        gerenciadorJogo = GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROPPPPXXXXXXXXXXXX DROPXXXXXXXXXXXX");
        //SE NÃO ESTIVER OCUPADO E ELE DROPAR ELEMENTAL
        if (eventData.pointerDrag != null && !gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag.GetComponent<CartaPrefab>() != null && gerenciadorJogo.turnoLocal  && !gerenciadorJogo.rodandoAnimacao)
        {
            gerenciadorJogo.dropElemental(eventData.pointerDrag, idSlot);
        }
        //SE TENTAR DROPAR ELEMENTAL COM ELE OCUPADO
        if (eventData.pointerDrag != null && gerenciadorJogo.slotsCampoP1[idSlot].ocupado && eventData.pointerDrag.GetComponent<CartaPrefab>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            gerenciadorJogo.gerenciadorAudio.playNegacao();
        }
        //ATIVAR ELEMENTAL
        if (eventData.pointerDrag != null && !gerenciadorJogo.slotsCampoP1[idSlot].ativado && eventData.pointerDrag.GetComponent<slotCristal>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            //gerenciadorJogo.ativarElemental(idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot, true);
            gerenciadorJogo.photonView.RPC("ativarElemental", Photon.Pun.RpcTarget.AllBufferedViaServer, idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
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
}
