using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]

public class LinhaBezier : MonoBehaviour
{
    public bool setaAtiva = false;
    //vetor com todas as posições.
    public Vector2 initialPoint, finalPoint, pointControllerA;

    //Renderizador da linha.
    LineRenderer lineRenderer;

    //variavel controladora de tempo de exibição da curva.
    float t1 = 0;

    //total de pontos de controle da linha(Quanto mais pontos, mais suave a linha ficará. Porem gasta mais memoria).
    int numPoints = 20;

    //referencia para vetor em memoria
    Vector3[] positions;

    void Start()
    {
        //Vetor é criado em memoria com o total de pontos configurados na variavel.
        positions = new Vector3[numPoints];

        //Instrução para verificar se existe um componente LineRenderer.
        if (!lineRenderer)
        {
            //pega o componente adicionado pelo script.
            lineRenderer = GetComponent<LineRenderer>();
        }

        //Seta o ponto inicial com as posicoes do objeto em que esse script foi anexado.
        initialPoint = this.transform.position;

        //chama a função para configurar o componente LineRenderer.
        SetConfigs();
    }

    void Update()
    {
        //verifica se o botão do mouse esta pressionado.
        if (Input.GetMouseButton(0) && setaAtiva)
        {
            //deixa o valor da variavel de controle em zero antes de renderizar a curva.
            t1 = 0.0f;

            //chama a função para renderizar a curva na tela.
            DrawCurve();
        }
        else
        {
            //chama a função para ocultar a linha.
            OcultLine();
        }
    }

    //Oculta a linha quando a mesma for desselecionada
    void OcultLine()
    {
        //variavel de controle interna da função para controle do loop.
        float t2 = 3.0f;

        //loop para controle de tempo de exibição da linha antes de desaparecer completamente da tela.
        if (t1 < t2)
        {
            //incremento da variavel de controle principal.
            t1 += 0.01f;
        }
        else
        {
            //zera as posições da linha e oculta da tela.
            lineRenderer.positionCount = 0;

            //zera a variavel de controle novamente.
            t1 = 0;
        }

    }

    //renderiza a curva no line renderer.
    private void DrawCurve()
    {
        Vector3 calculo;
        //Adiciona todas as posições no lineRenderer, Sem isso o unity acusa um erro de Array.
        lineRenderer.positionCount = numPoints;

        //pega a posição final do mouse ou toque e converte em distancias para o mundo.
        finalPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ///
        /// Variaveis de controle de posições do ponto de controle.
        /// 
        //posicionamento no eixo X.
        float midlePointY = Mathf.Abs(finalPoint.y) + 3;
        //posicionamento no eio Y.
        float midlePointX = initialPoint.x - finalPoint.x;

        //cria um laço e percore um array passando os valores para o line renderer
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            //instrução para controle dinamico do ponto central para interpolação da linha reta ou curva.
            if (finalPoint.y > 2)
            {
                //calcula a cruva linear de bézier;
                calculo = CalculateLinearBezier(t, initialPoint, finalPoint);
            }
            else
            {
                //Calcula a curva auadrática de bézier.
                calculo = CalculateQuadraticBezierPoint(t, initialPoint,
               pointControllerA,
                finalPoint);
            }

            //define as posições em um array.
            positions[i - 1] = calculo;
        }
        //Ponto central recebe os valores acima.
        pointControllerA = new Vector3(midlePointX, midlePointY);

        //define as posições no LineRenderer.
        lineRenderer.SetPositions(positions);
    }

    //calcula a curva quadratica de bézier.
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += 3 * u * t * p2;
        p += tt * p2;
        return p;
    }

    //Calcula a curva linear de bézier
    private Vector3 CalculateLinearBezier(float t, Vector3 p0, Vector3 p1)
    {
        return p0 + t * (p1 - p0);
    }

    //Define as propriedades padrões do line renderer, sem precisar configurar manualmente.
    void SetConfigs()
    {
        //Cria um objeto do tipo curva de animação.
        AnimationCurve curve = new AnimationCurve();

        //define a espessura inicial da linha passando uma chave do tipo curva de animação.
        curve.AddKey(0.0f, 0.2f);

        //define define uma chave de controle para criação de ponta na linha.
        curve.AddKey(0.95f, 0.2f);

        //define a espessura final da linha passando uma chave.
        curve.AddKey(1.0f, 0.1f);

        //define o tamanho multiplicador da linha ("Esse tamanho é multiplicado por todo o seguimento da linha").
        lineRenderer.widthMultiplier = 0.5f;

        //define o tamanho da curva.
        lineRenderer.widthCurve = curve;

        //define o material padrão da linha,com o material da propria unity.
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        //define uma cor inicial da linha (um vemelho mais escuro), usando a notação de cores padrão da unity.
        Color initalColor = new Color(0.3846484f, 0.01343489f, 0.01343489f, 1);

        //define o alpha como 1.
        float alpha = 1.0f;

        //cria um novo objeto da classe "Gradient" padrão da unity para cores gradientes.
        Gradient gradient = new Gradient();

        //define chaves para controle de cores na linha.
        gradient.SetKeys(
            //define chaves com valores de cores.
            new GradientColorKey[] { new GradientColorKey(initalColor, 0.75f), new GradientColorKey(Color.red, 1.0f) },
            //define chaves com valores para a propriedade alpha das cores.
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.15f), new GradientAlphaKey(alpha, 1.0f) }
        );

        //define as chaves lo LineRenderer.
        lineRenderer.colorGradient = gradient;
    }
}
