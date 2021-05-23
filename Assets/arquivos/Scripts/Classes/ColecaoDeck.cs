using SQLite4Unity3d;

[System.Serializable]
public class ColecaoDeck
{
    public string TituloDeck { get; set; }
    public int IdCarta { get; set; }
    public TipoCarta tipoCarta { get; set; }
    public int Quantidade { get; set; }
}

