using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MEC;
using UnityEngine.EventSystems;
using TMPro;
using funcoesUteis;

public class CartaPrefab : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Sprite foto;
    private Sprite iconeElemento;
    private Sprite moldura;
    private string titulo;
    private string descricao;
    private int ataque;
    private int defesa;
    private int cristais;
    private int nivel;

    public Image imgFoto;
    public Image imgIconeElemento;
    public Image imgMoldura;
    public Image imgEfeito;
    public TextMeshProUGUI txtTitulo;
    public TextMeshProUGUI txtCristais;
    public TextMeshProUGUI txtDescricao;
    public TextMeshProUGUI txtAtaque;
    public TextMeshProUGUI txtDefesa;
    public TextMeshProUGUI txtNivel;

    private HorizontalLayoutGroup horizontalLayoutGroup;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private GerenciadorUI gerenciadorUI;
    public GerenciadorJogo gerenciadorJogo;

    public CartaGeral cartaGeral;


    #region Monobehaviour
    private void Awake()
    {
        imgFoto.ZeraAlfa();
        imgIconeElemento.ZeraAlfa();
        imgMoldura.ZeraAlfa();
        imgEfeito.ZeraAlfa();
        txtTitulo.ZeraAlfa();
        txtDescricao.ZeraAlfa();
        txtAtaque.ZeraAlfa();
        txtDefesa.ZeraAlfa();
        txtCristais.ZeraAlfa();
        txtNivel.ZeraAlfa();

        horizontalLayoutGroup = transform.parent.gameObject.GetComponent<HorizontalLayoutGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        gerenciadorUI = GameObject.FindGameObjectWithTag("GerenciadorUI").GetComponent<GerenciadorUI>();
        gerenciadorJogo = GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>();
    }

    private void Start()
    {
        
    }

    #endregion


    #region FuncoesdeAnimacao
    public void aparecerAnimacaoZoom()
    {
        gerenciadorJogo.gerenciadorAudio.playCartaMostrandoouVirando();
        Timing.RunCoroutine(aparecerAnimacaoCorrotina());
    }
    public void aparecerAnimacaoFade()
    {
        Sequence s = DOTween.Sequence();
        s.Append(imgFoto.DOFade(1, 1));
        s.Join(imgIconeElemento.DOFade(1, 1));
        s.Join(imgMoldura.DOFade(1, 1));
        s.Join(txtTitulo.DOFade(1, 1));
        s.Join(txtDescricao.DOFade(1, 1));
        s.Join(txtAtaque.DOFade(1, 1));
        s.Join(txtDefesa.DOFade(1, 1));
        s.Join(txtCristais.DOFade(1, 1));
        s.Join(txtNivel.DOFade(1, 1));
    }

    IEnumerator<float> aparecerAnimacaoCorrotina()
    {
        gerenciadorJogo.rodandoAnimacao = true;
        horizontalLayoutGroup.enabled = true;
        Canvas.ForceUpdateCanvases();
        rectTransform.ForceUpdateRectTransforms();

        yield return Timing.WaitForOneFrame;

        float posX = rectTransform.anchoredPosition.x;
        float posY = rectTransform.anchoredPosition.y;
        
        horizontalLayoutGroup.enabled = false;

        //rectTransform.DOAnchorPos(new Vector2(348, 348), 0);
        rectTransform.anchoredPosition = new Vector2(900, -30);
        rectTransform.DOSizeDelta(new Vector2(36, 46), 0);




        Sequence s = DOTween.Sequence();
        s.Append(imgFoto.DOFade(1, 1));
        s.Join(imgIconeElemento.DOFade(1, 1));
        s.Join(imgMoldura.DOFade(1, 1));
        s.Join(txtTitulo.DOFade(1, 1));
        s.Join(txtDescricao.DOFade(1, 1));
        s.Join(txtAtaque.DOFade(1, 1));
        s.Join(txtDefesa.DOFade(1, 1));
        s.Join(txtCristais.DOFade(1, 1));
        s.Join(txtNivel.DOFade(1, 1));
        s.Join(rectTransform.DOAnchorPos(new Vector2(348, 348), 1));
        s.Join(rectTransform.DOSizeDelta(new Vector2(360, 468), 1));
        s.AppendInterval(.3f);
        s.Append(rectTransform.DOSizeDelta(new Vector2(120, 156), 1));
        s.Join(rectTransform.DOAnchorPos(new Vector2(posX, posY), 1));
        s.AppendCallback(() =>
        {
            horizontalLayoutGroup.enabled = true;
            gerenciadorJogo.rodandoAnimacao = false;
        });
        //
    }

    #endregion

    #region getters&setters
    public void atributos(CartaGeral carta)
    {
        Foto = carta.imgCarta;
        IconeElemento = carta.elemento.iconeElemento;
        Moldura = carta.elemento.moldura;
        Titulo = carta.titulo;
        Descricao = carta.descricao;
        Ataque = carta.ataque;
        Defesa = carta.defesa;
        Cristais = carta.cristais;
        Nivel = carta.nivel;

        cartaGeral = carta;
    }

    public void atributos(Sprite foto, Sprite iconeElemento, Sprite moldura, string titulo, string descricao, int ataque, int defesa, int cristais, int nivel)
    {
        Foto = foto;
        IconeElemento = iconeElemento;
        Moldura = moldura;
        Titulo = titulo;
        Descricao = descricao;
        Ataque = ataque;
        Defesa = defesa;
        Cristais = cristais;
        Nivel = nivel;
    }

    public Sprite Foto
    {
        get => foto; set
        {
            if (value != null)
            {
                foto = value;
                atualizaImgFoto();
            }

        }
    }
    public Sprite IconeElemento
    {
        get => iconeElemento; set
        {
            if (value != null)
            {
                iconeElemento = value;
                atualizaImgIconeElemento();
            }
        }
    }
    public Sprite Moldura
    {
        get => moldura; set
        {
            if (value != null)
            {
                moldura = value;
                atualizaImgMoldura();
            }
        }
    }
    public string Titulo
    {
        get => titulo; set
        {
            titulo = value;
            atualizaTxtTitulo();
        }
    }
    public string Descricao
    {
        get => descricao; set
        {
            descricao = value;
            atualizaTxtDescricao();
        }
    }
    public int Ataque
    {
        get => ataque; set
        {
            ataque = value;
            atualizaTxtAtaque();
        }
    }
    public int Defesa
    {
        get => defesa; set
        {
            defesa = value;
            atualizaTxtDefesa();
        }
    }
    public int Cristais
    {
        get => cristais; set
        {
            cristais = value;
            atualizaTxtCristais();
        }
    }
    public int Nivel
    {
        get => nivel; set
        {
            nivel = value;
            atualizaTxtNivel();
        }
    }

    #endregion


    #region getter&setters Methods
    private void atualizaImgFoto()
    {
        imgFoto.sprite = foto;
    }

    private void atualizaImgIconeElemento()
    {
        imgIconeElemento.sprite = iconeElemento;
    }

    private void atualizaImgMoldura()
    {
        imgMoldura.sprite = moldura;
    }

    private void atualizaTxtTitulo()
    {
        txtTitulo.text = titulo;
    }

    private void atualizaTxtDescricao()
    {
        txtDescricao.text = descricao;
    }

    private void atualizaTxtAtaque()
    {
        txtAtaque.text = ataque.ToString();
    }

    private void atualizaTxtDefesa()
    {
        txtDefesa.text = defesa.ToString();
    }

    private void atualizaTxtCristais()
    {
        txtCristais.text = cristais.ToString();
    }

    private void atualizaTxtNivel()
    {
        txtNivel.text = nivel.ToString();
    }

    #endregion


    #region Drag&Drop
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse down");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gerenciadorJogo.gerenciadorAudio.stopCartanaMao();   
        canvasGroup.blocksRaycasts = true;
        gerenciadorJogo.movimentandoCarta = false;
        gerenciadorJogo.endDragCarta();
        horizontalLayoutGroup.enabled = true;
        imgEfeito.DOFade(0, 1);
        FuncoesUteis.killCorroutines("brilhoCarta");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            gerenciadorJogo.gerenciadorAudio.playCartanaMao();
            canvasGroup.blocksRaycasts = false;
            gerenciadorJogo.movimentandoCarta = true;
            gerenciadorJogo.startDragCarta();
            FuncoesUteis.animacaoImagem(imgEfeito, gerenciadorJogo.brilhoCarta, true, 6, false, null, "brilhoCarta");
            imgEfeito.DOFade(1, 1);
            horizontalLayoutGroup.enabled = false;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gerenciadorJogo.turnoLocal && !gerenciadorJogo.rodandoAnimacao)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("dropCarta");
    }


    #endregion
}
