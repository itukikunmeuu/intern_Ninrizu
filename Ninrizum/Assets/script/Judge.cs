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
    [SerializeField] float endTime = 0;           // ゲーム終了時間
    [SerializeField] TextMeshProUGUI comboText;   // コンボ表示用（Inspectorでセット）

    private int combo = 0;

    void Update()
    {
        // カット判定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject nearest = null;
            float minDist = judgeRange;

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
                // カット成功
                meshCutter.CutCarrot(nearest);
                carrotManager.NotesObj.Remove(nearest);
                combo++;
                if (comboText != null) comboText.text = combo.ToString();
            }
            else
            {
                // カットミス
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
            }
        }

        // ノーツが判定ラインを通過したのにカットされなかった場合（ミス判定）
        // 例: 一番左のノーツが判定ラインを過ぎていたらミス
        if (carrotManager.NotesObj.Count > 0)
        {
            GameObject first = carrotManager.NotesObj[0];
            if (first != null && first.transform.position.x > judgeLine.position.x + judgeRange)
            {
                // ミスとしてコンボリセット
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
                // ノーツをリストから削除（必要に応じてDestroyも）
                carrotManager.NotesObj.RemoveAt(0);
           
            }
        }

        if (Time.time > endTime + GManager.instance.StartTime)
        {
            Invoke("ResultScene", 3f);
            return;
        }
    }
}
