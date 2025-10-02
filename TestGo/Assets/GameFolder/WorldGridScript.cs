using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class WorldGridScript : MonoBehaviour
{
    [Header("Tem Rio?")]
    [SerializeField] private bool rioExist;

    [Header("Chances")]
    [SerializeField] private float chanceBiomas;       //a partir do 0.2
    [SerializeField] private float chanceMinerio;       //a partir do 0.82
    [SerializeField] private float chanceParede;        //a partir do 0.67
    [SerializeField] private float chanceFlor;          //a partir do 0.43
    [SerializeField] private float chanceRio;           //a partir do 0.09

    [Header("Tamanhos")]
    [SerializeField] private Vector3 worldSize;
    [SerializeField] private int pixelSize;
    private List<int> biomas;
    private List<int> _rio;
    private float[] _world;
    private Color[] _worldPixels;

    /*
    Enumerator Comentado o.O
    100 minerio 1;
    101 minerio 2;
    ...
    109 minerio 9;
    200 flor 1;
    ...
    203 flor 3;
    300 parede 1;
    ...
    304 parede 4;
    */
    // Start is called before the first frame update
    void Start()
    {
        startMap();
    }

    async Task startMap()
    {
        //inicializa as variaveis do mundo
        _world = new float[(int)worldSize.x * (int)worldSize.y];
        _worldPixels = new Color[(int)worldSize.x * (int)worldSize.y * pixelSize * pixelSize];
        biomas = new List<int>();
        _rio = null;

        //caso tenha rio
        if (rioExist)
        {
            _rio = new List<int>();
            _rio.Add(0); //iniciamos um valor inicial pro rio para auxiliar depois
        }

        //abre uma thread pra fazer os calculos do mundo
        await Task.Run(() =>
        {
            //prepara as cores pro mapa
            getNoise();
        });

        //textura q vai receber cada pixel do mapa (ela recebendo e depois o apply)
        Texture2D _texture = new Texture2D((int)worldSize.x, (int)worldSize.y, TextureFormat.RGBA32, false);

        // Desliga o filtro bilinear (usa point filtering)
        _texture.filterMode = FilterMode.Point;

        // Opcional: evita wrap (repetição da textura)
        _texture.wrapMode = TextureWrapMode.Clamp;

        //Recebe os pixels e da o apply
        _texture.SetPixels(0, 0, (int)worldSize.x, (int)worldSize.y, _worldPixels);

        _texture.Apply();

        //o sprite pra rendereziar nossa textura q é o nosso mapa
        Sprite spr = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), Vector2.zero, 1f);
        this.GetComponent<SpriteRenderer>().sprite = spr;
    }

    void getNoise()
    {

        if (_rio != null)
        {
            getRio();
        }

        var _random = new System.Random();
        float _randomValue = 0;

        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                _randomValue = (float)_random.NextDouble();

                //adcionamos os pontos onde o rio vai fluir
                if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio && rioExist)
                {
                    //se estiver muito perto vamos ignorar
                    if (Vector3.Distance(new Vector3(Mathf.Floor(_rio[_rio.Count - 1] / (int)worldSize.y), _rio[_rio.Count - 1] % (int)worldSize.x),
                     new Vector3(i, d)) < worldSize.x/1.1f) continue;

                    _rio.Add(i * (int)worldSize.y + d);
                    Debug.Log("rio+");
                }

                if (0.20f < _randomValue && _randomValue < 0.20f + chanceBiomas)
                {
                    Debug.Log("Bioma adiconado slk");
                    biomas.Add(i * (int)worldSize.y + d);
                }

                if (0.82f < _randomValue && _randomValue < 0.82f + chanceMinerio)
                {
                    _world[i * (int)worldSize.y + d] = 100;
                }

                if (0.67f < _randomValue && _randomValue < 0.67f + chanceParede)
                {
                    _world[i * (int)worldSize.y + d] = 300;
                }

                if (0.43f < _randomValue && _randomValue < 0.43f + chanceFlor)
                {
                    _world[i * (int)worldSize.y + d] = 200;
                }
            }
        }
        
        //Garante q tenha pelo menos 1 bioma
        if (biomas.Count == 0)
        {
            biomas.Add((int)(worldSize.y/2)*(int)worldSize.x + (int)worldSize.x/2);
        }


        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                float xCoord = ((float)i / 250f) * 5;
                float yCoord = ((float)d / 250f) * 5;

                float diggo = Mathf.PerlinNoise(xCoord, yCoord);

                //Debug.Log(_worldPixels.Length + " : " + (_world[i * (int)worldSize.y + d] + diggo));

                _worldPixels[i * (int)worldSize.y + d] = new Color(0.7411765f, 0.7176471f, 0.4196079f, 1f);

                //pontos altos = branco
                if (diggo > 0.7f)
                {
                    _worldPixels[i * (int)worldSize.y + d] = new Color(1f, 0.937255f, 0.8352942f, 1f);
                }
                else
                //pontos meio altos = marrom
                if (diggo > 0.65f)
                {
                    _worldPixels[i * (int)worldSize.y + d] = new Color(0.9568628f, 0.6431373f, 0.3764706f, 1f);
                }
                else
                //terreno normal = verde
                if (diggo > 0.40f)
                {
                    _worldPixels[i * (int)worldSize.y + d] = new Color(0.5960785f, 0.9843138f, 0.5960785f, 1f);
                }
                else
                //terreno baixo = amarelo
                if (diggo > 0.37f)
                {
                    _worldPixels[i * (int)worldSize.y + d] = new Color(0.9333334f, 0.909804f, 0.6666667f, 1f);
                }
                else
                //terreno megabaixo = azul
                if (diggo > 0.01f)
                {
                    _worldPixels[i * (int)worldSize.y + d] = new Color(0.5294118f, 0.8078432f, 0.9215687f, 1f);
                }


            }
        }

        if (rioExist)
        {
            //reordena o rio para o fluxo do rio(a lista que armazena o fluxo) seguir de forma linear
            int ordenaRio = _rio[1];
            for (int i = 1; i < _rio.Count - 1; i++)
            {
                _rio[i] = _rio[i + 1];
            }
            _rio[_rio.Count - 1] = ordenaRio;
        }
    }

    void getRio()
    {
        //adcionamos os pontos onde o rio vai começar e acabar
        var _random = new System.Random();
        float _randomValue = 0;
 
        for (int i = 1; i < (int)worldSize.x - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if (_rio[0] != 0 && Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.y, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3(i % (int)worldSize.y, Mathf.Floor(i / (int)worldSize.x))) < worldSize.x / 3) continue;

                _rio.Add(i);
            }
        }
        for (int i = 1; i < (int)worldSize.y - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if (_rio[0] != 0 && Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.y, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3((i * (int)worldSize.y) % (int)worldSize.y, Mathf.Floor((i * (int)worldSize.y) / (int)worldSize.x))) < worldSize.x / 3) continue;

                _rio.Add(i * (int)worldSize.x);
            }
        }
        for (int i = 1; i < (int)worldSize.y - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if (_rio[0] != 0 && Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.y, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3(((i + 1) * (int)worldSize.y - 1) % (int)worldSize.y, Mathf.Floor(i * (int)worldSize.y / (int)worldSize.x))) < worldSize.x / 3) continue;

                _rio.Add(((i + 1) * (int)worldSize.x - 1));
            }
        }
        for (int i = 1; i < (int)worldSize.x - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if (_rio[0] != 0 && Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.y, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3((i + ((int)worldSize.x - 1) * (int)worldSize.y) % (int)worldSize.y, Mathf.Floor((i + ((int)worldSize.x - 1) * (int)worldSize.y) / (int)worldSize.x))) < worldSize.x / 3) continue;

                _rio.Add((i + ((int)worldSize.x - 1) * (int)worldSize.y));
            }
        }

        //caso não tenha ido vamos chamar denovo
        if (_rio.Count < 2)
        {
            Debug.Log("Pra criar o rio eu fui chamado denovo");
            getRio();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}



