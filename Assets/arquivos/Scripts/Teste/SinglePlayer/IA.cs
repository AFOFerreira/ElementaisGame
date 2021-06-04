﻿using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IA : MonoBehaviour
{
    public bool possoJogar;

    public List<SlotCampo> camposParaJogarMonstro = new List<SlotCampo>();
    public List<SlotCampo> camposParaJogarMagicas = new List<SlotCampo>();
    public List<SlotCampo> campoOcupadosMagicas = new List<SlotCampo>();
    public List<SlotCampo> camposOcupadosMonstros = new List<SlotCampo>();

    public List<CartaGeral> deckTotal = new List<CartaGeral>();
    public List<CartaGeral> cemiterio = new List<CartaGeral>();
    public List<CartaGeral> mao = new List<CartaGeral>();

    public SlotCampo campoSelecionado;

    public TipoJogador tipoIA;
    public TipoFase fase;
    GerenciadorJogo gerenciadorJogo;
    // Start is called before the first frame update

    void Start()
    {
        gerenciadorJogo = GerenciadorJogo.instance;
        Timing.RunCoroutine(iniciarObjetos());
        VerificaCampos();
        VerificaMeuTurno();
        for (int i = 0; i < 5; i++)
        {
            int id = Random.Range(0, deckTotal.Count - 1);
            passarParaMao(id);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (gerenciadorJogo.EmJogo)
        {
            VerificaMeuTurno();
            if (possoJogar)
            {
                VerificaCampos();
                fase = gerenciadorJogo.faseAtual;
                if (fase == TipoFase.MONSTRO)
                {
                    if (camposParaJogarMonstro.Count > camposOcupadosMonstros.Count -1)
                    {

                        for (int i = 0; i < camposParaJogarMonstro.Count; i++)
                        {
                            campoSelecionado = camposParaJogarMonstro[i];
                        }

                        Debug.Log(tipoIA + ": Posso jogar no campo: " + campoSelecionado.idCampo);
                        if (campoSelecionado.ocupado == false)
                        {
                            Debug.Log("Esperando jogada!");
                            var c = Random.Range(0, mao.Count);
                            var cartaA = mao[c];
                            if (Input.GetButtonDown("Jump"))
                            {
                                gerenciadorJogo.JogarCarta(cartaA, campoSelecionado.idCampo);
                            }
                        }
                        else
                        {
                            Debug.Log(tipoIA + ": Joguei no campo " + campoSelecionado.idCampo);
                            gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                        }

                    }
                    else
                    {
                        Debug.Log(tipoIA + ": Não posso jogar, nao há campos disponiveis!");
                        //gameManager.PassaFase();
                        gerenciadorJogo.faseAtual = TipoFase.MAGICA;
                    }
                }
                else if (fase == TipoFase.MAGICA)
                {
                    //if (camposParaJogarMagicas.Count > 0)
                    //{
                    //    for (int i = 0; i < camposParaJogarMagicas.Count; i++)
                    //    {
                    //        campoSelecionado = camposParaJogarMagicas[i];
                    //    }
                    //    Debug.Log(tipoIA + ": Posso jogar no campo: " + campoSelecionado.idCampo);
                    //    if (!campoSelecionado.ocupado)
                    //    {
                    //        Debug.Log("Esperando jogada!");
                    //        var c = Random.Range(0, mao.Count);
                    //        var cartaA = mao[c];
                    //        if (Input.GetButtonDown("Jump"))
                    //        {
                    //            //gerenciadorJogo.JogarCarta(campoSelecionado, cartaA.gameObject);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log(tipoIA + ": Não posso jogar, nao há campos disponiveis!");
                    //    gerenciadorJogo.PassaTurno();
                    //}
                    gerenciadorJogo.PassaTurno();
                }
                else
                {
                    //TURNO PARA DEFESA.
                    Debug.Log(tipoIA + ": Realizando defesa");
                }
            }
            else
            {
                Debug.Log(tipoIA + ": Não posso jogar, não é a minha vez!!");
                campoSelecionado = null;
                //DO NOTHING!
            }
        }
    }

    public void passarParaMao(int id)
    {
        if (mao.Count < 7)
        {
            CartaGeral carta = deckTotal[id];
            deckTotal.RemoveAt(id);
            mao.Add(carta);
        }
    }
    void VerificaMeuTurno()
    {
        if (tipoIA == gerenciadorJogo.turno)
            possoJogar = true;
        else
            possoJogar = false;
    }

    void VerificaCampos()
    {
        //camposParaJogarMagicas = gerenciadorJogo.VerificaCampoDisponivelMagicas(tipoIA);
        //campoOcupadosMagicas = gerenciadorJogo.VerificaCampoOcupadoMagicas(tipoIA);
        camposParaJogarMonstro = gerenciadorJogo.VerificaCampoDisponivelMonstros(tipoIA);
        camposOcupadosMonstros = gerenciadorJogo.VerificaCampoOcupadoMonstros(tipoIA);
    }

    IEnumerator<float> iniciarObjetos()
    {
        ElementoGeral fogo = new ElementoGeral(2, "Fogo", Resources.Load<Sprite>("imagens/Elementos/icon_fogo"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_fogo"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraFogo"),
            Resources.LoadAll<Sprite>("imagens/Elementos/fogoAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Fogo2Sprite").ToList());
        ElementoGeral agua = new ElementoGeral(0, "Água", Resources.Load<Sprite>("imagens/Elementos/icon_agua"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_agua"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraAgua"),
            Resources.LoadAll<Sprite>("imagens/Elementos/aguaAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Agua2Sprite").ToList());
        ElementoGeral terra = new ElementoGeral(1, "Terra", Resources.Load<Sprite>("imagens/Elementos/icon_terra"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_terra"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraTerra"),
            Resources.LoadAll<Sprite>("imagens/Elementos/terraAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Terra2Sprite").ToList());
        ElementoGeral ar = new ElementoGeral(3, "Ar", Resources.Load<Sprite>("imagens/Elementos/icon_ar"),
            Resources.Load<Sprite>("imagens/Elementos/modelo_ar"),
            Resources.Load<Sprite>("imagens/Elementos/bandeiraAr"),
            Resources.LoadAll<Sprite>("imagens/Elementos/arAtivar").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar1Sprite").ToList(),
            Resources.LoadAll<Sprite>("imagens/Elementos/AtaqueNovo/Ar2Sprite").ToList());


        deckTotal.Add(new CartaGeral(0, Resources.Load<Sprite>("imagens/Elementais/01/01-card"),
            Resources.Load<Sprite>("imagens/Elementais/01/01_arco"), "Pandion",
            "Este é o Pandion de fogo uma águia da espécie Imperial Oriental de estágio 1 da classe Guerreiro, seus ataques são rápidos e ferozes.",
            28, 19, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/01/01-sprites").ToList()));
        deckTotal.Add(new CartaGeral(1, Resources.Load<Sprite>("imagens/Elementais/02/02-card"),
            Resources.Load<Sprite>("imagens/Elementais/02/02_arco"), "Pandion",
            "Este é o Pandion de ar uma águia da espécie Harpia de estágio 1 da raça Curandeiro. Suas habilidades são seus chicotes dourados, e claro sua agilidade.",
            19, 22, 1, 1, ar, Resources.LoadAll<Sprite>("imagens/Elementais/02/02-sprites").ToList()));
        deckTotal.Add(new CartaGeral(2, Resources.Load<Sprite>("imagens/Elementais/03/03-card"),
            Resources.Load<Sprite>("imagens/Elementais/03/03_arco"), "Dubhan",
            "Este Elemental é da espécie de Jabutis é muito comum ser encontrado nas matas brasileiras do Nordeste ou Sudeste.",
            24, 56, 1, 2, terra, Resources.LoadAll<Sprite>("imagens/Elementais/03/03-sprites").ToList()));
        deckTotal.Add(new CartaGeral(3, Resources.Load<Sprite>("imagens/Elementais/04/04-card"),
            Resources.Load<Sprite>("imagens/Elementais/04/04_arco"), "Dubhan",
            "Este Dubhan de fogo é da família dos Jabutis, este elemental é um mini vulcão ambulante, seus tentáculos são capazes de prender um predador ou cuspir um forte jato de uma espécie de gás inflamável.",
            59, 44, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/04/04-sprites").ToList()));
        deckTotal.Add(new CartaGeral(4, Resources.Load<Sprite>("imagens/Elementais/05/05-card"),
            Resources.Load<Sprite>("imagens/Elementais/05/05_arco"), "Dubhan",
            "Este Dubhan é o segundo estágio de evolução da espécie de Jabutis de água, com ataque 55 e defesa 37.", 55,
            37, 2, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/05/05-sprites").ToList()));
        deckTotal.Add(new CartaGeral(5, Resources.Load<Sprite>("imagens/Elementais/06/06-card"),
            Resources.Load<Sprite>("imagens/Elementais/06/06_arco"), "Taireth",
            "Taireth é uma mutação dos tubaões brancos, ele vive em zonas tropicais de águas quentes, seu tamanho pode chegar aos 5 metros e a pesar 200Kg.",
            25, 12, 1, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/06/06-sprites").ToList()));
        deckTotal.Add(new CartaGeral(6, Resources.Load<Sprite>("imagens/Elementais/07/07-card"),
            Resources.Load<Sprite>("imagens/Elementais/07/07_arco"), "Taireth",
            "Taireth de terra é da família do Tubarão-tigre, vive em águas tropicais, em suas costas este Elemental carrega uma espécie de coral hipnótico paralisante, que serve para facilitar a caça.",
            17, 30, 1, 1, agua, Resources.LoadAll<Sprite>("imagens/Elementais/07/07-sprites").ToList()));
        deckTotal.Add(new CartaGeral(7, Resources.Load<Sprite>("imagens/Elementais/08/08-card"),
            Resources.Load<Sprite>("imagens/Elementais/08/08_arco"), "Caedin",
            "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiu controlar muito bem a terra e pedras ao seu redor.",
            21, 36, 1, 1, terra, Resources.LoadAll<Sprite>("imagens/Elementais/08/08-sprites").ToList()));
        deckTotal.Add(new CartaGeral(8, Resources.Load<Sprite>("imagens/Elementais/09/09-card"),
            Resources.Load<Sprite>("imagens/Elementais/09/09_arco"), "Caedin",
            "Caedin de terra é um cachorro com uma super proteção natural, seu corpo tem pequenos escudos de pedra, e ao longo do tempo conseguiru controlar muito bem a terra e pedras ao seu redor.",
            21, 5, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/09/09-sprites").ToList()));
        deckTotal.Add(new CartaGeral(9, Resources.Load<Sprite>("imagens/Elementais/10/10-card"),
            Resources.Load<Sprite>("imagens/Elementais/10/10_arco"), "Caedin",
            "Caedin de fogo é o segundo estágio da evolução de um cachorro do elemento fogo. Agora, Caedin tem uma calda a mais, seu corpo tem uma inversão de cores e seus olho grande quantia de calor.",
            49, 38, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/10/10-sprites").ToList()));
        deckTotal.Add(new CartaGeral(10, Resources.Load<Sprite>("imagens/Elementais/11/11-card"),
            Resources.Load<Sprite>("imagens/Elementais/11/11_arco"), "Gork",
            "Gorki tem habilidades para controlar as plantas terrestres, ele consegue fazer as flores soltarem venenos, as raízes saírem do chão e fazer com que as gramas cresçam muito alto e rapidamente.",
            19, 31, 1, 1, terra, Resources.LoadAll<Sprite>("imagens/Elementais/11/11-sprites").ToList()));
        deckTotal.Add(new CartaGeral(11, Resources.Load<Sprite>("imagens/Elementais/12/12-card"),
            Resources.Load<Sprite>("imagens/Elementais/12/12_arco"), "Gork",
            "Este Primata do elemento fogo em estágio 1 da raça Guerreiro com ataque 29 e defesa 20, possui poderes vulcânicos.",
            29, 20, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/12/12-sprites").ToList()));
        deckTotal.Add(new CartaGeral(12, Resources.Load<Sprite>("imagens/Elementais/13/13-card"),
            Resources.Load<Sprite>("imagens/Elementais/13/13_arco"), "Gaeron",
            "Gaeron de fogo é está em seu primeiro estágio evolutivo, sua maior habilidade são os disparos de pequenas bolas de chamas. Em sua cabeça e coluna é possível ver alguns ossos saltando de seu corpo.",
            33, 5, 1, 1, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/13/13-sprites").ToList()));
        deckTotal.Add(new CartaGeral(13, Resources.Load<Sprite>("imagens/Elementais/14/14-card"),
            Resources.Load<Sprite>("imagens/Elementais/14/14_arco"), "Gaeron",
            "Este Elemental felino em estágio de evolução 2 possui suas habilidades e amaduras melhoradas e fortificadas.",
            69, 48, 1, 2, fogo, Resources.LoadAll<Sprite>("imagens/Elementais/14/14-sprites").ToList()));
        deckTotal.Add(new CartaGeral(14, Resources.Load<Sprite>("imagens/Elementais/15/15-card"),
            Resources.Load<Sprite>("imagens/Elementais/15/15_arco"), "Gaeron",
            "Evolução de estágio 2 do felino de terra da classe Tanque e possui uma forte defesa decorrente de sua armadura evoluida.",
            38, 57, 1, 2, terra, Resources.LoadAll<Sprite>("imagens/Elementais/15/15-sprites").ToList()));


        yield return Timing.WaitForOneFrame;

    }
}