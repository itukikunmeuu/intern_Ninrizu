using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Judge : MonoBehaviour
{
    [SerializeField] private GameObject[] MessageObj;
    [SerializeField] CarrotManager carrotManager;

    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject finish;

    AudioSource audio;
    [SerializeField] AudioClip hitSound;

    float endTime = 0;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        endTime = carrotManager.NotesTime[carrotManager.NotesTime.Count - 1];
    }
    void Update()
    {
        if (GManager.instance.Start)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Judgement(GetABS(Time.time - (carrotManager.NotesTime[0] + GManager.instance.StartTime)), 0);
            }

            if (Time.time > endTime + GManager.instance.StartTime)
            {
                finish.SetActive(true);
                Invoke("ResultScene", 3f);
                return;
            }

            if (Time.time > carrotManager.NotesTime[0] + 0.2f + GManager.instance.StartTime)
            {
                message(3);
                deleteData(0);
                Debug.Log("Miss");
                GManager.instance.miss++;
                GManager.instance.combo = 0;
            }
        }
    }
    void Judgement(float timeLag, int numOffset)
    {
        audio.PlayOneShot(hitSound);
        if (timeLag <= 0.05)
        {
            Debug.Log("Perfect");
            message(0);
            GManager.instance.ratioScore += 5;
            GManager.instance.perfect++;
            GManager.instance.combo++;
            deleteData(numOffset);
        }
        else
        {
            if (timeLag <= 0.08)
            {
                Debug.Log("Great");
                message(1);
                GManager.instance.ratioScore += 3;
                GManager.instance.great++;
                GManager.instance.combo++;
                deleteData(numOffset);
            }
            else
            {
                if (timeLag <= 0.10)
                {
                    Debug.Log("Bad");
                    message(2);
                    GManager.instance.ratioScore += 1;
                    GManager.instance.bad++;
                    GManager.instance.combo = 0;
                    deleteData(numOffset);
                }
            }
        }
    }
    float GetABS(float num)
    {
        if (num >= 0)
        {
            return num;
        }
        else
        {
            return -num;
        }
    }
    void deleteData(int numOffset)
    {
        carrotManager.NotesTime.RemoveAt(numOffset);
        carrotManager.LaneNum.RemoveAt(numOffset);
        carrotManager.NoteType.RemoveAt(numOffset);
        GManager.instance.score = (int)Math.Round(1000000 * Math.Floor(GManager.instance.ratioScore / GManager.instance.maxScore * 1000000) / 1000000);
        comboText.text = GManager.instance.combo.ToString();
        scoreText.text = GManager.instance.score.ToString();
    }

    void message(int judge)
    {
        Instantiate(MessageObj[judge], new Vector3(carrotManager.LaneNum[0] - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }

    void ResultScene()
    {
        SceneManager.LoadScene("Result");
    }
}
