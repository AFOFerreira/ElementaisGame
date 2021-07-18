using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SlotCampo
{
    public CartaGeral cartaGeral;
    public int idCampo;
    public bool marcado;
    public TipoJogador donoCampo;
    public TipoCarta tipoSlot;

    public Image imgElementalCampo;
    public Image imgAnimAtivar;
    public Image imgAnimAtaque;

    public CanvasGroup canvasGroup;

    public Image mask;
    public Image fotocarta;
    public Image molduraCampo;
    public TextMeshProUGUI titulo;

    public bool ocupado;
    public bool ativado;
    public bool disponivel;

}
