using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using System;

public class AudioBase : MonoBehaviour
{
    [FMODUnity.EventRef]
    Guid _musicaMenu = new Guid("c3a01e20-430e-4f74-b851-cd01c1ddea8c");
    FMOD.Studio.EventInstance musicaMenu;

    [FMODUnity.EventRef]
    Guid _musicaGameplay = new Guid("9b7155e6-e936-4119-9322-4275a3d22770");
    FMOD.Studio.EventInstance musicaGameplay;

    [FMODUnity.EventRef]
    Guid _musicaLobby = new Guid("2fcfe62f-8868-43d5-af5a-743a0b86842e");
    FMOD.Studio.EventInstance musicaLobby;

    [FMODUnity.EventRef]
    Guid _musicaTelaBatalha = new Guid("ee054170-4b92-49d6-949d-779fc2149179");
    FMOD.Studio.EventInstance musicaTelaBatalha;

    [FMODUnity.EventRef]
    Guid _musicaTutorial = new Guid("485a96ac-ab90-4dd4-869c-ba27bdbdae55");
    FMOD.Studio.EventInstance musicaTutorial;

    [FMODUnity.EventRef]
    Guid _musicaVitoria = new Guid("d005344e-5931-4afe-af0f-805970937f54");
    FMOD.Studio.EventInstance musicaVitoria;

    [FMODUnity.EventRef]
    Guid _musicaDerrota = new Guid("0fabd485-4159-48c9-845d-8850b22c5cb9");
    FMOD.Studio.EventInstance musicaDerrota;

    [FMODUnity.EventRef]
    Guid _fanfarraLoading = new Guid("15d214f2-ece3-49eb-9d03-d16c264d0b92");
    FMOD.Studio.EventInstance fanfarraLoading;

    [FMODUnity.EventRef]
    Guid _ataqueAgua = new Guid("f606c22c-161e-4850-a9a0-6f6068529c9c");
    FMOD.Studio.EventInstance ataqueAgua;

    [FMODUnity.EventRef]
    Guid _ataqueTerra = new Guid("d97c0904-8643-4ef2-9abd-b23b6aefc36f");
    FMOD.Studio.EventInstance ataqueTerra;

    [FMODUnity.EventRef]
    Guid _ataqueFogo = new Guid("9870e9b3-12be-4e6a-8698-462dac6a5807");
    FMOD.Studio.EventInstance ataqueFogo;

    [FMODUnity.EventRef]
    Guid _ataqueAr = new Guid("a047de91-1cec-4e15-b58f-a1a26b84216e");
    FMOD.Studio.EventInstance ataqueAr;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoGenerico = new Guid("1e35a643-7eb4-4553-9845-c6dfaa577f91");
    FMOD.Studio.EventInstance cliqueBotaoGenerico;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoPlay = new Guid("33297f0c-c93a-4083-9b96-1dbbf1797b3d");
    FMOD.Studio.EventInstance cliqueBotaoPlay;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoAgua = new Guid("ee3a039e-1413-4aaf-8f9e-64d3d0dff4cd");
    FMOD.Studio.EventInstance cliqueBotaoAgua;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoTerra = new Guid("ee3a039e-1413-4aaf-8f9e-64d3d0dff4cd");
    FMOD.Studio.EventInstance cliqueBotaoTerra;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoFogo = new Guid("ee3a039e-1413-4aaf-8f9e-64d3d0dff4cd");
    FMOD.Studio.EventInstance cliqueBotaoFogo;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoAr = new Guid("ee3a039e-1413-4aaf-8f9e-64d3d0dff4cd");
    FMOD.Studio.EventInstance cliqueBotaoAr;

    [FMODUnity.EventRef]
    Guid _cliqueBotaoElemento = new Guid("ee3a039e-1413-4aaf-8f9e-64d3d0dff4cd");
    FMOD.Studio.EventInstance cliqueBotaoElemento;

    [FMODUnity.EventRef]
    Guid _negacao = new Guid("b9ff3c7d-a35c-4e30-8bc4-041a1fe8496d");
    FMOD.Studio.EventInstance negacao;

    [FMODUnity.EventRef]
    Guid _abrirHud = new Guid("15ffbf5a-2e23-4edb-a547-e83679d9618b");
    FMOD.Studio.EventInstance abrirHud;

    [FMODUnity.EventRef]
    Guid _fecharHud = new Guid("3c32e062-6257-41c4-ad81-0c9cce6ae97a");
    FMOD.Studio.EventInstance fecharHud;

    [FMODUnity.EventRef]
    Guid _cristalAgua = new Guid("de048a65-8674-4b08-ad70-bf36d5c99c52");
    FMOD.Studio.EventInstance cristalAgua;

    [FMODUnity.EventRef]
    Guid _cristalTerra = new Guid("de048a65-8674-4b08-ad70-bf36d5c99c52");
    FMOD.Studio.EventInstance cristalTerra;

    [FMODUnity.EventRef]
    Guid _cristalFogo = new Guid("de048a65-8674-4b08-ad70-bf36d5c99c52");
    FMOD.Studio.EventInstance cristalFogo;

    [FMODUnity.EventRef]
    Guid _cristalAr = new Guid("de048a65-8674-4b08-ad70-bf36d5c99c52");
    FMOD.Studio.EventInstance cristalAr;

    [FMODUnity.EventRef]
    Guid _cristalElemento = new Guid("de048a65-8674-4b08-ad70-bf36d5c99c52");
    FMOD.Studio.EventInstance cristalElemento;

    [FMODUnity.EventRef]
    Guid _relogio = new Guid("a93dfebc-336c-4ddc-a382-99b7697bda71");
    FMOD.Studio.EventInstance relogio;

    [FMODUnity.EventRef]
    Guid _trocaturnoprincipal = new Guid("4665492d-bfe0-4b17-b83e-a3f327f29d68");
    FMOD.Studio.EventInstance trocaturnoprincipal;

    [FMODUnity.EventRef]
    Guid _trocaturnosecundAria = new Guid("9e24c1ac-7c73-4663-a09b-d923499c5cc5");
    FMOD.Studio.EventInstance trocaturnosecundAria;

    [FMODUnity.EventRef]
    Guid _cartasEmbaralhando = new Guid("3e3207c0-9fb0-4e06-b5ca-d7a99bf751ca");
    FMOD.Studio.EventInstance cartasEmbaralhando;

    [FMODUnity.EventRef]
    Guid _cartaMostrandoouVirando = new Guid("42a1c522-81f6-4ce7-9eb7-483cd7634bfc");
    FMOD.Studio.EventInstance cartaMostrandoouVirando;

    [FMODUnity.EventRef]
    Guid _cartaBaixando = new Guid("e43d80dd-e290-489d-a02d-91a8e6968cfd");
    FMOD.Studio.EventInstance cartaBaixando;

    [FMODUnity.EventRef]
    Guid _cartaAuxiliar = new Guid("8fcb418a-e990-4973-bb10-938cf55c33d0");
    FMOD.Studio.EventInstance cartaAuxiliar;

    [FMODUnity.EventRef]
    Guid _cartanaMao = new Guid("89eec0e3-c0a6-4ec4-bbed-2eccf41258ab");
    FMOD.Studio.EventInstance cartanaMao;

    /// <summary>
    /// ATENÇÃO!!
    /// DAQUI PARA BAIXO FOI CRIADO NÃO SEI SE ESTÁ COM O MESMO NOME QUE O SEU
    /// </summary>
    [FMODUnity.EventRef]
    Guid _danoDiretoMasculino = new Guid("799de145-51c5-40b4-b7d5-68df7dac4d2f");
    FMOD.Studio.EventInstance danoDiretoMasculino;

    [FMODUnity.EventRef]
    Guid _danoDiretoFeminino = new Guid("5beda573-ea17-4645-b9a2-6dd50bdef14a");
    FMOD.Studio.EventInstance danoDiretoFeminino;

    [FMODUnity.EventRef]
    Guid _invocacaoCriatura = new Guid("429ef97d-1260-4597-80c1-6f190955bbcb");
    FMOD.Studio.EventInstance invocacaoCriatura;

    [FMODUnity.EventRef]
    Guid _criaturaMorrendo = new Guid("02a29a86-da39-4f0a-bc35-2b444118d6bd");
    FMOD.Studio.EventInstance criaturaMorrendo;

    [FMODUnity.EventRef]
    Guid _moedaSrteio = new Guid("b179f275-ef4e-447e-ae38-6ad45b0648f7");
    FMOD.Studio.EventInstance moedaSorteio;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ATENÇÃO!!
    /// até aqui!
    /// </summary>


    public void playMusicaMenu()
    {
        musicaMenu = FMODUnity.RuntimeManager.CreateInstance(_musicaMenu);
        musicaMenu.start();
    }
    public void playMusicaGameplay()
    {
        musicaGameplay = FMODUnity.RuntimeManager.CreateInstance(_musicaGameplay);
        musicaGameplay.start();
    }
    public void playMusicaLobby()
    {
        musicaLobby = FMODUnity.RuntimeManager.CreateInstance(_musicaLobby);
        musicaLobby.start();
    }
    public void playMusicaTelaBatalha()
    {
        musicaTelaBatalha = FMODUnity.RuntimeManager.CreateInstance(_musicaTelaBatalha);
        musicaTelaBatalha.start();
    }
    public void playMusicaTutorial()
    {
        musicaTutorial = FMODUnity.RuntimeManager.CreateInstance(_musicaTutorial);
        musicaTutorial.start();
    }
    public void playMusicaVitoria()
    {
        musicaVitoria = FMODUnity.RuntimeManager.CreateInstance(_musicaVitoria);
        musicaVitoria.start();
    }
    public void playMusicaDerrota()
    {
        musicaDerrota = FMODUnity.RuntimeManager.CreateInstance(_musicaDerrota);
        musicaDerrota.start();
    }
    public void playFanfarraLoading()
    {
        fanfarraLoading = FMODUnity.RuntimeManager.CreateInstance(_fanfarraLoading);
        fanfarraLoading.start();
    }
    public void playAtaqueAgua()
    {
        ataqueAgua = FMODUnity.RuntimeManager.CreateInstance(_ataqueAgua);
        ataqueAgua.start();
    }
    public void playAtaqueTerra()
    {
        ataqueTerra = FMODUnity.RuntimeManager.CreateInstance(_ataqueTerra);
        ataqueTerra.start();
    }
    public void playAtaqueFogo()
    {
        ataqueFogo = FMODUnity.RuntimeManager.CreateInstance(_ataqueFogo);
        ataqueFogo.start();
    }
    public void playAtaqueAr()
    {
        ataqueAr = FMODUnity.RuntimeManager.CreateInstance(_ataqueAr);
        ataqueAr.start();
    }
    public void playCliqueBotaoGenerico()
    {
        cliqueBotaoGenerico = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoGenerico);
        cliqueBotaoGenerico.start();
    }
    public void playCliqueBotaoPlay()
    {
        cliqueBotaoPlay = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoPlay);
        cliqueBotaoPlay.start();
    }
    public void playCliqueBotaoAgua()
    {
        cliqueBotaoAgua = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoAgua);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Elemento", 0);
        cliqueBotaoAgua.start();
    }
    public void playCliqueBotaoTerra()
    {
        cliqueBotaoTerra = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoTerra);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Elemento", 1);
        cliqueBotaoTerra.start();
    }
    public void playCliqueBotaoFogo()
    {
        cliqueBotaoFogo = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoFogo);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Elemento", 2);
        cliqueBotaoFogo.start();
    }
    public void playCliqueBotaoAr()
    {
        cliqueBotaoAr = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoAr);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Elemento", 3);
        cliqueBotaoAr.start();
    }

    public void playCliqueBotaoElemento(float elemento)
    {
        cliqueBotaoElemento = FMODUnity.RuntimeManager.CreateInstance(_cliqueBotaoElemento);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Elemento", elemento);
        cliqueBotaoElemento.start();
    }

    public void playNegacao()
    {
        negacao = FMODUnity.RuntimeManager.CreateInstance(_negacao);
        negacao.start();
    }
    public void playAbrirHud()
    {
        abrirHud = FMODUnity.RuntimeManager.CreateInstance(_abrirHud);
        abrirHud.start();
    }
    public void playFecharHud()
    {
        fecharHud = FMODUnity.RuntimeManager.CreateInstance(_fecharHud);
        fecharHud.start();
    }
    public void playCristalAgua()
    {
        cristalAgua = FMODUnity.RuntimeManager.CreateInstance(_cristalAgua);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCristal", 0);
        cristalAgua.start();
    }
    public void playCristalTerra()
    {
        cristalTerra = FMODUnity.RuntimeManager.CreateInstance(_cristalTerra);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCristal", 1);
        cristalTerra.start();
    }
    public void playCristalFogo()
    {
        cristalFogo = FMODUnity.RuntimeManager.CreateInstance(_cristalFogo);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCristal", 2);
        cristalFogo.start();
    }
    public void playCristalAr()
    {
        cristalAr = FMODUnity.RuntimeManager.CreateInstance(_cristalAr);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCristal", 3);
        cristalAr.start();
    }

    public void playCristalElemento(float elemento)
    {
        cristalElemento = FMODUnity.RuntimeManager.CreateInstance(_cristalElemento);
        //cristalElemento.setParameterByName("TipoCristal", elemento);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCristal", elemento);
        cristalElemento.start();
    }


    public void playRelogio()
    {
        relogio = FMODUnity.RuntimeManager.CreateInstance(_relogio);
        relogio.start();
    }
    public void playTrocaturnoprincipal()
    {
        trocaturnoprincipal = FMODUnity.RuntimeManager.CreateInstance(_trocaturnoprincipal);
        trocaturnoprincipal.start();
    }
    public void playTrocaturnosecundAria()
    {
        trocaturnosecundAria = FMODUnity.RuntimeManager.CreateInstance(_trocaturnosecundAria);
        trocaturnosecundAria.start();
    }
    public void playCartasEmbaralhando()
    {
        cartasEmbaralhando = FMODUnity.RuntimeManager.CreateInstance(_cartasEmbaralhando);
        cartasEmbaralhando.start();
    }
    public void playCartaMostrandoouVirando()
    {
        cartaMostrandoouVirando = FMODUnity.RuntimeManager.CreateInstance(_cartaMostrandoouVirando);
        cartaMostrandoouVirando.start();
    }
    public void playCartaBaixando()
    {
        cartaBaixando = FMODUnity.RuntimeManager.CreateInstance(_cartaBaixando);
        cartaBaixando.start();
    }
    public void playCartaAuxiliar()
    {
        cartaAuxiliar = FMODUnity.RuntimeManager.CreateInstance(_cartaAuxiliar);
        cartaAuxiliar.start();
    }
    public void playCartanaMao()
    {
        cartanaMao = FMODUnity.RuntimeManager.CreateInstance(_cartanaMao);
        cartanaMao.start();
    }


    public void stopMusicaMenu()
    {
        musicaMenu.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaGameplay()
    {
        musicaGameplay.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaLobby()
    {
        musicaLobby.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaTelaBatalha()
    {
        musicaTelaBatalha.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaTutorial()
    {
        musicaTutorial.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaVitoria()
    {
        musicaVitoria.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopMusicaDerrota()
    {
        musicaDerrota.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void stopCartanaMao()
    {
        cartanaMao.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    /// <summary>
    /// ATENÇÃO!!
    /// DAQUI PARA BAIXO FOI CRIADO NÃO SEI SE ESTÁ COM O MESMO NOME QUE O SEU
    /// </summary>
    public void playDanoDiretoMasculino()
    {
        danoDiretoMasculino = FMODUnity.RuntimeManager.CreateInstance(_danoDiretoMasculino);
        danoDiretoMasculino.start();
    }

    public void playDanoDiretoFeminino()
    {
        danoDiretoFeminino = FMODUnity.RuntimeManager.CreateInstance(_danoDiretoFeminino);
        danoDiretoFeminino.start();
    }

    public void playInvocacaoCriatura(float elemento)
    {
        invocacaoCriatura = FMODUnity.RuntimeManager.CreateInstance(_invocacaoCriatura);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TipoCriatura", elemento);
        invocacaoCriatura.start();
    }

    public void playCriaturaMorrendo()
    {
        criaturaMorrendo = FMODUnity.RuntimeManager.CreateInstance(_criaturaMorrendo);
        criaturaMorrendo.start();
    }

    public void playMoedaSorteio()
    {
        moedaSorteio = FMODUnity.RuntimeManager.CreateInstance(_moedaSrteio);
        moedaSorteio.start();
    }

    /// <summary>
    /// ATENÇÃO!!
    /// o método abaixo não existia, criei para facilitar o processo em um outro script
    /// </summary>
    /// 

    public void playAtaqueElemental(int idElemento)
    {
        switch (idElemento)
        {
            case 0: playAtaqueAgua();
                break;

            case 1:
                playAtaqueTerra();
                break;

            case 2:
                playAtaqueFogo();
                break;

            case 3:
                playAtaqueAr();
                break;
            default:
                break;
        }

    }
}
