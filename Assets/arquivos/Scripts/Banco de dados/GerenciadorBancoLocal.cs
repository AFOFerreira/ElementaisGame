using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.Networking;
using System.Linq;
#if !UNITY_EDITOR
using System.Collections;
#endif

public class GerenciadorBancoLocal
{
    /// <summary>
    /// Esta é a classe responsavél por todo o gerenciamento do banco de dados do jogo.
    /// </summary>
    //referencia para a classe.
    private SQLiteConnection _connection;

    
    //Cosntrutor para o banco, somente necessario para criar o banco.
    public GerenciadorBancoLocal(string NomeDoBanco)
    {
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", NomeDoBanco);
#else
        //verifica se o arquivo existe e Application.persistentDataPath
        var caminho = string.Format("{0}/{1}", Application.persistentDataPath, NomeDoBanco);
        if (!File.Exists(caminho))
        {
            Debug.Log("Base de dados inexistente");
#if UNITY_ANDROID
            var loadDb = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + NomeDoBanco);  // este é o caminho para os StreamingAssets no Android
            loadDb.SendWebRequest();
            while (!loadDb.isDone) { }  // CUIDADO aqui, por razões de segurança você não deve deixar isso enquanto o loop estiver sem supervisão, coloque um cronômetro e verifique o erro
            //em seguida, salve em Application.persistentDataPath
            File.WriteAllBytes(caminho, loadDb.downloadHandler.data);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + NomeDoBanco;  // este é o caminho para os StreamingAssets no IOS
                //em seguida, salve em Application.persistentDataPath
                File.Copy(loadDb, caminho);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + NomeDoBanco;  // este é o caminho para seus StreamingAssets no WP8
                // em seguida, salve em Application.persistentDataPath
                File.Copy(loadDb, caminho);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + NomeDoBanco;  // este é o caminho para seus StreamingAssets no WINDOWS
		// em seguida, salve em Application.persistentDataPath
		File.Copy(loadDb, caminho);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + NomeDoBanco;  // este é o caminho para seus StreamingAssets no OSX
		// em seguida, salve em Application.persistentDataPath
		File.Copy(loadDb, caminho);
#else
            var loadDb = Application.dataPath + "/StreamingAssets/" + NomeDoBanco;
            File.Copy(loadDb, caminho);
#endif
            Debug.Log("BASE DE DADOS CRIADA!");
        }
        var dbPath = caminho;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Caminho Final: " + dbPath);

    }

    //Funcao para criar as tabelas no banco.
    public void CriaBanco()
    {
        DropBanco();
        _connection.CreateTable<ElementalAPI>();
        _connection.CreateTable<Magica>();
        _connection.CreateTable<Monstro>();
        _connection.CreateTable<CartaAPI>();
        _connection.CreateTable<ElementoAPI>();
        _connection.CreateTable<ColecaoCartas>();
        _connection.CreateTable<ColecaoDeck>();
        //_connection.CreateTable<GeralColecao>();       
    }

    //Função para dropar o banco.
    public void DropBanco()
    {
        _connection.DropTable<CartaAPI>();
        _connection.DropTable<ElementalAPI>();
        _connection.DropTable<ElementoAPI>();
        _connection.DropTable<ColecaoCartas>();
        _connection.DropTable<ColecaoDeck>();
        _connection.DropTable<Magica>();
        _connection.DropTable<Monstro>();
        //_connection.DropTable<GeralColecao>();
    }

    //Comandos para inserir e atualizar as tabelas.
    #region Insert/Update
    //Insere ou atualiza a tabela de cartas
    public void InserirCarta(CartaAPI carta)
    {
        _connection.InsertOrReplace(carta);
    } 
    public void InserirCarta(Magica carta)
    {
        _connection.InsertOrReplace(carta);
    }
    //Insere ou atualiza a tabela de elemento
    public void InserirElemento(ElementoAPI elemento)
    {
        _connection.InsertOrReplace(elemento);
    }
    //Insere ou atualiza a tabela de elementais
    public void InserirElemental(ElementalAPI elemental)
    {
        _connection.InsertOrReplace(elemental);
    }
    //Insere a colecao de cartas
    public void InserirCartaColecao(ColecaoCartas colecao)
    {
        _connection.Insert(colecao);
    }
    //Insere as cartas no deck
    public void InserirCartaDeck(ColecaoDeck deck)
    {
        _connection.Insert(deck);
    }
    #endregion

    //Comandos para consultar as cartas.
    #region Get

    //Comando para monstar as cartas monstro em tela
    public void CartasMonstro()
    {
        var _inventario = new List<Monstro>();
        var elementais = ConsultarElementais().ToList();
        Debug.Log(elementais.Count());
        foreach (var x in elementais)
        {
            Monstro c = new Monstro();
            c.Id = x.idElemental;
            c.Nivel = x.nivel;
            c.Cristais = x.cristais;
            c.Ataque = x.ataque;
            c.Defesa = x.defesa;
            var cartaid = ConsultarCartasPeloId(x.idCarta);
            c.id_carta = x.idCarta;
            Debug.Log("Carta atual: " + cartaid.titulo);
            c.Nome = cartaid.titulo;
            c.Descricao = cartaid.descricao;
            c.id_elemento = x.idElemento;

            //var l = ImageManager.instance.LoadImages(cartaid.titulo + cartaid.idCarta);
            //c.Foto = ImageManager.instance.BytesToSprite(l);
            //var elementoID = ConsultarElementoPeloId(x.idElemento);
            //var j = ImageManager.instance.LoadImages(elementoID.nome + elementoID.idElemento);
            //c.Moldura = ImageManager.instance.BytesToSprite(j);
            c.TipoCarta = TipoCarta.Elemental;
            //cartasLista.Add(c);
            _connection.Insert(c);
        }

    }

    //Instrução para consultar as cartas Monstros.
    public List<Monstro> ConsultarCartasMonstro()
    {
        var monstros = _connection.Table<Monstro>();
        var ListMonstro = new List<Monstro>();
        foreach (var x in monstros)
        {
            var monstro = new Monstro();
            monstro.Id = x.Id;
            monstro.id_carta = x.id_carta;
            monstro.id_elemento = x.id_elemento;
            monstro.Nivel = x.Nivel;
            monstro.Nome = x.Nome;
            monstro.TipoCarta = x.TipoCarta;
            monstro.TipoElemental = x.TipoElemental;
            monstro.Ataque = x.Ataque;
            monstro.Cristais = x.Cristais;
            monstro.Defesa = x.Defesa;
            monstro.Descricao = x.Descricao;
            var l = ImageManager.instance.LoadImages(x.Nome + x.id_carta);
            monstro.Foto = ImageManager.instance.BytesToSprite(l);
            var elementoID = ConsultarElementoPeloId(x.id_elemento);
            var j = ImageManager.instance.LoadImages(elementoID.nome + elementoID.idElemento);
            monstro.Moldura = ImageManager.instance.BytesToSprite(j);
            ListMonstro.Add(monstro);
        }
        return ListMonstro;
    } //Instrução para consultar as cartas Monstros.
    public List<Magica> ConsultarCartasMagicas()
    {
        var magica = _connection.Table<Magica>();
        var ListMagica = new List<Magica>();
        foreach (var x in magica)
        {
            Magica m = new Magica();
            m.Id = x.Id;
            m.Descricao = x.Descricao;
            m.Nome = x.Nome;
            m.qtdTurno = x.qtdTurno;
            m.TipoCarta = x.TipoCarta;
            m.TipoEfeito = x.TipoEfeito;
            m.TipoMagica = x.TipoMagica;
            m.Foto = null;
            m.Moldura = null;
            ListMagica.Add(m);
        }
        return ListMagica;
    }

    //Instrução para consultar as cartas.
    public IEnumerable<CartaAPI> ConsultarCartas()
    {
        return _connection.Table<CartaAPI>();
    }

    //Instrução para consultar os elementos.
    public IEnumerable<ElementoAPI> ConsultarElementos()
    {
        return _connection.Table<ElementoAPI>();
    }
    //Instrução para consultar os elementais.
    public IEnumerable<ElementalAPI> ConsultarElementais()
    {
        return _connection.Table<ElementalAPI>();
    }
    //Instrução para consultar as cartas pelo id;
    public CartaAPI ConsultarCartasPeloId(int id)
    {
        return _connection.Table<CartaAPI>().Where(x => x.idCarta == id).FirstOrDefault();
    }
    //Instrução para consultar os elementos pelo id;
    public ElementoAPI ConsultarElementoPeloId(int id)
    {
        return _connection.Table<ElementoAPI>().Where(x => x.idElemento == id).FirstOrDefault();
    }
    //Instrução para consultar os elementais pelo id;
    public ElementalAPI ConsultarElementaisPeloId(int id)
    {
        return _connection.Table<ElementalAPI>().Where(x => x.idCarta == id).FirstOrDefault();
    }
    //Instrução para consultar a colecao de cartas pelo id;
    public ColecaoCartas ConsultarColecaoPeloId(int id)
    {
        return _connection.Table<ColecaoCartas>().Where(x => x.Id == id).FirstOrDefault();
    }

    #region Consulta Deck
    //Instrução para consultar o deck de cartas.
    public IEnumerable<ColecaoDeck> ConsultarDeck()
    {
        return _connection.Table<ColecaoDeck>();
    }
    //Instrução para consultar o deck pelo id;
    public ColecaoDeck ConsultarDeck(int id)
    {
        return _connection.Table<ColecaoDeck>().Where(x => x.IdCarta == id).FirstOrDefault();
    }
    //Instrução para consultar o deck pelo nome.
    public IEnumerable<ColecaoDeck> ConsultarDeck(string nome)
    {
        return _connection.Table<ColecaoDeck>().Where(x => x.TituloDeck == nome);
    }
    #endregion

    #region Filtros
    //Instrução para consultar a colecao de cartas.
    public IEnumerable<ColecaoCartas> ConsultarColecao()
    {
        return _connection.Table<ColecaoCartas>();
    }

    //Instrução para consultar a colecao de cartas de acordo com o tipo.
    public IEnumerable<ColecaoCartas> ConsultarColecao(TipoCarta tipo)
    {
        return _connection.Table<ColecaoCartas>().Where(x => x.TipoCarta == tipo);
    }

    #endregion

    #endregion

}
