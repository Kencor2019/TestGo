using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class TechTree : MonoBehaviour
{
    public static TechTree arvore;

    public List<techNode> nodes = new List<techNode>();
    [SerializeField] private GameObject butao;
    private GameObject arvorizador;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        arvore = this;
        makeTree();

        arvorizador = Instantiate(butao);
        arvorizador.GetComponentInChildren<TextMeshProUGUI>().SetText("opa");

        foreach (techNode op in nodes)
        {
            if (op.getParent() != null) continue;
           
            GameObject obrigado = Instantiate(butao);
            obrigado.GetComponentInChildren<TextMeshProUGUI>().SetText(op.getName());

            foreach (techNode opp in op.getKids())
            {
                GameObject desobrigado = Instantiate(butao, obrigado.transform);
                desobrigado.GetComponentInChildren<TextMeshProUGUI>().SetText(opp.getName());

                foreach (techNode oppp in opp.getKids())
                {
                    GameObject final = Instantiate(butao, desobrigado.transform);
                    final.GetComponentInChildren<TextMeshProUGUI>().SetText(oppp.getName());
                   
                    foreach (techNode opppp in oppp.getKids())
                    {
                        GameObject daaa = Instantiate(butao, final.transform);
                        daaa.GetComponentInChildren<TextMeshProUGUI>().SetText(opppp.getName());

                    }
                }
            }

        }
    }

    /*
    foreach (TextMeshProUGUI ov in arvorizador.GetComponentsInChildren<TextMeshProUGUI>())
        {
            ov.SetText("Opa");
        }
        */

    void makeTree()
    {
        techNode[] _nodes = new techNode[(1 * 3 * 3 * 2) + (1 * 3 * 3) + (1 * 3) + 1];
        _nodes[0] = new techNode("coreNode", "core", 0, 0, 3);//, null, getnodes(1, 3, _nodes)); //1
        _nodes[1] = new techNode("broca", "brocas", 1, 1, 3);//, getnodes(0, 0, _nodes), getnodes(4, 6, _nodes));  //2
        _nodes[2] = new techNode("torre", "torres", 2, 1, 3);//, getnodes(0, 0, _nodes), getnodes(7, 9, _nodes));
        _nodes[3] = new techNode("guerreiro1", "unidades", 3, 1, 3);//, getnodes(0, 0, _nodes), getnodes(10, 12, _nodes));
        _nodes[4] = new techNode("broca forte", "brocas", 4, 1, 2);//, getnodes(1, 1, _nodes), getnodes(13, 14, _nodes));  //3
        _nodes[5] = new techNode("extrator de liquido", "brocas", 5, 1, 2);//, getnodes(1, 1, _nodes), getnodes(15, 16, _nodes));
        _nodes[6] = new techNode("filtro de gas", "brocas", 6, 1, 2);//, getnodes(1, 1, _nodes), getnodes(17, 18, _nodes));
        _nodes[7] = new techNode("antiaerea", "torres", 7, 1, 2);//, getnodes(2, 2, _nodes), getnodes(19, 20, _nodes));
        _nodes[8] = new techNode("armaLaser", "torres", 8, 1, 2);//, getnodes(2, 2, _nodes), getnodes(21, 22, _nodes));
        _nodes[9] = new techNode("torreSniper", "torres", 9, 1, 2);//, getnodes(2, 2, _nodes), getnodes(23, 24, _nodes));
        _nodes[10] = new techNode("mosca1", "unidades", 10, 1, 2);//, getnodes(3, 3, _nodes), getnodes(25, 26, _nodes));
        _nodes[11] = new techNode("navio1", "unidades", 11, 1, 2);//, getnodes(3, 3, _nodes), getnodes(27, 28, _nodes));
        _nodes[12] = new techNode("curandeiro1", "unidades", 12, 1, 2);//, getnodes(3, 3, _nodes), getnodes(29, 30, _nodes));
        _nodes[13] = new techNode("MegaBroca", "brocas", 13, 1, 0);//, getnodes(4, 4, _nodes), null);  // 4
        _nodes[14] = new techNode("Acelerador", "utilidade", 14, 1, 0);//, getnodes(4, 4, _nodes), null);
        _nodes[15] = new techNode("MegaExtrator", "brocas", 15, 1, 0);//, getnodes(5, 5, _nodes), null);
        _nodes[16] = new techNode("Desaquecedor", "utilidade", 16, 1, 0);//, getnodes(5, 5, _nodes), null);
        _nodes[17] = new techNode("MegaFiltro", "brocas", 17, 1, 0);//, getnodes(6, 6, _nodes), null);
        _nodes[18] = new techNode("Unidade transporte", "utilidade", 18, 1, 0);//, getnodes(6, 6, _nodes), null);
        _nodes[19] = new techNode("Metralhadora", "torres", 19, 1, 0);//, getnodes(7, 7, _nodes), null);
        _nodes[20] = new techNode("Muro-Ponte", "defesa", 20, 1, 0);//, getnodes(7, 7, _nodes), null);
        _nodes[21] = new techNode("Evaporador", "torres", 21, 1, 0);//, getnodes(8, 8, _nodes), null);
        _nodes[22] = new techNode("Campo de Forca", "defesa", 22, 1, 0);//, getnodes(8, 8, _nodes), null);
        _nodes[23] = new techNode("Artilharia", "torres", 23, 1, 0);//, getnodes(9, 9, _nodes), null);
        _nodes[24] = new techNode("Muro de Plasma", "defesa", 24, 1, 0);//, getnodes(9, 9, _nodes), null);
        _nodes[25] = new techNode("mosca2", "unidade", 25, 1, 0);//, getnodes(10, 10, _nodes), null);
        _nodes[26] = new techNode("evoludor", "unidade", 26, 1, 0);//, getnodes(10, 10, _nodes), null);
        _nodes[27] = new techNode("navio2", "unidade", 27, 1, 0);//, getnodes(11, 11, _nodes), null);
        _nodes[28] = new techNode("evoludor2", "unidade", 28, 1, 0);//, getnodes(11, 11, _nodes), null);
        _nodes[29] = new techNode("cura2", "unidade", 29, 1, 0);//, getnodes(12, 12, _nodes), null);
        _nodes[30] = new techNode("evoludor3", "unidade", 30, 1, 0);//, getnodes(12, 12, _nodes), null);

        _nodes[0].setParents(null); //1
        _nodes[1].setParents(getnodes(0, 0, _nodes));  //2
        _nodes[2].setParents(getnodes(0, 0, _nodes));
        _nodes[3].setParents(getnodes(0, 0, _nodes));
        _nodes[4].setParents(getnodes(1, 1, _nodes));  //3
        _nodes[5].setParents(getnodes(1, 1, _nodes));
        _nodes[6].setParents(getnodes(1, 1, _nodes));
        _nodes[7].setParents(getnodes(2, 2, _nodes));
        _nodes[8].setParents(getnodes(2, 2, _nodes));
        _nodes[9].setParents(getnodes(2, 2, _nodes));
        _nodes[10].setParents(getnodes(3, 3, _nodes));
        _nodes[11].setParents(getnodes(3, 3, _nodes));
        _nodes[12].setParents(getnodes(3, 3, _nodes));
        _nodes[13].setParents(getnodes(4, 4, _nodes));  // 4
        _nodes[14].setParents(getnodes(4, 4, _nodes));
        _nodes[15].setParents(getnodes(5, 5, _nodes));
        _nodes[16].setParents(getnodes(5, 5, _nodes));
        _nodes[17].setParents(getnodes(6, 6, _nodes));
        _nodes[18].setParents(getnodes(6, 6, _nodes));
        _nodes[19].setParents(getnodes(7, 7, _nodes));
        _nodes[20].setParents(getnodes(7, 7, _nodes));
        _nodes[21].setParents(getnodes(8, 8, _nodes));
        _nodes[22].setParents(getnodes(8, 8, _nodes));
        _nodes[23].setParents(getnodes(9, 9, _nodes));
        _nodes[24].setParents(getnodes(9, 9, _nodes));
        _nodes[25].setParents(getnodes(10, 10, _nodes));
        _nodes[26].setParents(getnodes(10, 10, _nodes));
        _nodes[27].setParents(getnodes(11, 11, _nodes));
        _nodes[28].setParents(getnodes(11, 11, _nodes));
        _nodes[29].setParents(getnodes(12, 12, _nodes));
        _nodes[30].setParents(getnodes(12, 12, _nodes));

        _nodes[0].setKids(getnodes(1, 3, _nodes)); //1
        _nodes[1].setKids(getnodes(4, 6, _nodes));  //2
        _nodes[2].setKids(getnodes(7, 9, _nodes));
        _nodes[3].setKids(getnodes(10, 12, _nodes));
        _nodes[4].setKids(getnodes(13, 14, _nodes));  //3
        _nodes[5].setKids(getnodes(15, 16, _nodes));
        _nodes[6].setKids(getnodes(17, 18, _nodes));
        _nodes[7].setKids(getnodes(19, 20, _nodes));
        _nodes[8].setKids(getnodes(21, 22, _nodes));
        _nodes[9].setKids(getnodes(23, 24, _nodes));
        _nodes[10].setKids(getnodes(25, 26, _nodes));
        _nodes[11].setKids(getnodes(27, 28, _nodes));
        _nodes[12].setKids(getnodes(29, 30, _nodes));
        _nodes[13].setKids(null);  // 4
        _nodes[14].setKids(null);
        _nodes[15].setKids(null);
        _nodes[16].setKids(null);
        _nodes[17].setKids(null);
        _nodes[18].setKids(null);
        _nodes[19].setKids(null);
        _nodes[20].setKids(null);
        _nodes[21].setKids(null);
        _nodes[22].setKids(null);
        _nodes[23].setKids(null);
        _nodes[24].setKids(null);
        _nodes[25].setKids(null);
        _nodes[26].setKids(null);
        _nodes[27].setKids(null);
        _nodes[28].setKids(null);
        _nodes[29].setKids(null);
        _nodes[30].setKids(null);



        for (int i = 0; i <= 30; i++)
        {
            nodes.Add(_nodes[i]);
        }
       
    }

    List<techNode> getnodes(int op, int op2, techNode[] reference)
    {
        List<techNode> minhaLista = new List<techNode>();

        for (int i = op; i <= op2; i++)
        {
            minhaLista.Add(reference[i]);
        }

        return minhaLista;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}




public class techNode
{
    [SerializeField]private string name;
    [SerializeField]private string tipe;
    [SerializeField]private int nodeid;
    [SerializeField]private int numParents { get; set; }
    [SerializeField]private int numKids;
    [SerializeField] private List<techNode> parents;
    [SerializeField]private List<techNode> kids;

    public techNode(string a1, string a2, int a3, int a4, int a5)
    {
        name = a1;
        tipe = a2;
        nodeid = a3;
        numParents = a4;
        numKids = a5;
    }

    //name get set
    public string getName()
    {
        return name;
    }

    public void setName(string setter) {
        name = setter;
    }
   
    //tipe get set
    public void setTipe(string setter) {
        tipe = setter;
    }

    public string getTipe() {
        return tipe;
    }

    //id get set
    public void setId(int setter) {
        nodeid = setter;
    }
   
    public int getId() {
        return nodeid;
    }

    //parent get set
    public List<techNode> getParent() {
        return parents;
    }
   
    public void setParents(List<techNode> setter)
    {
        parents = setter;
    }

    //kids get set
    public List<techNode> getKids() {
        return kids;
    }

    public void setKids(List<techNode> setter) {
        kids = setter;
    }
}
