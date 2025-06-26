using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Judge : MonoBehaviour
{
    [SerializeField] CarrotManager carrotManager; // CarrotManager�Q��
    [SerializeField] Transform judgeLine;         // ���胉�C��
    [SerializeField] MeshCutter meshCutter;       // MeshCutter�Q��
    [SerializeField] float judgeRange = 0.5f;     // ���苖�e�͈�

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject nearest = null;
            float minDist = judgeRange;

            // CarrotManager�ŊǗ����Ă���ɂ񂶂񃊃X�g���Q��
            foreach (var carrot in carrotManager.NotesObj)
            {
                if (carrot == null) continue;
                float dist = Mathf.Abs(carrot.transform.position.x - judgeLine.position.x);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = carrot;
                }
            }

            if (nearest != null)
            {
                // �J�b�g
                meshCutter.CutCarrot(nearest);
                carrotManager.NotesObj.Remove(nearest);
            }
        }
    }
}
