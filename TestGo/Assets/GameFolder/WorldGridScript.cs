using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class WorldGridScript : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Color[][][] sprites;

    [Header("Info")]
    [SerializeField] private bool rioExist;
    [SerializeField] private int numBiomas;

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
    private int[] _world;
    private Color[] _worldPixels;
    private System.Random _random = new System.Random();

    /*
    Enumerator Comentado o.O
    1 bioma1;
    ...
    5 bioma5;
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
    1000 rio;
    */
    // Start is called before the first frame update
    void Start()
    {
        loadSprites();
        startMap();
    }

    async Task startMap()
    {
        //inicializa as variaveis do mundo
        _world = new int[(int)worldSize.x * (int)worldSize.y];
        _worldPixels = new Color[(int)worldSize.x * (int)worldSize.y * pixelSize * pixelSize];
        biomas = new List<int>();
        _rio = null;
        //Debug.Log(_rio.Count);
        //assim eu posso colocar um valor simples para cada chance (ex: chanceBioma = 3 media de 3 biomas por mapa)
        chanceBiomas /= (worldSize.x * worldSize.y);
        chanceMinerio /= (worldSize.x * worldSize.y);
        chanceParede /= (worldSize.x * worldSize.y);
        chanceFlor /= (worldSize.x * worldSize.y);
        chanceRio /= (worldSize.x * worldSize.y);
       

        //caso tenha rio
        if (rioExist)
        {
            _rio = new List<int>();
            //_rio.Add(0); //iniciamos um valor inicial pro rio para auxiliar depois
        }

        //abre uma thread pra fazer os calculos do mundo
        await Task.Run(() =>
        {
            try
            {
                calculateMapPixels();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro dentro da Task: {ex}");
            }
        });

        //textura q vai receber cada pixel do mapa (ela recebendo e depois o apply)
        Texture2D _texture = new Texture2D((int)worldSize.x * pixelSize, (int)worldSize.y * pixelSize, TextureFormat.RGBA32, false);

        // Desliga o filtro bilinear (usa point filtering)
        //_texture.filterMode = FilterMode.Point;

        // Opcional: evita wrap (repetição da textura)
        _texture.wrapMode = TextureWrapMode.Clamp;

        //Recebe os pixels e da o apply
        _texture.SetPixels(0, 0, (int)worldSize.x * pixelSize, (int)worldSize.y * pixelSize, _worldPixels);

        _texture.Apply();

        //o sprite pra rendereziar nossa textura q é o nosso mapa
        Sprite spr = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), Vector2.zero, 16f);
        this.GetComponent<SpriteRenderer>().sprite = spr;
    }

    void calculateMapPixels()
    {

        if (_rio != null)
        {
            getRio(0);
        }

        //gera os recursos do mapa
        mapRessources();

        //Garante q tenha pelo menos 1 bioma
        if (biomas.Count == 0)
        {
            biomas.Add((int)(worldSize.y / 2) * (int)worldSize.x + (int)worldSize.x / 2);
            _world[(int)(worldSize.y / 2) * (int)worldSize.x + (int)worldSize.x / 2] = 1;
        }

        //Aqui vamos pegar os pontos onde o Rio atravessa antes vamos reordenar o rio
        if (rioExist)
        {
            int ordenaRio = _rio[1];
            for (int i = 1; i < _rio.Count - 1; i++)
            {
                _rio[i] = _rio[i + 1];
            }
            _rio[_rio.Count - 1] = ordenaRio;
            rioMakerFluid();
        }

        GenerateRessources();

        //se tiver um bioma geramos um mapa sem preocupar com a distancia dos biomas
        if (biomas.Count == 1)
        {
            for (int i = 0; i < worldSize.x; i++)
            {
                for (int d = 0; d < worldSize.y; d++)
                {
                    //squareBioma guarda a posição do bioma mais proximo do nosso quadrado
                    int squareBioma = biomas[0];

                    getNoise(i, d, squareBioma);
                }
            }
        }//se tiver mais de um bioma geramos um mapa com a distancia dos biomas
        else
        {
            generateMap();
        }
    }

    void GenerateRessources()
    {
        int maxCluster = 9;
        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                if(_world[d * (int)worldSize.x + i] < 100)
                {
                    continue;
                }
                for (int j = 0; j < maxCluster; j++)
                {
                    for (int k = 0; k < maxCluster; k++)
                    {
                        if ((d + (k - ((maxCluster-1)/2))) * (int)worldSize.x + i + (j - ((maxCluster-1)/2)) < 0 || (d + (k - ((maxCluster-1)/2))) * (int)worldSize.x + i + (j - ((maxCluster-1)/2)) >= worldSize.x * worldSize.y)
                        {
                            continue;
                        }
                        //pra fazer o rio based
                        if (_world[d * (int)worldSize.x + i] == 1000)
                        {
                            if(_world[(d + (k - ((maxCluster-1)/2))) * (int)worldSize.x + i + (j - ((maxCluster-1)/2))] == 1000) continue;
                            _world[(d + (k - ((maxCluster-1)/2))) * (int)worldSize.x + i + (j - ((maxCluster-1)/2))] = 1001;
                        }

                        //pra dar cluster effect nos minerios, flores e paredes
                        if (_world[d * (int)worldSize.x + i] >= 300)
                        {

                        }
                        else
                        if (_world[d * (int)worldSize.x + i] >= 200)
                        {

                        }
                        else
                        if (_world[d * (int)worldSize.x + i] >= 100)
                        {

                        }
                    }
                }
            }
        }
    }

    void rioMakerFluid()
    {
        float interpolate = 0;
        for(int i = 0; i < _rio.Count - 1; i++)
        {
            float _distanc = Mathf.FloorToInt(Vector3.Distance(
                new Vector3(_rio[i] % (int)worldSize.x, Mathf.Floor(_rio[i] / (int)worldSize.x)),
                new Vector3(_rio[i+1] % (int)worldSize.x, Mathf.Floor(_rio[i+1] / (int)worldSize.x)))
                );

            int nextRio = i + 1;
            Vector3 _anchor = new Vector3(_rio[i] % (int)worldSize.x, Mathf.Floor(_rio[i] / (int)worldSize.x));
            Vector3 _anchor2 = new Vector3(_rio[i+1] % (int)worldSize.x, Mathf.Floor(_rio[i+1] / (int)worldSize.x));

            Vector3 _suport = new Vector3(_rio[i] % (int)worldSize.x + ((i%2 == 0)? + _distanc/10 : - _distanc/10), Mathf.Floor(_rio[i] / (int)worldSize.x + ((i%2 == 0)? - _distanc/10 : + _distanc/10)));
            Vector3 _suport2 = new Vector3(_rio[i+1] % (int)worldSize.x + ((i%2 == 0)? + _distanc/10 : - _distanc/10), Mathf.Floor(_rio[i+1] / (int)worldSize.x + ((i%2 == 0)? - _distanc/10 : + _distanc/10)));

            for (int d = 0; d <= _distanc; d++)
            {
                interpolate = (1f / _distanc) * d;
                Vector3 lerpSuper = LerpSuper(_anchor, _suport, _anchor2, _suport2, interpolate);

                //caso tenha pontos fora do grid
                if ((int)lerpSuper.y < 0 || (int)lerpSuper.y >= worldSize.y || (int)lerpSuper.x < 0 || (int)lerpSuper.x >= worldSize.x)
                {
                    continue;
                }

                _world[(int)lerpSuper.y * (int)worldSize.x + (int)lerpSuper.x] = 1000;
               
                Debug.DrawLine(lerpSuper, LerpSuper(_anchor, _suport, _anchor2, _suport2, interpolate + 1 / _distanc));
            }
        }
    }


    void mapRessources()
    {
        //valor aleatorio para geração do mapa
        float _randomValue = 0;

        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                //bordas de paredes
                if (i == 0 || d == 0 || i == worldSize.x - 1 || d == worldSize.y - 1)
                {
                    _world[d * (int)worldSize.x + i] = 300;
                    continue;
                }

                _randomValue = (float)_random.NextDouble();

                //adcionamos os pontos onde o rio vai fluir
                if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio && rioExist)
                {
                    //se estiver muito perto de um ponto já existente vamos ignorar
                    bool devoIgnorar = false;
                    foreach (int rio in _rio)
                    {
                        if (Vector3.Distance(new Vector3(rio % (int)worldSize.x, Mathf.Floor(rio / (int)worldSize.x)),
                            new Vector3(i, d)) < (worldSize.x + worldSize.y) / 5)
                        {
                            devoIgnorar = true;
                            break;
                        }
                    }

                    if (!devoIgnorar)
                    {
                        _rio.Add(d * (int)worldSize.x + i);
                    }

                    continue;
                }

                //verifica se nasce um bioma nesse bloco
                if (0.20f < _randomValue && _randomValue < 0.20f + chanceBiomas)
                {
                    Debug.Log("Added Bioma");
                    biomas.Add(d * (int)worldSize.x + i);
                    _world[d * (int)worldSize.x + i] = 1;
                    continue;
                }

                //verifica se nasce minerio nesse bloco
                if (0.82f < _randomValue && _randomValue < 0.82f + chanceMinerio)
                {
                    _world[d * (int)worldSize.x + i] = 100;
                    continue;
                }

                //verifica se nasce uma parede nesse bloco
                if (0.67f < _randomValue && _randomValue < 0.67f + chanceParede)
                {
                    _world[d * (int)worldSize.x + i] = 300;
                    continue;
                }

                //verifica se nasce enfeites nesse bloco
                if (0.43f < _randomValue && _randomValue < 0.43f + chanceFlor)
                {
                    _world[d * (int)worldSize.x + i] = 200;
                    continue;
                }
            }
        }
    }

    void generateMap()
    {
        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                if (_world[d * (int)worldSize.x + i] == 1001)
                {
                    for (int j = 0; j < pixelSize; j++)
                    {
                        for (int h = 0; h < pixelSize; h++)
                        {
                            _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                            Color.cyan;
                        }
                    }
                    continue;
                }
                if (_world[d * (int)worldSize.x + i] >= 1000 && _world[d * (int)worldSize.x + i] < 2000)
                {
                    for (int j = 0; j < pixelSize; j++)
                    {
                        for (int h = 0; h < pixelSize; h++)
                        {
                            _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                            Color.blue;
                        }
                    }
                    continue;
                }
                //squareBioma guarda a posição do bioma mais proximo do nosso quadrado
                int squareBioma = biomas[0];
                foreach (int bioma in biomas)
                {
                    if (Vector3.Distance(new Vector3(bioma % (int)worldSize.x, Mathf.Floor(bioma / (int)worldSize.x)),
                     new Vector3(i, d)) < Vector3.Distance(new Vector3(squareBioma % (int)worldSize.x, Mathf.Floor(squareBioma / (int)worldSize.x)),
                     new Vector3(i, d))) squareBioma = bioma;
                }

                getNoise(i, d, squareBioma);
            }
        }
    }

    void loadSprites()
    {
        sprites = new Color[numBiomas][][];
        for (int i = 0; i < numBiomas; i++)
        {
            sprites[i] = new Color[_sprites.Length][];
            for (int d = 0; d < _sprites.Length; d++)
            {
                sprites[i][d] = _sprites[d].texture.GetPixels();
            }
        }
    }

    void getNoise(int i, int d, int squareBioma)
    {
        float xCoord = ((float)i / 325f) * 18;
        float yCoord = ((float)d / 325f) * 18;

        float diggo = Mathf.PerlinNoise(xCoord, yCoord);

        if (diggo > 0.70f)
        {
            for (int j = 0; j < pixelSize; j++)
            {
                for (int h = 0; h < pixelSize; h++)
                {
                    _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                    sprites[_world[squareBioma] - 1][0][h * pixelSize + j];
                }
            }
        }
        else
         if (diggo > 0.50f)
        {
            for (int j = 0; j < pixelSize; j++)
            {
                for (int h = 0; h < pixelSize; h++)
                {
                    _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                    sprites[_world[squareBioma] - 1][1][h * pixelSize + j];
                }
            }
        }
        else
         if (diggo > 0.45f)
        {
            for (int j = 0; j < pixelSize; j++)
            {
                for (int h = 0; h < pixelSize; h++)
                {
                    _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                    sprites[_world[squareBioma] - 1][2][h * pixelSize + j];
                }
            }
        }
        else
        {
            for (int j = 0; j < pixelSize; j++)
            {
                for (int h = 0; h < pixelSize; h++)
                {
                    _worldPixels[d * (int)worldSize.x * pixelSize * pixelSize + i * pixelSize + (h * (int)worldSize.x * pixelSize + j)] =
                    sprites[_world[squareBioma] - 1][3][h * pixelSize + j];
                }
            }
        }

    }

    void getRio(float antiGargalo)
    {
        //adcionamos os pontos onde o rio vai começar e acabar
        float _randomValue = 0;

        for (int i = 1; i < (int)worldSize.x - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio + antiGargalo)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if(_rio.Count > 0)
                    if (Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.x, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3(i % (int)worldSize.x, Mathf.Floor(i / (int)worldSize.x))) < (worldSize.x + worldSize.y) / 2) continue;

                _rio.Add(i);
            }
        }
        for (int i = 1; i < (int)worldSize.y - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio + antiGargalo)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if(_rio.Count > 0)
                    if (Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.x, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3((i * (int)worldSize.y) % (int)worldSize.x, Mathf.Floor((i * (int)worldSize.y) / (int)worldSize.x))) < (worldSize.x + worldSize.y) / 2) continue;

                _rio.Add(i * (int)worldSize.x);
            }
        }
        for (int i = 1; i < (int)worldSize.y - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio + antiGargalo)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if(_rio.Count > 0)
                    if (Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.x, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3(((i + 1) * (int)worldSize.y - 1) % (int)worldSize.x, Mathf.Floor(i * (int)worldSize.y / (int)worldSize.x))) < (worldSize.x + worldSize.y) / 2) continue;

                _rio.Add(((i + 1) * (int)worldSize.x - 1));
            }
        }
        for (int i = 1; i < (int)worldSize.x - 1; i++)
        {
            if (_rio.Count >= 2) { return; }

            _randomValue = (float)_random.NextDouble();

            if (0.09f < _randomValue && _randomValue < 0.09f + chanceRio + antiGargalo)
            {
                //se o primeiro ponto tiver nascido e este estiver muito perto não vamos ignorar
                if(_rio.Count > 0)
                    if (Vector3.Distance(new Vector3(_rio[0] % (int)worldSize.x, Mathf.Floor(_rio[0] / (int)worldSize.x)),
                 new Vector3((i + ((int)worldSize.x - 1) * (int)worldSize.y) % (int)worldSize.x, Mathf.Floor((i + ((int)worldSize.x - 1) * (int)worldSize.y) / (int)worldSize.x))) < (worldSize.x + worldSize.y) / 2) continue;

                _rio.Add((i + ((int)worldSize.x - 1) * (int)worldSize.y));
            }
        }

        //caso não tenha ido vamos chamar denovo com uma chance maior
        if (_rio.Count < 2)
        {
            getRio(antiGargalo * 2 + chanceRio * 2);
        }

    }
   
    Vector3 LerpSuper(Vector3 a1, Vector3 s1, Vector3 a2, Vector3 s2, float t)
    {
        // Interpola os dois lados da curva
        Vector3 side1 = Vector3.Lerp(
            Vector3.Lerp(a1, s1, t),
            Vector3.Lerp(s1, s2, t),
            t
        );

        Vector3 side2 = Vector3.Lerp(
            Vector3.Lerp(s1, s2, t),
            Vector3.Lerp(s2, a2, t),
            t
        );

        // Combina os dois lados num resultado final
        return Vector3.Lerp(side1, side2, t);
    }

    // Update is called once per frame
    void Update()
    {
           
       
    }
}



/*
                if (_world[d * (int)worldSize.x + i] < 100)
                {
                    _worldPixels[d * (int)worldSize.x + i] = Color.black;
                }
                else
                if (_world[d * (int)worldSize.x + i] < 200)
                {
                    _worldPixels[d * (int)worldSize.x + i] = Color.green;
                }
                else
                if (_world[d * (int)worldSize.x + i] < 300)
                {
                    _worldPixels[d * (int)worldSize.x + i] = Color.cyan;
                }
                else
                if (_world[d * (int)worldSize.x + i] < 400)
                {
                    _worldPixels[d * (int)worldSize.x + i] = Color.yellow;
                }

                Debug.Log(_world[d * (int)worldSize.x + i]);
                */
