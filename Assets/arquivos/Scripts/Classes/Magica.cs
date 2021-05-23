using SQLite4Unity3d;

[System.Serializable]
public class Magica :ColecaoCartas
{
    
    public TipoMagica TipoMagica { get; set; }
    public TipoEfeito TipoEfeito { get; set; }
    public int qtdTurno { get; set; }
}
