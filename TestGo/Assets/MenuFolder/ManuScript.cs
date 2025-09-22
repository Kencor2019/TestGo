using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuScript : MonoBehaviour
{
    [SerializeField] private EmitionScript olhoBizarro;
    [SerializeField] private EmitionScript Bizarro;
    private float counter;
    private float limit = 0;

    // Start is called before the first frame update
    void Start()
    {
        olhoBizarro.Start();
        //Bizarro.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        olhosBizarros(0);
    }

    bool olhosBizarros(float antibug)
    {
        //Contador padrão
        counter += Time.deltaTime;
        if (counter < limit)
        {
            return false;
        }

        Vector3 pos = new Vector3(UnityEngine.Random.Range(-8.5f - antibug, 8.5f + antibug), UnityEngine.Random.Range(-5 - antibug, 5 + antibug), 0);

        foreach (GameObject bullet in olhoBizarro.getBalas())
        {
            if (Vector3.Distance(pos, bullet.transform.position) < 3.5f) return olhosBizarros(antibug += 0.05f);
        }
        //Esse acima garante q não vai ter nenhum em cima do outro e o antibug é pra caso o espaço da cam não seja bastante(evita stackoverflow)

        //"invoca" o fundinho
        olhoBizarro.instanciar(pos, Quaternion.identity);

        //faz a primeira carinha ter sorriso feio(feio pq ta mal desenhado) só q ficou feio então tirei
        //if (olhoBizarro.getMunicao() == 0) Bizarro.instanciar(pos - Vector3.up / 2, Quaternion.identity);

        //reseta o contador e calcula aleatoriamente quanto tempo vai ter o proximo pra aparecer
        counter = 0;
        limit = UnityEngine.Random.Range(0.6f, 1.7f);

        return true;

    }

    public void generalChangeScene(string sss)
    {
        GeneralScript.general.changeScene(sss);
    }
}
