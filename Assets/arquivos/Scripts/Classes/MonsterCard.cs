using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class MonsterCard : MonoBehaviour
{
    [Header("Atributos da classe")]
    [SerializeField] int id;
    [SerializeField] string titulo;
    [SerializeField] string descricao;
    [SerializeField] int ataque;
    [SerializeField] int defesa;
    [SerializeField] int nivel;
    [SerializeField] int cristais;
    [SerializeField] Sprite moldura;
    [SerializeField] Sprite foto;

    public ZonaDrop drop;
    [Header("atributos do GameObject")]
    public Text Titulo;
    public Text Descricao;
    public Text Ataque;
    public Text Defesa;
    public Text Nivel;
    public Text Cristais;
    public Image Moldura;
    public Image Foto;
    public Monstro _monstro;
    public Magica _magica;
    public GameObject[] quantidadeImg;
    public TextMeshProUGUI quantidadeTotalText;

    [Header("Painel de confirmacao")]
    public GameObject painelConfirmacao;
    public TextMeshProUGUI txtQuantidade;
    public Button proxima;
    public Button anterior;
    public Button confirmar;
    public Button cancelar;

    [SerializeField] int quantidadeAtual;
    [SerializeField] int quantidadeLimite;
    [SerializeField] int quantidadeTotal;

    Transform posicaoBase;
    Transform posicaoMedia;
    public bool podeAdicionar;
    int quantidadeCartas = 1;
    public Transform PosicaoBase { get => posicaoBase; set => posicaoBase = value; }
    public int Id { get => id; set => id = value; }
    public Transform PosicaoMedia { get => posicaoMedia; set => posicaoMedia = value; }
    public int QuantidadeTotal { get => quantidadeTotal; set => quantidadeTotal = value; }

    public MonsterCard()
    {

    }
    public void SetInformations(Monstro m)
    {
        Id = m.Id;
        titulo = m.Nome;
        descricao = m.Descricao;
        ataque = m.Ataque;
        defesa = m.Defesa;
        nivel = m.Nivel;
        cristais = m.Cristais;
        moldura = m.Moldura;
        foto = m.Foto;
        //------//------///----///-
        _monstro = m;
        Titulo.text = titulo;
        Descricao.text = descricao;
        Ataque.text = ataque.ToString();
        Defesa.text = defesa.ToString();
        Nivel.text = nivel.ToString();
        Cristais.text = cristais.ToString();
        Moldura.sprite = moldura;
        Foto.sprite = foto;

    }
    public void SetInformations(Magica m)
    {
        Id = m.Id;
        titulo = m.Nome;
        descricao = m.Descricao;
        moldura = m.Moldura;
        foto = m.Foto;
        //------//------///----///-
        _magica = m;
        Titulo.text = titulo;
        Descricao.text = descricao;
        Ataque.gameObject.SetActive(false);
        Defesa.gameObject.SetActive(false);
        Nivel.gameObject.SetActive(false);
        Cristais.gameObject.SetActive(false);
        Moldura.sprite = moldura;
        Foto.sprite = foto;
    }


}
