using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[System.Serializable]
public class ColecaoCartas
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }

    [Ignore]
    public Sprite Foto { get; set; }
    [Ignore]
    public Sprite Moldura { get; set; }
    public TipoCarta TipoCarta { get; set; }
}
