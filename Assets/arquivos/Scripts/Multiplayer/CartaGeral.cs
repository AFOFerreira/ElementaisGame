using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CartaGeral
{
    public int idCarta;
    public Sprite imgCarta;
    public Sprite imgPerfil;
    public string titulo;
    public string descricao;
    public int ataque;
    public int defesa;
    public int cristais;
    public int nivel;
    public ElementoGeral elemento;
    public List<Sprite> animCampo;

    public CartaGeral(int idCarta, Sprite imgCarta, Sprite imgPerfil, string titulo, string descricao, int ataque, int defesa, int cristais, int nivel, ElementoGeral elemento, List<Sprite> animCampo)
    {
        this.idCarta = idCarta;
        this.imgCarta = imgCarta;
        this.imgPerfil = imgPerfil;
        this.titulo = titulo;
        this.descricao = descricao;
        this.ataque = ataque;
        this.defesa = defesa;
        this.cristais = cristais;
        this.nivel = nivel;
        this.elemento = elemento;
        this.animCampo = animCampo;
    }
    public CartaGeral(CartaGeral carta)
    {
        this.idCarta = carta.idCarta;
        this.imgCarta = carta.imgCarta;
        this.imgPerfil = carta.imgPerfil;
        this.titulo = carta.titulo;
        this.descricao = carta.descricao;
        this.ataque = carta.ataque;
        this.defesa = carta.defesa;
        this.cristais = carta.cristais;
        this.nivel = carta.nivel;
        this.elemento = carta.elemento;
        this.animCampo = carta.animCampo;
    }

    public CartaGeral(){}
}
