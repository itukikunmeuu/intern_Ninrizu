using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Spaceキーが押されるたびに"Cut"トリガーを発火
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Cut");
        }
    }
}
