using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementoGeral
{
    public int idElemento;
    public string nomeElemento;
    public Sprite iconeElemento;
    public Sprite moldura;
    public Sprite bandeira;
    public List<Sprite> animCriar;
    public List<Sprite> animAtaque1;
    public List<Sprite> animAtaque2;

    public ElementoGeral(int idElemento, string nomeElemento, Sprite iconeElemento, Sprite moldura, Sprite bandeira, List<Sprite> animCriar, List<Sprite> animAtaque1, List<Sprite> animAtaque2)
    {
        this.idElemento = idElemento;
        this.nomeElemento = nomeElemento;
        this.iconeElemento = iconeElemento;
        this.moldura = moldura;
        this.bandeira = bandeira;
        this.animCriar = animCriar;
        this.animAtaque1 = animAtaque1;
        this.animAtaque2 = animAtaque2;
    }
}
