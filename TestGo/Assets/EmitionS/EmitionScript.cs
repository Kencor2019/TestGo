using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class EmitionScript : ScriptableObject
{

    [SerializeField] private int municaoMax;
    [SerializeField] private GameObject balaOrigem;
    private GameObject[] bala;
    private int municao;

    // Start is called before the first frame update
    public GameObject[] Start()
    {
        municao = 0;
        bala = new GameObject[municaoMax];
        for (int i = 0; i < municaoMax; i++)
        {
            bala[i] = Instantiate(balaOrigem);
            bala[i].SetActive(false);
        }

        return bala;
    }

    public GameObject instanciar()
    {
        bala[municao % municaoMax].SetActive(true);

        municao++;

        return bala[municao % municaoMax];
    }

    public GameObject instanciar(Vector3 position, Quaternion rotation)
    {
        bala[municao % municaoMax].SetActive(true);
        bala[municao % municaoMax].transform.position = position;
        bala[municao % municaoMax].transform.rotation = rotation;

        municao++;

        return bala[municao % municaoMax];
    }

    public GameObject[] getBalas()
    {
        return bala;
    }

    public int getMunicao()
    {
        return municao % municaoMax;
    }

    public int getMunicaoMax()
    {
        return municaoMax;
    }
    
}
