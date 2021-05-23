using System;

[Serializable]
public class ElementoAPI 
{
    public int idElemento { get; set; }
    public string nome { get; set; }
    public string cor { get; set; }
    public string animCriar { get; set; }
    public string animAtaque { get; set; }
    public int animCriarQtdFrames { get; set; }
    public int animCriarTamanhoH { get; set; }
    public int animCriarTamanhoV { get; set; }
    public int animAtaqueQtdFrames { get; set; }
    public int animAtaqueTamanhoH { get; set; }
    public int animAtaqueTamanhoV { get; set; }
    public string moldura { get; set; }
    public string icone { get; set; }
    public string atualizadoEm { get; set; }
}
