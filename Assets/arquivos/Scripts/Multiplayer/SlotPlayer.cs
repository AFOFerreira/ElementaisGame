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
    GerenciadorUI gerenciadorUI = new GerenciadorUI();

    private void Start()
    {
        gerenciadorJogo = GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
        gerenciadorUI = GerenciadorUI.gerenciadorUI;
    }

    public void alteraVida(int valor)
    {
        vida = valor;
        txtVida.text = valor.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {

        if (!P1 && eventData.pointerDrag.GetComponent<SlotDrop>() != null 
            && !gerenciadorJogo.rodandoAnimacao && gerenciadorJogo.slotsCampoP1[eventData.pointerDrag.GetComponent<SlotDrop>().idSlot] 
            != gerenciadorJogo.ultimoCampoAtivado)
        {
            bool camposVazios = true;

            foreach (var campo in gerenciadorJogo.slotsCampoP2)
            {
                if (campo.ocupado)
                {
                    camposVazios = false;
                }
            }

            if (camposVazios)
            {
                //gerenciadorJogo.executaAtaqueLocal(eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, false);
                gerenciadorJogo.executaAtaque(eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, 3, false);
                //gerenciadorJogo.photonView.RPC("executaAtaque", Photon.Pun.RpcTarget.AllBufferedViaServer,eventData.pointerDrag.GetComponent<SlotDrop>().idSlot, 3);
            }
            else
            {
                gerenciadorUI.MostrarAlerta("Ainda há elementais que precisam ser destruidos!");
                gerenciadorJogo.gerenciadorAudio.playNegacao();
            }
        }
        else
        {
            gerenciadorJogo.gerenciadorAudio.playNegacao();
            gerenciadorUI.MostrarAlerta("Você precisa esperar o proximo turno para atacar com este elemental!");
        }

    }
}
