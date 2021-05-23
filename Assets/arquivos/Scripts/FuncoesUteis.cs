using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using UnityEngine.UI;
using TMPro;

namespace funcoesUteis
{
    public static class FuncoesUteis
    {

        public static void killCorroutines(String tag)
        {
            Timing.KillCoroutines(tag);
        }

        public static void animacaoImagem(Image image, List<Sprite> sprites, Boolean loop, int spritePerFrame, Boolean destroyOnEnd = false, Action callBack = null, String tag = "")
        {
            Timing.RunCoroutine(rodaAnimacao(image, sprites, loop, spritePerFrame, destroyOnEnd, callBack).CancelWith(image.gameObject), Segment.FixedUpdate, tag);
        }

        public static void animacaoImagemLoopTrecho(Image image, List<Sprite> sprites, int spritePerFrame, int frameInicioLoop, String tag = "")
        {
            Timing.RunCoroutine(rodaAnimacaoLoop(image, sprites, spritePerFrame, frameInicioLoop).CancelWith(image.gameObject), Segment.FixedUpdate, tag);
        }
        private static IEnumerator<float> rodaAnimacao(Image image, List<Sprite> sprites, Boolean loop, int spritePerFrame, Boolean destroyOnEnd = false, Action callBack = null)
        {

            int index = 0;
            int frame = 0;


            bool loopWhile = true;

            while (loopWhile)
            {
                if (!loop && index == sprites.Count)
                {
                    //não fazer nada
                }
                else
                {
                    frame++;
                }

                if (frame < spritePerFrame)
                {
                    //não fazer nada
                }
                else
                {
                    image.sprite = sprites[index];
                    frame = 0;
                    index++;

                    if (index >= sprites.Count)
                    {
                        if (loop)
                        {
                            index = 0;
                        }
                        else
                        {
                            callBack?.Invoke();
                            loopWhile = false;
                        }
                        if (destroyOnEnd)
                        {
                            UnityEngine.GameObject.Destroy(image.gameObject);
                        }
                    }
                }
                yield return Timing.WaitForOneFrame;
            }
        }

        private static IEnumerator<float> rodaAnimacaoLoop(Image image, List<Sprite> sprites, int spritePerFrame, int frameInicioLoop)
        {

            int index = 0;
            int frame = 0;
            int step = 1;
            bool completed = false;


            bool loopWhile = true;

            while (loopWhile)
            {
                frame++;

                if (frame >= spritePerFrame)
                {
                    index += step;
                    Debug.Log(index);
                    image.sprite = sprites[index];
                    frame = 0;

                    if (index >= sprites.Count-1)
                    {
                        completed = true;
                        step *= -1;
                    }
                    if (completed && index <= frameInicioLoop)
                    {
                        step *= -1;
                    }
                }
                yield return Timing.WaitForOneFrame;
            }
        }

        public static void ZeraAlfa(this Image imagem)
        {
            imagem.color = new Color(imagem.color.r, imagem.color.g, imagem.color.b, 0f);
        }
        public static void ZeraAlfa(this TextMeshProUGUI texto)
        {
            texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, 0f);
        }

    }
}

