using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartaElemental {
    private string fotoPerfil;
    private string imagemCarta;
    private string animCampo;

    //--------------------------------------------
    private Sprite imgFotoPerfil;
    private Sprite imgImagemcarta;
    private Sprite imgAnimCampo;
    //---------------------------------------------

    private string titulo;
    private string descricao;
    private string raridade;


    private int idCarta;
    private int qtdFrames;
    private int tamanhoH;
    private int tamanhoV;
    private int idElemento;
    private int ataque;
    private int defesa;
    private int nivel;
    private int cristais;

    public string FotoPerfil { get => fotoPerfil; set => fotoPerfil = value; }
    public string ImagemCarta { get => imagemCarta; set => imagemCarta = value; }
    public string AnimCampo { get => animCampo; set => animCampo = value; }
    public Sprite ImgFotoPerfil { get => imgFotoPerfil; set => imgFotoPerfil = value; }
    public Sprite ImgImagemcarta { get => imgImagemcarta; set => imgImagemcarta = value; }
    public Sprite ImgAnimCampo { get => imgAnimCampo; set => imgAnimCampo = value; }
    public string Titulo { get => titulo; set => titulo = value; }
    public string Descricao { get => descricao; set => descricao = value; }
    public string Raridade { get => raridade; set => raridade = value; }
    public int IdCarta { get => idCarta; set => idCarta = value; }
    public int QtdFrames { get => qtdFrames; set => qtdFrames = value; }
    public int TamanhoH { get => tamanhoH; set => tamanhoH = value; }
    public int TamanhoV { get => tamanhoV; set => tamanhoV = value; }
    public int IdElemento { get => idElemento; set => idElemento = value; }
    public int Ataque { get => ataque; set => ataque = value; }
    public int Defesa { get => defesa; set => defesa = value; }
    public int Nivel { get => nivel; set => nivel = value; }
    public int Cristais { get => cristais; set => cristais = value; }

    public CartaElemental(string fotoPerfil, string imagemCarta, string animCampo, Sprite imgFotoPerfil, Sprite imgImagemcarta, Sprite imgAnimCampo, string titulo, string descricao, string raridade, int idCarta, int qtdFrames, int tamanhoH, int tamanhoV, int idElemento, int ataque, int defesa, int nivel, int cristais)
    {
        ImgFotoPerfil = imgFotoPerfil;
        ImgImagemcarta = imgImagemcarta;
        ImgAnimCampo = imgAnimCampo;
        Titulo = titulo;
        Descricao = descricao;
        Raridade = raridade;
        IdCarta = idCarta;
        QtdFrames = qtdFrames;
        TamanhoH = tamanhoH;
        TamanhoV = tamanhoV;
        IdElemento = idElemento;
        Ataque = ataque;
        Defesa = defesa;
        Nivel = nivel;
        Cristais = cristais;

        //chamar pegar imagem no drive e jogar nas variaveis
        FotoPerfil = fotoPerfil;
        ImagemCarta = imagemCarta;
        AnimCampo = animCampo;
    }
}
