using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncoesNpp
{

    //baixar imagem

    //SalvarImagem drive

    //pegar imagem drive


    public static string pegaNomeUrl(string url)
    {
        string nome = url;
        if(url != null)
        {
            string[] palavras = url.Split(new char[] { '/', '.', '\\' });
            int qtd = palavras.Length - 2;

            nome = palavras[qtd];


        }

        return nome;
    }
}
