using UnityEngine;

[System.Serializable]
public class Monstro : ColecaoCartas
{

    public int Ataque { get; set; }
    public int Defesa { get; set; }
    public int Cristais { get; set; }
    public int Nivel { get; set; }
    public int id_elemento { get; set; }
    public int id_carta { get; set; }
    public TipoElemental TipoElemental { get; set; }

    public Monstro() { }

    public Monstro(int ataque, int defesa, int cristais, int nivel, int id_elemento, int id_carta, TipoElemental tipoElemental)

    {

        Ataque = ataque;
        Defesa = defesa;
        Cristais = cristais;
        Nivel = nivel;
        this.id_elemento = id_elemento;
        this.id_carta = id_carta;
        TipoElemental = tipoElemental;
    }
}
