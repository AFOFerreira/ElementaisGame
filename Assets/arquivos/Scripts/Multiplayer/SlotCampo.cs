using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SlotCampo
{
    public CartaGeral cartaGeral;
    public int idCampo;
    public Image imgElementalCampo;
    public Image imgAnimAtivar;
    public Image imgAnimAtaque;
    public bool ocupado;
    public bool ativado;
    public bool disponivel;
    public bool marcado;
    public TipoJogador donoCampo;
    public TipoCarta tipoCartaCampo;

}
