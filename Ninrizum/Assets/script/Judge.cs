using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Judge : MonoBehaviour
{
    [SerializeField] CarrotManager carrotManager;
    [SerializeField] Transform judgeLine;
    [SerializeField] MeshCutter meshCutter;
    [SerializeField] float judgeRange = 0.5f;
    [SerializeField] float endTime = 0;
    [SerializeField] TextMeshProUGUI comboText;

    [SerializeField] AudioClip cutSE; // 効果音（Inspectorでセット）
    private AudioSource audioSource;

    private int combo = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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
                meshCutter.CutCarrot(nearest);
                carrotManager.NotesObj.Remove(nearest);
                combo++;
                if (comboText != null) comboText.text = combo.ToString();
            }
            else
            {
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
            }
            // 効果音を再生
            if (cutSE != null)
            {
                audioSource.PlayOneShot(cutSE);
            }
        }

        if (carrotManager.NotesObj.Count > 0)
        {
            GameObject first = carrotManager.NotesObj[0];
            if (first != null && first.transform.position.x > judgeLine.position.x + judgeRange)
            {
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
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
