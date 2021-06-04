using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerIa : MonoBehaviour
{
    public static GameManagerIa instance;
    public List<CampoSingle> camposIa;
    public List<CampoSingle> camposPlayer;

    public TipoJogador turno;
    public TipoFase faseAtual;
    public int fase;

    public bool EmJogo;
    public bool turnoDefesa;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        EmJogo = true;
        sorteio();
        fase = 1;
        //turno = TipoJogador.IA;
        if (EmJogo)
        {
            foreach (var obj in Transform.FindObjectsOfType<CampoSingle>())
            {
                if (obj.donoCampo == TipoJogador.PLAYER)
                {
                    camposPlayer.Add(obj);
                }
                else
                {
                    camposIa.Add(obj);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EmJogo)
        {
            //Debug.Log("Em Jogo -GameManager-");

        }
    }

    #region controle de campo
    public List<CampoSingle> VerificaCampoDisponivelMagicas(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<CampoSingle>();
        if (tipoJogador == TipoJogador.IA)
        {
            foreach (var obj in camposIa)
            {
                if (obj.vazio == true && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in camposPlayer)
            {
                if (obj.vazio == true && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        return CamposVazios;
    }
    public List<CampoSingle> VerificaCampoOcupadoMagicas(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<CampoSingle>();
        if (tipoJogador == TipoJogador.IA)
        {
            foreach (var obj in camposIa)
            {
                if (obj.vazio == false && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in camposPlayer)
            {
                if (obj.vazio == false && obj.tipoCartaCampo == TipoCarta.MAGICA)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        return CamposVazios;
    }
    public List<CampoSingle> VerificaCampoDisponivelMonstros(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<CampoSingle>();
        if (tipoJogador == TipoJogador.IA)
        {
            foreach (var obj in camposIa)
            {
                if (obj.vazio == true && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in camposPlayer)
            {
                if (obj.vazio == true && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        return CamposVazios;
    }
    public List<CampoSingle> VerificaCampoOcupadoMonstros(TipoJogador tipoJogador)
    {
        var CamposVazios = new List<CampoSingle>();
        if (tipoJogador == TipoJogador.IA)
        {
            foreach (var obj in camposIa)
            {
                if (obj.vazio == false && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        else
        {
            foreach (var obj in camposPlayer)
            {
                if (obj.vazio == false && obj.tipoCartaCampo == TipoCarta.ELEMENTAL)
                {
                    CamposVazios.Add(obj);
                }
            }
        }
        return CamposVazios;
    }
    #endregion

    #region controle turno
    public void PassaTurno()
    {
        if (turno == TipoJogador.IA)
        {
            turno = TipoJogador.PLAYER;
        }
        else
        {
            turno = TipoJogador.IA;
        }
        faseAtual = TipoFase.MONSTRO;
    }
    #endregion

    #region controle sorteio
    public void sorteio()
    {
        int valor = Random.Range(0, 100);
        if (valor >= 0 && valor <= 49)
        {
            turno = TipoJogador.PLAYER;
        }
        else
        {
            turno = TipoJogador.IA;
        }
    }
    #endregion

    #region Controle de jogo
    public void JogarCarta(CampoSingle campo, GameObject carta)
    {
        Instantiate(carta, campo.containerCarta.transform);
    }
    #endregion
}
