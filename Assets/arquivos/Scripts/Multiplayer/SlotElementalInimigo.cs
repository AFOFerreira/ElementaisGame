﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotElementalInimigo : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int idSlot;
    public GerenciadorJogo gerenciadorJogo;
    public Bezier setaAtaque;

    private void Start()
    {
        gerenciadorJogo =GerenciadorJogo.instance;
        setaAtaque = GameObject.FindGameObjectWithTag("SetaAtaque").GetComponent<Bezier>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<SlotDrop>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            if (gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].ativado
                && gerenciadorJogo.slotsCampoP2[idSlot].ocupado && 
                gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel)
            {
                //gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot, idSlot);
                gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer, eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, idSlot);
                gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot].disponivel = false;
            }
        }
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
