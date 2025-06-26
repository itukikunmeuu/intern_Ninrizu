using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Judge : MonoBehaviour
{
    [SerializeField] CarrotManager carrotManager; // CarrotManager参照
    [SerializeField] Transform judgeLine;         // 判定ライン
    [SerializeField] MeshCutter meshCutter;       // MeshCutter参照
    [SerializeField] float judgeRange = 0.5f;     // 判定許容範囲

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject nearest = null;
            float minDist = judgeRange;

            // CarrotManagerで管理しているにんじんリストを参照
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
                // カット
                meshCutter.CutCarrot(nearest);
                carrotManager.NotesObj.Remove(nearest);
            }
        }
    }
}
