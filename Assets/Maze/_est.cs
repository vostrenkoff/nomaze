using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private int _huj;
    private delegate void MethodHandler(int x, int y);
    private void Start()
    {
        MethodHandler mh;
        mh = (int x, int y) =>
        {
            int z = x + y;
        };
    }
    private IEnumerator Timer(float time, MethodHandler method)
    {
        yield return new WaitForSeconds(time);
    }
    private void SayHello()
    {

    }
    private void SayHuj()
    {

    }
}
