using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mazegennew : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();
    public int size;
    public float offset;
    void Start()
    {
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Instantiate(prefabs[Random.Range(0, prefabs.Count)], new Vector3(x * offset, 0, z * offset), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
