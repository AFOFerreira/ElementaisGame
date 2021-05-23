using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiltrosMagicas : MonoBehaviour
{
    public TabGroup tabGroup;
    public GerenciadorColecao gerenciadorColecao;
    public Button todas;
    public Button unicas;
    public Button continuas;
    // Start is called before the first frame update
    void Start()
    {
        todas.onClick.AddListener(() => MostrarTodas(tabGroup.indexBotaoSelecionado));
        unicas.onClick.AddListener(() => MostrarUnicas(tabGroup.indexBotaoSelecionado));
        continuas.onClick.AddListener(() => MostrarContinuas(tabGroup.indexBotaoSelecionado));
    }

    void MostrarTodas(int id)
    {
        if (id == 2)
        {
            gerenciadorColecao.SomenteAuxiliares();
        }
        else if (id == 3)
        {
            gerenciadorColecao.SomenteArmadilhas();
        }
        else
            return;

    }
    void MostrarUnicas(int id)
    {
        if (id == 2)
        {
            gerenciadorColecao.SelecionarAuxiliarUnicas();
        }
        else if (id == 3)
        {
            gerenciadorColecao.SelecionarArmadilhaUnicas();
        }
        else
            return;
    }
    void MostrarContinuas(int id)
    {
        if (id == 2)
        {
            gerenciadorColecao.SelecionarAuxiliarContinuas();
        }
        else if (id == 3)
        {
            gerenciadorColecao.SelecionarArmadilhaContinuas();
        }
        else
            return;
    }
}
