using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using DG.Tweening;

public enum ParametroAlvo
{
    ataque, defesa
}
public enum AcoesDisponiveis
{
    nada,
    alterarAtaqueDefesa,
    alterarAtaqueDefesaPorElemento,
    alterarVidaPlayer
}
public class Acoes
{
    public AcoesDisponiveis acao;
    public List<object> parametros;

    public Acoes(AcoesDisponiveis acao, List<object> parametros)
    {
        this.acao = acao;
        this.parametros = parametros;
    }

}
public class IdSlotsExecutado
{
    public bool player;
    public int idSlot;

    public IdSlotsExecutado(bool player, int idSlot)
    {
        this.player = player;
        this.idSlot = idSlot;
    }
}
public static class listaAcoesClass
{

    public static List<IdSlotsExecutado> alterarAtaqueDefesaPorElemento(GerenciadorJogo gerenciadorJogo, List<int> idSlots, bool player, ParametroAlvo parametroAlvo, int qtdAlterar, List<ElementoGeral> elementos, bool verificando = false)
    {
        List<IdSlotsExecutado> idSlotsExecutado = new List<IdSlotsExecutado>();
        List<SlotElemental> slotBusca;
        slotBusca = ((player) ? gerenciadorJogo.gerenciadorUI.slotDetalhesP1 : gerenciadorJogo.gerenciadorUI.slotDetalhesP2);

        //se não passar o id, signific que aplica a todos os elementais daqueles elementos
        if (idSlots == null)
        {
            idSlots = new List<int>();

            foreach (SlotElemental slot in slotBusca)
            {
                foreach (ElementoGeral elemento in elementos)
                {
                    if (slot.cartaGeralTemp.elemento == elemento)
                    {
                        idSlots.Add(slot.idSlot);
                    }
                }
            }

            idSlotsExecutado = alterarAtaqueDefesa(gerenciadorJogo, idSlots, player, parametroAlvo, qtdAlterar, verificando);
        }
        else //verifica se o elemental passado é de algum dos elementos possiveis
        {
            bool deuCerto = false;
            foreach (SlotElemental slot in slotBusca)
            {
                foreach (var elemento in elementos)
                {
                    if (slot.cartaGeralTemp.elemento == elemento)
                    {
                        deuCerto = true;
                        idSlotsExecutado = alterarAtaqueDefesa(gerenciadorJogo, idSlots, player, parametroAlvo, qtdAlterar, verificando);
                    }
                }
            }

            if (!deuCerto)
            {
                //MENSAGEM - ELEMENTO INCORRETO!!!
                gerenciadorJogo.gerenciadorAudio.playNegacao();
                idSlotsExecutado = null;
            }
        }
        return idSlotsExecutado;
    }
    public static List<IdSlotsExecutado> alterarAtaqueDefesa(GerenciadorJogo gerenciadorJogo, List<int> idSlots, bool player, ParametroAlvo parametroAlvo, int qtdAlterar, bool verificando = false)
    {
        List<IdSlotsExecutado> idSlotsExecutado = new List<IdSlotsExecutado>();
        List<SlotElemental> slotBusca;

        slotBusca = ((player) ? gerenciadorJogo.gerenciadorUI.slotDetalhesP1 : gerenciadorJogo.gerenciadorUI.slotDetalhesP2);

        if (idSlots != null)
        {
            foreach (int slot in idSlots)
            {
                if (!verificando)
                {
                    if (parametroAlvo == ParametroAlvo.ataque)
                    {
                        slotBusca[slot].cartaGeralTemp.ataque += qtdAlterar;
                    }
                    else if (parametroAlvo == ParametroAlvo.defesa)
                    {
                        slotBusca[slot].cartaGeralTemp.defesa += qtdAlterar;
                    }
                    slotBusca[slot].atualizarInformações();
                }
                idSlotsExecutado.Add(new IdSlotsExecutado(player, slot));
            }
        }
        else
        {
            foreach (SlotElemental slot in slotBusca)
            {
                if (slot.cartaGeralTemp != null)
                {
                    if (!verificando)
                    {
                        if (parametroAlvo == ParametroAlvo.ataque)
                        {
                            slot.cartaGeralTemp.ataque += qtdAlterar;
                        }
                        else if (parametroAlvo == ParametroAlvo.defesa)
                        {
                            slot.cartaGeralTemp.defesa += qtdAlterar;
                        }
                        slot.atualizarInformações();
                    }
                    idSlotsExecutado.Add(new IdSlotsExecutado(player, slot.idSlot));
                }
            }
        }
        return idSlotsExecutado;

    }

    public static List<IdSlotsExecutado> alterarVidaPlayer(GerenciadorJogo gerenciadorJogo, List<int> idSlots, bool player, int qtdAlterar, bool verificando = false)
    {
        List<IdSlotsExecutado> idSlotsExecutado = new List<IdSlotsExecutado>();
        idSlotsExecutado.Add(new IdSlotsExecutado(player, 3));

        if(idSlots != null)
        {
            //nada
        }

        if (!verificando)
        {
            Sequence alterarVidaPlayerSequence = DOTween.Sequence().SetId("alterarVidaPlayerSequence");
            alterarVidaPlayerSequence.AppendInterval(2f);
            alterarVidaPlayerSequence.AppendCallback(() =>
            {
                if (qtdAlterar < 0)
                {
                    gerenciadorJogo.gerenciadorUI.AtaqueDireto(!player, Math.Abs(qtdAlterar));

                }
                else
                {
                    gerenciadorJogo.gerenciadorUI.slotsPlayer[player ? 0 : 1].alteraVida(qtdAlterar);
                }
            });
        }


        return idSlotsExecutado;

    }


}
