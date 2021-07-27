using DG.Tweening;
using funcoesUteis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class CartaGeral
{
    //-------------------Gerais
    public int idCarta;
    public TipoCarta tipoCarta;
    public Sprite imgCarta;
    public string titulo;
    public string descricao;
    public List<Sprite> animCampo;

    //-------------------Elementais
    public Sprite imgPerfil;
    public int ataque;
    public int defesa;
    public int cristais;
    public int nivel;
    public ElementoGeral elemento;

    //-------------------AuxArm
    public AuxArmGeral auxArm;
    public LocalAnim localAnim;
    public List<Acoes> listAcoes;
    public TipoEfeito tipoEfeito;
    public EfeitoAo efeitoAo;
    public int qtdTurnos;

    //-------------------Default constructor
    public CartaGeral()
    {

    }


    //=========ELEMENTAL
    public CartaGeral(TipoCarta tipoCarta, int idCarta, Sprite imgCarta, string titulo, string descricao, List<Sprite> animCampo,
        Sprite imgPerfil, int ataque, int defesa, int cristais, int nivel, ElementoGeral elemento)
    {
        this.tipoCarta = tipoCarta;
        this.idCarta = idCarta;
        this.imgCarta = imgCarta;
        this.titulo = titulo;
        this.descricao = descricao;
        this.animCampo = animCampo;

        this.imgPerfil = imgPerfil;
        this.ataque = ataque;
        this.defesa = defesa;
        this.cristais = cristais;
        this.nivel = nivel;
        this.elemento = elemento;

    }

    //=========AUX ARM
    public CartaGeral(TipoCarta tipoCarta, int idCarta, Sprite imgCarta, string titulo, string descricao, List<Sprite> animCampo,
        AuxArmGeral auxArm, LocalAnim localAnim, EfeitoAo efeitoAo,int qtdTurno,TipoEfeito tipoEfeito, List<Acoes> listAcoes)
    {
        this.tipoCarta = tipoCarta;
        this.idCarta = idCarta;
        this.imgCarta = imgCarta;
        this.titulo = titulo;
        this.descricao = descricao;
        this.animCampo = animCampo;
        this.qtdTurnos = qtdTurno;
        this.auxArm = auxArm;
        this.localAnim = localAnim;
        this.efeitoAo = efeitoAo;
        this.listAcoes = listAcoes;
        this.tipoEfeito = tipoEfeito;

    }

    public CartaGeral(CartaGeral carta)
    {
        this.tipoCarta = carta.tipoCarta;
        this.idCarta = carta.idCarta;
        this.imgCarta = carta.imgCarta;
        this.titulo = carta.titulo;
        this.descricao = carta.descricao;
        this.animCampo = carta.animCampo;

        if (carta.tipoCarta == TipoCarta.Elemental)
        {
            this.imgPerfil = carta.imgPerfil;
            this.ataque = carta.ataque;
            this.defesa = carta.defesa;
            this.cristais = carta.cristais;
            this.nivel = carta.nivel;
            this.elemento = carta.elemento;
        }
        if ((carta.tipoCarta == TipoCarta.Auxiliar) || (carta.tipoCarta == TipoCarta.Auxiliar))
        {
            this.auxArm = carta.auxArm;
            this.localAnim = carta.localAnim;
            this.listAcoes = carta.listAcoes;
            this.efeitoAo = carta.efeitoAo;
            this.tipoEfeito = carta.tipoEfeito;
            this.qtdTurnos = carta.qtdTurnos;
        }
    }

    public void executaAcoes(GerenciadorJogo gerenciadorJogo, List<int> idSlot = null, bool player = true)
    {
        List<IdSlotsExecutado> idSlotsExecutado = new List<IdSlotsExecutado>();
        List<IdSlotsExecutado> idSlotsExecutadoTemp = new List<IdSlotsExecutado>();
        foreach (Acoes acaoAtual in listAcoes)
        {
            if (acaoAtual.acao != AcoesDisponiveis.nada)
            {
                acaoAtual.parametros[0] = gerenciadorJogo;

                if (idSlot != null)
                {
                    acaoAtual.parametros[1] = idSlot;
                }

                //acaoAtual.parametros[2] = player;
                player = (bool)acaoAtual.parametros[2];
                //acaoAtual.parametros.Add(false); 
                acaoAtual.parametros[acaoAtual.parametros.Count - 1] = false;// Parametro testando, se True está testando, False irá executar a ação
                Debug.Log("EXECUTA PARAMETROS: " + acaoAtual.parametros.Count);
                if (efeitoAo == EfeitoAo.Selecionar)
                {
                    if (acaoAtual.parametros[1] != null)
                    {
                        idSlotsExecutadoTemp = (List<IdSlotsExecutado>)typeof(listaAcoesClass).GetMethod(((AcoesDisponiveis)acaoAtual.acao).ToString()).
                                        Invoke(this, acaoAtual.parametros.ToArray());
                    }

                }
                else
                {
                    idSlotsExecutadoTemp = (List<IdSlotsExecutado>)typeof(listaAcoesClass).GetMethod(((AcoesDisponiveis)acaoAtual.acao).ToString()).
                                        Invoke(this, acaoAtual.parametros.ToArray());
                }
                if (idSlotsExecutadoTemp != null)
                {
                    idSlotsExecutado.AddRange(idSlotsExecutadoTemp);
                }

            }
        }
        //executar animação
        if (localAnim == LocalAnim.CampoCartas)
        {
            if (idSlotsExecutado != null)
            {
                foreach (IdSlotsExecutado item in idSlotsExecutado.Distinct())
                {
                    List<SlotCampo> slotBusca = ((item.player) ? gerenciadorJogo.slotsCampoP1 : gerenciadorJogo.slotsCampoP2);

                    slotBusca[item.idSlot].imgAnimAtivar.DOFade(1, .3f);
                    FuncoesUteis.animacaoImagem(slotBusca[item.idSlot].imgAnimAtivar, animCampo, false, 6, false, () =>
                    {
                        slotBusca[item.idSlot].imgAnimAtivar.DOFade(0, .3f);
                    });
                }
            }
        }
        else if (localAnim == LocalAnim.CampoMeio)
        {
            gerenciadorJogo.animAuxArm.DOFade(1, .3f);
            FuncoesUteis.animacaoImagem(gerenciadorJogo.animAuxArm, animCampo, false, 6, false, () =>
            {
                gerenciadorJogo.animAuxArm.DOFade(0, .3f);
            });
        }
    }

    public List<IdSlotsExecutado> verificaAcoesList(GerenciadorJogo gerenciadorJogo, List<int> idSlot = null, bool player = true)
    {
        List<IdSlotsExecutado> idSlotsExecutado = new List<IdSlotsExecutado>();
        List<IdSlotsExecutado> idSlotsExecutadoTemp = new List<IdSlotsExecutado>();
        foreach (Acoes acaoAtual in listAcoes)
        {
            if (acaoAtual.acao != AcoesDisponiveis.nada)
            {
                acaoAtual.parametros[0] = gerenciadorJogo;
                acaoAtual.parametros[1] = idSlot;
                //acaoAtual.parametros[2] = player;
                //acaoAtual.parametros.Add(true);// Parametro testando, se True está testando, False irá executar a ação
                acaoAtual.parametros[acaoAtual.parametros.Count - 1] = true;// Parametro testando, se True está testando, False irá executar a ação
                player = (bool)acaoAtual.parametros[2];

                idSlotsExecutadoTemp = (List<IdSlotsExecutado>)typeof(listaAcoesClass).GetMethod(((AcoesDisponiveis)acaoAtual.acao).ToString()).
                                        Invoke(this, acaoAtual.parametros.ToArray());
                if (idSlotsExecutadoTemp != null)
                {
                    idSlotsExecutado.AddRange(idSlotsExecutadoTemp);
                }

            }
        }

        return idSlotsExecutado;
    }

    public bool verificaAcoesBool(GerenciadorJogo gerenciadorJogo, List<int> idSlot = null, bool player = true)
    {

        return verificaAcoesList(gerenciadorJogo, idSlot, player).Count > 0;

    }
}
