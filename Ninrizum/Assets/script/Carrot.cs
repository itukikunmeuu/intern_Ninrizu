using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
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
            // ƒ[ƒ‹ƒhÀ•W‚Ìx²•ûŒü‚ÉˆÚ“®
            transform.position += Vector3.right * NoteSpeed * Time.deltaTime;
        }
    }
}
