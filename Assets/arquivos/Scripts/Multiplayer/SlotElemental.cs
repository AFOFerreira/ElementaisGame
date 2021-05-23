using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[System.Serializable]
public class SlotElemental : MonoBehaviour, IDropHandler
{
    public int idSlot;
    public bool P1;
    //---------------------------------
    public RectTransform panelDetalhes;
    public Image perfilElemental;
    public Image arcoAtivacao;
    public Image brilhoAtivacao;
    //--------------------------------
    public TextMeshProUGUI txtAtaque;
    public TextMeshProUGUI txtDefesa;
    //----------------------------------
    public Image bandeiraElemento;
    //---------------------------------
    public CartaGeral cartaGeral;
    public CartaGeral cartaGeralTemp;
    //----------------------------------------
    GerenciadorJogo gerenciadorJogo;

    private void Awake()
    {

        gerenciadorJogo = GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
    }

    private void Start()
    {
        //ocultarBandeira();
        bandeiraElemento.rectTransform.DOAnchorPos(new Vector2(0, 48), 0f);
    }

    public void exibirBandeira()
    {
        bandeiraElemento.rectTransform.DOAnchorPos(Vector2.zero, .5f);
    }

    public void ocultarBandeira()
    {
        bandeiraElemento.rectTransform.DOAnchorPos(new Vector2(0, 48),.5f);
    }

    public void setarInformações(CartaGeral carta)
    {
        cartaGeral = carta;
        cartaGeralTemp = new CartaGeral(carta);

        txtAtaque.text = carta.ataque.ToString();
        txtDefesa.text = carta.defesa.ToString();

        if (carta.imgPerfil != null)
        {
            perfilElemental.sprite = carta.imgPerfil;
        }
        if (carta.elemento.bandeira != null)
        {
            bandeiraElemento.sprite = carta.elemento.bandeira;
            exibirBandeira();
        }

    }
    public void alteraArco(int idArco)
    {

        arcoAtivacao.sprite = GameObject.FindGameObjectWithTag("GerenciadorUI").GetComponent<GerenciadorUI>().spriteArcos[idArco];

        if(idArco == 2)
        {
            brilhoAtivacao.DOFade(1, .2f);
        }
        else
        {
            brilhoAtivacao.DOFade(0, .2f);
        }
    }

    public void atualizarInformações()
    {

        txtAtaque.text = cartaGeralTemp.ataque.ToString();
        txtDefesa.text = cartaGeralTemp.defesa.ToString();

        if (cartaGeralTemp.imgPerfil != null)
        {
            perfilElemental.sprite = cartaGeralTemp.imgPerfil;
        }
        if (cartaGeralTemp.elemento.bandeira != null)
        {
            bandeiraElemento.sprite = cartaGeralTemp.elemento.bandeira;
        }
    }

    public void morte()
    {
        cartaGeralTemp = null;
        cartaGeral = null;

        txtAtaque.text = "--";
        txtDefesa.text = "--";

        //SUMIR FOTO PERFIL
        perfilElemental.sprite = GameObject.FindGameObjectWithTag("GerenciadorUI").GetComponent<GerenciadorUI>().fotoPadraoElemental;
        //MUDAR ARCO
        arcoAtivacao.sprite = GameObject.FindGameObjectWithTag("GerenciadorUI").GetComponent<GerenciadorUI>().spriteArcos[0];
        //ocultarbandeira
        ocultarBandeira();
        //Tirar Brilho
        brilhoAtivacao.DOFade(0, .2f);

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !gerenciadorJogo.slotsCampoP1[idSlot].ativado && eventData.pointerDrag.GetComponent<slotCristal>() != null && gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            //gerenciadorJogo.ativarElemental(idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot, true);
            gerenciadorJogo.photonView.RPC("ativarElemental", Photon.Pun.RpcTarget.AllBufferedViaServer, idSlot, eventData.pointerDrag.GetComponent<slotCristal>().idSlot);
        }
    }
}
