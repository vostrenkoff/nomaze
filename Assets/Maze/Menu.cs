using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] public GameObject loads, Desert, Winter;
    public static bool biome = true;
    static int i = 1;

    public void NextScene()
    {
        i = 0;
        loads.SetActive(true);
        SceneManager.LoadScene("Maze");
    }
    public void BackScene()
    {

        SceneManager.LoadScene("Menu");
    }
    public void Update()
    {
        if (i > 0)
        {
            if (biome)
            {
                Desert.SetActive(true);
                Winter.SetActive(false);
            }
            else
            {
                Desert.SetActive(false);
                Winter.SetActive(true);
            }
        }
    }
    public void ChangeBiome()
    {
       if(biome)
        {
            biome = false;
            Debug.Log("false");
        }
       else
        {
            biome = true;
            Debug.Log("true");
        }
    }
}