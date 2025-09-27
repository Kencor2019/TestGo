using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridScript : MonoBehaviour
{
    [SerializeField]private Vector3 worldSize;
    private int[][] world;
    // Start is called before the first frame update
    void Start()
    {
        world = new int[(int)worldSize.x][];
        for (int i = 0; i < worldSize.x; i++)
        {
            world[i] = new int[(int)worldSize.y];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
