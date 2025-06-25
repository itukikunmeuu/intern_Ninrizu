using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    //ノーツのスピードを設定
    float NoteSpeed = 8;
    bool start;

    void Start()
    {
        NoteSpeed = 8;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            start = true;
        }
        if (start)
        {
            transform.position += transform.right * Time.deltaTime * NoteSpeed;
        }
    }
    
}
