using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;

public class Web : MonoBehaviour
{

    #region cadastro
    public async Task<String> CadastroForm(string nome, string email, string nickname, string senha)
    {
        string retorno = null;
        try
        {
            string uri = "https://elementaisgame.com/api/v1api/game/cadastro_Form.php";
            WWWForm form = new WWWForm();
            form.AddField("nome", nome);
            form.AddField("email", email);
            form.AddField("nickname", nickname);
            form.AddField("senha", senha);
            form.AddField("auth", 0);

            UnityWebRequest www = UnityWebRequest.Post(uri, form);
            await www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                retorno = null;
            }
            else
            {
                if (www.isDone)
                {
                    Debug.Log(":\nReceived: " + www.downloadHandler.text);
                    retorno = www.downloadHandler.text;
                }
            }
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Erro: " + e.Message);
        }
        return retorno;
    }

    public async Task<String> cadastrarFG(string nome, string email, string nickname)
    {
        string retorno = null;
        try
        {
            string uri = "https://elementaisgame.com/api/v1api/game/cadastro_FG.php";
            WWWForm form = new WWWForm();
            form.AddField("nome", nome);
            form.AddField("email", email);
            form.AddField("nickname", nickname);

            UnityWebRequest www = UnityWebRequest.Post(uri, form);
            await www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("http error: " + www.error);
                return null;
            }
            else
            {
                if (www.isDone)
                {
                    Debug.Log(":\nReceived: " + www.downloadHandler.text);
                    retorno = www.downloadHandler.text;
                }
            }
        }
        catch (HttpRequestException e)
        {
            Debug.Log("\nException Caught!");
            Debug.Log("Erro: " + e.Message);
        }
        return retorno;
    }
    #endregion

    #region Login
    public async Task<string> LoginFG(string email, string auth)
    {
        string result = null;
        try
        {
            string uri = "https://elementaisgame.com/api/v1api/game/loginFG.php";
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("auth", auth);

            UnityWebRequest www = UnityWebRequest.Post(uri, form);

            await www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error);
            else
            {
                result = www.downloadHandler.text;
            }
        }
        catch (HttpRequestException e)
        {
            Debug.Log("erro: \n" + e.Message);

        }

        return result;
    }

    public async Task<string> LoginForm(string email, string senha)
    {
        string result = null;
        try
        {
            string uri = "https://elementaisgame.com/api/v1api/game/loginForm.php";
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("senha", senha);
            form.AddField("auth", 0);

            UnityWebRequest www = UnityWebRequest.Post(uri, form);

            await www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Main.Instance.ShowError(www.error);
            }
            else
            {
                result = www.downloadHandler.text;
            }
        }
        catch (HttpRequestException e)
        {
            Debug.Log("erro: \n" + e.Message);
            Main.Instance.ShowError(e.Message);

        }

        return result;
    }
    #endregion
}

