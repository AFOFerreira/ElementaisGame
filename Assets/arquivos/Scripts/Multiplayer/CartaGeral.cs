using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class CartaGeral
{
    //-------------------Gerais
    public int idCarta;
    public TipoCarta tipoCarta;
    public Sprite imgCarta;
    public string titulo;
    public string descricao;
    public List<Sprite> animCampo;

    //-------------------Elementais
    public Sprite imgPerfil;
    public int ataque;
    public int defesa;
    public int cristais;
    public int nivel;
    public ElementoGeral elemento;

    //-------------------AuxArm
    public AuxArmGeral auxArm;
    public string localAnim;
    public List<Acoes> listAcoes;

    //-------------------Default constructor
    public CartaGeral()
    {

    }


    //=========ELEMENTAL
    public CartaGeral(TipoCarta tipoCarta, int idCarta, Sprite imgCarta, string titulo, string descricao, List<Sprite> animCampo,
        Sprite imgPerfil, int ataque, int defesa, int cristais, int nivel, ElementoGeral elemento)
    {
        this.tipoCarta = tipoCarta;
        this.idCarta = idCarta;
        this.imgCarta = imgCarta;
        this.titulo = titulo;
        this.descricao = descricao;
        this.animCampo = animCampo;

        this.imgPerfil = imgPerfil;
        this.ataque = ataque;
        this.defesa = defesa;
        this.cristais = cristais;
        this.nivel = nivel;
        this.elemento = elemento;

    }

    //=========AUX ARM
    public CartaGeral(TipoCarta tipoCarta, int idCarta, Sprite imgCarta, string titulo, string descricao, List<Sprite> animCampo,
        AuxArmGeral auxArm, string localAnim, List<Acoes> listAcoes)
    {
        this.tipoCarta = tipoCarta;
        this.idCarta = idCarta;
        this.imgCarta = imgCarta;
        this.titulo = titulo;
        this.descricao = descricao;
        this.animCampo = animCampo;

        this.auxArm = auxArm;
        this.localAnim = localAnim;
        this.listAcoes = listAcoes;

    }

    public CartaGeral(CartaGeral carta)
    {
        this.tipoCarta = carta.tipoCarta;
        this.idCarta = carta.idCarta;
        this.imgCarta = carta.imgCarta;
        this.titulo = carta.titulo;
        this.descricao = carta.descricao;
        this.animCampo = carta.animCampo;

        if (carta.tipoCarta == TipoCarta.Elemental)
        {
            this.imgPerfil = carta.imgPerfil;
            this.ataque = carta.ataque;
            this.defesa = carta.defesa;
            this.cristais = carta.cristais;
            this.nivel = carta.nivel;
            this.elemento = carta.elemento;
        }
        if ((carta.tipoCarta == TipoCarta.Auxiliar) || (carta.tipoCarta == TipoCarta.Auxiliar))
        {
            this.auxArm = carta.auxArm;
            this.localAnim = carta.localAnim;
            this.listAcoes = carta.listAcoes;
        }
    }
}
