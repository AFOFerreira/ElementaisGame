
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class Cadastro : MonoBehaviour
{
    public TMP_InputField nome;
    public TMP_InputField sobrenome;
    public TMP_InputField email;
    public TMP_InputField senha;
    public TMP_InputField confirma_senha;
    string nickname;
    string nomeFinal;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.scale(gameObject, new Vector2(1, 1), 0.3f);
    }

    public async void Cadastrar()
    {
        if (isEmpty(nome))
        {
            if (isEmpty(sobrenome))
            {
                if (isEmpty(email) && IsValidEmail(email.text))
                {
                    if (isEmpty(senha))
                    {
                        if (isEmpty(confirma_senha))
                        {
                            if (senha.text == confirma_senha.text)
                            {
                                nomeFinal = nome.text + " " + sobrenome.text;
                                nickname = nome.text + Random.Range(0, 10000).ToString();
                                var x = await Main.Instance.web.CadastroForm(nomeFinal, email.text, nickname, confirma_senha.text);
                                if (x == "0")
                                {
                                    Debug.Log("usuario já cadastrado no sistema");
                                    Main.Instance.ShowError("Este usuario já esta cadastrado no sistema");
                                }
                                else if (x == "1")
                                {
                                    Debug.Log("usuario cadastrado com sucesso!");
                                    Main.Instance.ShowError("Cadastro realizado com sucesso!!");
                                    Close();
                                }
                            }
                            else
                            {
                                Main.Instance.ShowError("as senhas nao coincidem!");
                            }
                        }
                        else
                        {
                            Main.Instance.ShowError("Você precisa confirmar sua senha!");
                        }
                    }
                    else
                    {
                        Main.Instance.ShowError("Você precisa digitar uma senha!");
                    }
                }
                else
                {
                    Main.Instance.ShowError("Você precisa digitar um email válido!");
                }
            }
            else
            {
                Main.Instance.ShowError("Você precisa digitar um sobrenome!");
            }
        }
        else
        {
            Main.Instance.ShowError("Você precisa digitar um nome!");
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
    public void Abrir(bool b)
    {
        this.gameObject.SetActive(b);
    }
    void DestroyMe()
    {
        Destroy(gameObject);
    }
    public static bool IsValidEmail(string email)
    {
        Regex rx = new Regex(
        @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
        return rx.IsMatch(email);
    }
}
