using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    Monstro[] monstroLista;

    public List<Monstro> _Deck { get; private set; }

    
    private void Awake()
    {
        _Deck = new List<Monstro>();
        _Deck = monstroLista.OrderBy(i => i.Nome).ToList();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(Monstro monstro)
    {
        if (monstro != null)
            _Deck.Add(monstro);

    }

    public void RemoveCard(Monstro monstro)
    {
        if (monstro != null)
            _Deck.Remove(monstro);
    }

}
