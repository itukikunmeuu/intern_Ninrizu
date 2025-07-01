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
        // Space�L�[��������邽�т�"Cut"�g���K�[�𔭉�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Cut");
        }
    }
}
