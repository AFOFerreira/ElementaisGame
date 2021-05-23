using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class LoginComponnent : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputSenha;
    public GameObject CadastroComponnent;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.scale(gameObject, new Vector2(1, 1), 0.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public async void Entrar()
    {
        if (isEmpty(inputEmail) && isEmpty(inputSenha))
        {
            var result = await Main.Instance.web.LoginForm(inputEmail.text, inputSenha.text);
            if (result == "0")
            {
                Main.Instance.ShowError("Usuario Não Cadastrado");
                return;
            }
            else
            {
                JObject json = JObject.Parse(result);
                var v = JsonConvert.DeserializeObject<config_usuario>(json.ToString());
                Main.Instance.usuario.Nome = v.Nome;
                Main.Instance.usuario.Nickname = v.Nickname;
                Main.Instance.usuario.Xp = v.Xp;
                Main.Instance.usuario.Nivel = v.Nivel;
                Main.Instance.MainMenu();
            }
        }
        else
        {
            Main.Instance.ShowError("Há campos não preenchidos!");
        }
    }

    private bool isEmpty(TMP_InputField etText)
    {
        string text = etText.text.Trim();
        if (text.Length > 1)
            return true;
        return false;
    }

    public void Close()
    {
        LeanTween.scale(gameObject, new Vector2(0, 0), 0.3f).setOnComplete(DestroyMe);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void ShowCadastro()
    {
        Close();    
        Instantiate(CadastroComponnent, FindObjectOfType<Canvas>().transform);
    }
}
