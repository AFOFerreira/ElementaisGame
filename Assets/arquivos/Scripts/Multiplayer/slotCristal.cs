using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using funcoesUteis;

[System.Serializable]
public class slotCristal : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int idSlot;
    public int qtdCristais;
    public TextMeshProUGUI txtQtdCristais;
    public Image imgAnimCristal;
    public List<Sprite> animCristal;
    private Bezier setaCristais;
    public Color cor;
    private GerenciadorJogo gerenciadorJogo;

    private void Awake()
    {
        imgAnimCristal.ZeraAlfa();
    }

    private void Start()
    {
        setaCristais = GameObject.FindGameObjectWithTag("SetaAtaque").GetComponent<Bezier>();
        gerenciadorJogo = GerenciadorJogo.instance;
    }
    public void addCristal(int qtdInserir)
    {
        
        imgAnimCristal.sprite = animCristal[0];
        Sequence s = DOTween.Sequence();
        s.Join(imgAnimCristal.DOFade(1, .1f));
        s.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(imgAnimCristal, animCristal, false, 6, false, () =>
            {
                txtQtdCristais.text = (qtdCristais += qtdInserir).ToString();
            });
        });
        s.AppendInterval(2.5f);
        s.AppendCallback(() =>
        {
                //AQUI
                gerenciadorJogo.gerenciadorAudio.playCristalElemento(idSlot);
        });
        s.AppendInterval(1f);
        s.Append(imgAnimCristal.DOFade(0, .1f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        if (gerenciadorJogo.turnoLocal && qtdCristais > 0 && !gerenciadorJogo.rodandoAnimacao && gerenciadorJogo.turno == TipoJogador.PLAYER)
        {
            setaCristais.initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            setaCristais.cor = cor;
            setaCristais.setaAtiva = true;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        setaCristais.setaAtiva = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void usarCristais(int qtdDescontar)
    {
        qtdCristais -= qtdDescontar;
        txtQtdCristais.text = qtdCristais.ToString();
    }
}


