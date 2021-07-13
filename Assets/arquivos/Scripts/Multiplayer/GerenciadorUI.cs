using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using funcoesUteis;
using TMPro;
using System.Collections;

public class GerenciadorUI : MonoBehaviour
{
    public bool uiPronta = false;
    public static GerenciadorUI gerenciadorUI;
    public AudioBase gerenciadorAudio;
    public GerenciadorJogo gerenciadorJogo;
    public List<Sprite> spriteArcos;
    public Sprite fotoPadraoElemental;
    //---------------------------------------------------
    public RectTransform panelTopo;
    public RectTransform panelBaixo;
    //--------------------------------------------------
    public RectTransform panelInfoElementaisP1;
    public List<SlotElemental> slotDetalhesP1;
    //--------------------------------------------------
    public RectTransform panelInfoElementaisP2;
    public List<SlotElemental> slotDetalhesP2;
    //----------------------------------------------------
    public GameObject panelAtaque;
    public Image fundoAtaque;
    public Image player1Ataque;
    public Image player2Ataque;
    public Image imgAnimAtaque;
    public RectTransform barraSuperior;
    public RectTransform barraInferior;
    public RectTransform panelZoom;
    //---------------------------------------------------
    public List<slotCristal> slotsCristais;
    //-----------------------------------------
    public List<Sprite> spritesBtnTurno;
    public Button btnTurno;
    //-------------------------------------
    public GameObject panelAtaqueDireto;
    public Image fundoAtaqueDireto;
    public Image fotoPlayerAtaque;
    public List<Sprite> spritesFotoPlayer;
    //---------------------------------------
    public List<SlotPlayer> slotsPlayer;
    //--------------------------------------
    public Image panelVitoriaDerrota;
    public List<Sprite> animVitoria;
    public List<Sprite> animDerrota;
    //------------------------------------------

    public float tempoCronometro = 15;
    public TextMeshProUGUI txtTempoCronometro;
    public TxtAlertaGame txtAlerta;
    public GameObject btnPronto;
    bool uiM = false;

    [Header("IMG SORTEIO, TURNO")]
    public Sprite P1COMECA;
    public Sprite P2COMECA;
    public Sprite P1TURNO;
    public Sprite P2TURNO;
    public GameObject ImagemAnimacao;
    public GameObject ImagemAnimacaoTurno;
    public GameObject MensagemBatalha;

    private void Awake()
    {
        if (gerenciadorUI == null)
        {
            gerenciadorUI = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        panelTopo.DOAnchorPos(new Vector2(0, 205), 0f);// posicionando o objeto fora da tela
        panelBaixo.DOAnchorPos(new Vector2(0, -200), 0f);// posicionando o objeto fora da tela
                                                         //-----------------------------------------
        /* panelInfoElementaisP1.DOAnchorPos(new Vector2(0, 350), 0f);// posicionando o objeto fora da tela
         slotDetalhesP1[0].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), 0f);// posicionando o objeto fora da tela
         slotDetalhesP1[1].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), 0f);// posicionando o objeto fora da tela
         slotDetalhesP1[2].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), 0f);// posicionando o objeto fora da tela
         //------------------------------------------
         panelAtaque.SetActive(false);
         panelInfoElementaisP2.DOAnchorPos(new Vector2(0, 350), 0f);// posicionando o objeto fora da tela
         slotDetalhesP2[0].panelDetalhes.DOAnchorPos(new Vector2(180, 0), 0f);// posicionando o objeto fora da tela
         slotDetalhesP2[1].panelDetalhes.DOAnchorPos(new Vector2(180, 0), 0f);// posicionando o objeto fora da tela
         slotDetalhesP2[2].panelDetalhes.DOAnchorPos(new Vector2(180, 0), 0f);// posicionando o objeto fora da tela
 */
    }

    void Start()
    {
        gerenciadorJogo = GerenciadorJogo.instance;
        gerenciadorAudio = AudioBase._instance;
        foreach (var slot in slotDetalhesP1)
        {
            slot.alteraArco(0);
        }
        foreach (var slot in slotDetalhesP2)
        {
            slot.alteraArco(0);
        }

        //MostrarUI();

        animDerrota = Resources.LoadAll<Sprite>("imagens/vitoriaDerrota/derrotaSprite").ToList();
        animVitoria = Resources.LoadAll<Sprite>("imagens/vitoriaDerrota/vitoriaSprite").ToList();

    }

    private void MostrarUI()
    {
        Sequence s = DOTween.Sequence();

        s.Append(panelTopo.DOAnchorPos(Vector2.zero, 0.5f));
        s.Append(panelBaixo.DOAnchorPos(Vector2.zero, 0.5f));

        s.Play();
        uiM = true;
    }
    public void MostrarAlerta(string msg)
    {
        txtAlerta.Ativar(msg);
    }
    public void atualizaCronometro(float tempo)
    {
        if (tempo > 0f)
        {
            if (tempo > 20)
            {
                txtTempoCronometro.color = Color.green;

            }
            else if (tempo > 10)
            {
                txtTempoCronometro.color = Color.yellow;
            }
            else
            {
                txtTempoCronometro.color = Color.red;
            }
            txtTempoCronometro.text = tempo.ToString("##");
        }
        else
        {
            txtTempoCronometro.text = "----";
            txtTempoCronometro.color = Color.red;
        }
    }
    public void abreFechaDetalhesElementaisP1()
    {
        /*DOTween.Kill("detalhesP1");
        Sequence s = DOTween.Sequence().SetId("detalhesP1");

        if (panelInfoElementaisP1.offsetMax.y == 0)
        {
            s.Append(slotDetalhesP1[2].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), .3f));// posicionando o objeto fora da tela
            s.Append(slotDetalhesP1[1].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), .3f));// posicionando o objeto fora da tela
            s.Append(slotDetalhesP1[0].panelDetalhes.DOAnchorPos(new Vector2(-180, 0), .3f));// posicionando o objeto fora da tela
            s.Append(panelInfoElementaisP1.DOAnchorPos(new Vector2(0, 350), .5f));// posicionando o objeto fora da tela

        }
        else
        {
            s.Append(panelInfoElementaisP1.DOAnchorPos(Vector2.zero, .5f));
            s.Append(slotDetalhesP1[0].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
            s.Append(slotDetalhesP1[1].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
            s.Append(slotDetalhesP1[2].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
        }


        s.Play();*/
    }
    public void trocaBtnTurno(int idBtn)
    {

        List<int> tiposCristaisCampo = new List<int>();
        List<int> tiposCristaisMao = new List<int>();

        foreach (var item in slotDetalhesP1)
        {
            if (item.cartaGeralTemp != null)
            {
                tiposCristaisCampo.Add(item.cartaGeralTemp.elemento.idElemento);
            }
        }
        foreach (var item in GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().deckMaoPlayer)
        {
            tiposCristaisCampo.Add(item.elemento.idElemento);
        }

        int alvoSorteio = Random.Range(0, 1);
        int sorteado;

        if (alvoSorteio == 0 && tiposCristaisCampo.Count > 0)
        {
            sorteado = tiposCristaisCampo[Random.Range(0, tiposCristaisCampo.Count - 1)];
        }
        else if (alvoSorteio == 1 && tiposCristaisMao.Count > 0)
        {
            sorteado = tiposCristaisCampo[Random.Range(0, tiposCristaisCampo.Count - 1)];
        }
        else
        {
            sorteado = Random.Range(0, 3);
        }

        //0 - Turno oponente
        //1 - Seu turno
        btnTurno.image.sprite = spritesBtnTurno[idBtn];

        if (idBtn == 0)
        {
            btnTurno.enabled = false;
        }
        else
        {
            btnTurno.enabled = true;
            slotsCristais[sorteado].addCristal(1);
        }

    }
    public void abreFechaDetalhesElementaisP2()
    {
        /* DOTween.Kill("detalhesP2");
         Sequence s = DOTween.Sequence().SetId("detalhesP2");

         if (panelInfoElementaisP2.offsetMax.y == 0)
         {
             s.Append(slotDetalhesP2[2].panelDetalhes.DOAnchorPos(new Vector2(180, 0), .3f));// posicionando o objeto fora da tela
             s.Append(slotDetalhesP2[1].panelDetalhes.DOAnchorPos(new Vector2(180, 0), .3f));// posicionando o objeto fora da tela
             s.Append(slotDetalhesP2[0].panelDetalhes.DOAnchorPos(new Vector2(180, 0), .3f));// posicionando o objeto fora da tela
             s.Append(panelInfoElementaisP2.DOAnchorPos(new Vector2(0, 350), .5f));// posicionando o objeto fora da tela

         }
         else
         {
             s.Append(panelInfoElementaisP2.DOAnchorPos(Vector2.zero, .5f));
             s.Append(slotDetalhesP2[0].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
             s.Append(slotDetalhesP2[1].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
             s.Append(slotDetalhesP2[2].panelDetalhes.DOAnchorPos(Vector2.zero, .3f));// posicionando o objeto fora da tela
         }


         s.Play();*/
    }
    public void inicioAtaque(CartaGeral atacante, CartaGeral inimigo, bool P1)
    {

        if (P1)
        {
            imgAnimAtaque.transform.DORotate(new Vector3(0, 0, 0), 0);
        }
        else
        {
            imgAnimAtaque.transform.DORotate(new Vector3(0, 180, 0), 0);
        }

        fundoAtaque.ZeraAlfa();
        player1Ataque.ZeraAlfa();
        player2Ataque.ZeraAlfa();


        player1Ataque.sprite = atacante.animCampo[0];
        player2Ataque.sprite = inimigo.animCampo[0];

        barraSuperior.DOAnchorPos(new Vector2(1470, 0), 0f);// posicionando o objeto fora da tela
        barraInferior.DOAnchorPos(new Vector2(-1470, 0), 0f);// posicionando o objeto fora da tela

        panelAtaque.SetActive(true);
        //iniciar animações personagens
        FuncoesUteis.animacaoImagem(player1Ataque, atacante.animCampo, true, 6);
        FuncoesUteis.animacaoImagem(player2Ataque, inimigo.animCampo, true, 6);

        Sequence s = DOTween.Sequence();
        s.Join(fundoAtaque.DOFade(1f, 1f));//
        s.Join(barraSuperior.DOAnchorPos(Vector2.zero, 1f));// 
        s.Join(barraInferior.DOAnchorPos(Vector2.zero, 1f));// 
        s.Append(panelZoom.DOScale(new Vector3(2, 2, 0), 1f));
        s.Join(panelZoom.DOAnchorPos(new Vector2(0, 68), 1f));
        s.Join(player1Ataque.DOFade(1f, 1f));
        s.Join(player2Ataque.DOFade(1f, 1f));
        s.AppendCallback(() =>
        {
            FuncoesUteis.animacaoImagem(imgAnimAtaque, ((P1) ? atacante.elemento.animAtaque1 : inimigo.elemento.animAtaque1), false, 4, false, fimAtaque);
        });
        s.AppendCallback(() =>
        {
            GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().gerenciadorAudio.playAtaqueElemental((P1) ? atacante.elemento.idElemento : inimigo.elemento.idElemento);
        });
        s.Join(imgAnimAtaque.DOFade(1, 1f));

    }
    public void fimAtaque()
    {
        Sequence s = DOTween.Sequence();
        s.Append(player1Ataque.DOFade(0f, 1f));
        s.Join(imgAnimAtaque.DOFade(0, .1f));
        s.Join(player2Ataque.DOFade(0f, 1f));
        s.Join(barraSuperior.DOAnchorPos(new Vector2(1470, 0), 1f));// 
        s.Join(barraInferior.DOAnchorPos(new Vector2(-1470, 0), 1f));//
        s.Join(panelZoom.DOScale(new Vector3(1, 1, 0), 1f));
        s.Join(panelZoom.DOAnchorPos(Vector2.zero, 1f));
        s.Append(fundoAtaque.DOFade(0f, 1f));// 
        s.AppendCallback(() => { panelAtaque.SetActive(false); });
    }
    public void AtaqueDireto(bool P1, int subVida)
    {
        fotoPlayerAtaque.sprite = spritesFotoPlayer[P1 ? 1 : 0];
        fundoAtaqueDireto.ZeraAlfa();
        fotoPlayerAtaque.ZeraAlfa();
        panelAtaqueDireto.SetActive(true);

        Sequence s = DOTween.Sequence();
        s.Append(fotoPlayerAtaque.DOFade(1f, .4f));//
        s.Join(fundoAtaqueDireto.DOFade(.4f, .4f));
        s.Append(fundoAtaqueDireto.DOFade(.0f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.4f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.0f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.4f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.0f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.4f, .1f));
        s.Append(fundoAtaqueDireto.DOFade(.0f, .1f));
        s.Append(fotoPlayerAtaque.DOFade(0f, .4f));//
        s.AppendCallback(() =>
        {
            panelAtaqueDireto.SetActive(false);
            GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().fimAtaqueDireto(subVida, P1);
        });
    }
    public IEnumerator AnimacaoBandeiraSorteio(bool P1)
    {
        Sequence s = DOTween.Sequence();
        if (P1)
        {
            ImagemAnimacao.GetComponent<Image>().sprite = P1COMECA;
        }
        else
        {
            ImagemAnimacao.GetComponent<Image>().sprite = P2COMECA;
        }
        s.Append(ImagemAnimacao.transform.DOScaleX(1, 0.5f));

        yield return new WaitForSeconds(2f);
        s.Complete();
        gerenciadorJogo.EmJogo = true;
        gerenciadorJogo.panelSorteio.SetActive(false);
        yield return null;
    }
    public IEnumerator AnimacaoTrocarBandeiraTurno(bool P1)
    {
        Sequence s = DOTween.Sequence();
        ImagemAnimacaoTurno.SetActive(true);
        if (P1)
        {
            ImagemAnimacaoTurno.GetComponent<Image>().sprite = P1TURNO;
        }
        else
        {
            ImagemAnimacaoTurno.GetComponent<Image>().sprite = P2TURNO;
        }
        yield return new WaitForSeconds(2f);
        ImagemAnimacaoTurno.SetActive(false);
        yield return null;
    }
    public void animVitoriaDerrota(bool venceu)
    {

        panelVitoriaDerrota.ZeraAlfa();
        panelVitoriaDerrota.gameObject.SetActive(true);

        Sequence s = DOTween.Sequence();
        s.Append(panelVitoriaDerrota.DOFade(1, .5f));
        s.AppendCallback(() =>
        {
            //Main.Instance.StopAllMusics();
            if (venceu)
            {
                FuncoesUteis.animacaoImagemLoopTrecho(panelVitoriaDerrota, animVitoria, 6, 29);
                GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().gerenciadorAudio.stopMusicaGameplay();
                GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().gerenciadorAudio.playMusicaVitoria();
            }
            else
            {
                FuncoesUteis.animacaoImagemLoopTrecho(panelVitoriaDerrota, animDerrota, 6, 32);
                GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().gerenciadorAudio.stopMusicaGameplay();
                GameObject.FindGameObjectWithTag("GerenciadorJogo").GetComponent<GerenciadorJogo>().gerenciadorAudio.playMusicaDerrota();
            }

        });
        s.AppendInterval(3);

    }
    private void Update()
    {
        if (gerenciadorJogo.EmJogo)
        {

            if (!uiM)
                MostrarUI();
            btnTurno.interactable = gerenciadorJogo.emBatalha || gerenciadorJogo.turno != TipoJogador.PLAYER ? false : true;
            
        }
    }

    public void HabilitarBtnPronto()
    {
        btnPronto.SetActive(true);
    }

    public void HabilitarAlertaBatalha(bool b,string msg)
    {
        MensagemBatalha.GetComponent<TextMeshProUGUI>().text = msg;
        MensagemBatalha.SetActive(b);
    }


    public void Pronto()
    {
        gerenciadorJogo.PassaFaseBatalha();
    }

    public void DesabilitarBtnPronto()
    {
        btnPronto.SetActive(false);
    }
}
