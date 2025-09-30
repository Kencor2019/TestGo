using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class WorldGridScript : MonoBehaviour
{
    [SerializeField]private Vector3 worldSize;
    private Color[] _world;
    // Start is called before the first frame update
    void Start()
    {
        startMap();   
    }

    async Task startMap()
    {
        _world = new Color[(int)worldSize.x * (int)worldSize.y];

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
        _texture.SetPixels(0, 0, (int)worldSize.x, (int)worldSize.y, _world);

        _texture.Apply();

        //o sprite pra rendereziar nossa textura q é o nosso mapa
        Sprite spr = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), Vector2.zero, 1f);
        this.GetComponent<SpriteRenderer>().sprite = spr;
    }

    void getNoise()
    {
        for (int i = 0; i < worldSize.x; i++)
        {
            for (int d = 0; d < worldSize.y; d++)
            {
                float xCoord = ((float)i / 250f) * 5;
                float yCoord = ((float)d / 250f) * 5;

                float diggo = Mathf.PerlinNoise(xCoord, yCoord);

                Debug.Log(_world.Length + " : " + diggo);

                _world[i * (int)worldSize.y + d] = new Color(0.7411765f, 0.7176471f, 0.4196079f, 1f);

                //pontos altos = branco
                if (diggo > 0.7f)
                {
                    _world[i * (int)worldSize.y + d] = new Color(1f, 0.937255f, 0.8352942f, 1f);
                }
                else
                //pontos meio altos = marrom
                if (diggo > 0.65f)
                {
                    _world[i * (int)worldSize.y + d] = new Color(0.9568628f, 0.6431373f, 0.3764706f, 1f);
                }
                else
                //terreno normal = verde
                if (diggo > 0.40f)
                {
                    _world[i * (int)worldSize.y + d] = new Color(0.5960785f, 0.9843138f, 0.5960785f, 1f);
                }
                else
                //terreno baixo = amarelo
                if (diggo > 0.37f)
                {
                    _world[i * (int)worldSize.y + d] = new Color(0.9333334f, 0.909804f, 0.6666667f, 1f);
                }
                else
                //terreno megabaixo = azul
                if (diggo > 0.01f)
                {
                    _world[i * (int)worldSize.y + d] = new Color(0.5294118f, 0.8078432f, 0.9215687f, 1f);
                }


            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
