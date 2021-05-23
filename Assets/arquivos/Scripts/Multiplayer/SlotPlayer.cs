using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SlotPlayer : MonoBehaviour, IDropHandler
{
    public int vida;
    public TextMeshProUGUI txtVida;
    private GerenciadorJogo gerenciadorJogo;
    public bool P1;

    private void Start()
    {
        gerenciadorJogo = GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
    }

    public void alteraVida(int valor)
    {
        vida = valor;
        txtVida.text = valor.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!P1 && eventData.pointerDrag != null &&
            eventData.pointerDrag.GetComponent<SlotElementalDrop>() != null &&
            gerenciadorJogo.turnoLocal &&
            !gerenciadorJogo.rodandoAnimacao &&
            gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot].ativado &&
            gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot].disponivel)
        {
            bool camposVazios = true;

            foreach (var campo in gerenciadorJogo.slotsCampoP2)
            {
                if(campo.ocupado)
                {
                    camposVazios = false;
                }
            }

            if (camposVazios)
            {
                //gerenciadorJogo.executaAtaqueDireto(eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot);
                //gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot, 3);
                gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer,eventData.pointerDrag.GetComponent<SlotElementalDrop>().idSlot, 3);
            }
            else
            {
                gerenciadorJogo.gerenciadorAudio.playNegacao();
            }
        }
            //throw new System.NotImplementedException();
        }



}
