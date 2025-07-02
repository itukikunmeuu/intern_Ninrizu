using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

[Serializable]
public class Data
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;
}
[Serializable]
public class Note
{
    public int type;
    public int num;
    public int block;
    public int LPB;
}
public class CarrotManager : MonoBehaviour
{
    public int noteNum;
    private string songName;

    public List<int> LaneNum = new List<int>();
    public List<int> NoteType = new List<int>();
    public List<float> NotesTime = new List<float>();
    public List<GameObject> NotesObj = new List<GameObject>();

    [SerializeField] private float NotesSpeed;
    [SerializeField] GameObject noteObj;

    void OnEnable()
    {
        noteNum = 0;
        songName = "Raspberry-Adventure";

        Load(songName);
    }

    private void Load(string SongName)
    {
        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSec = kankaku * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            // 横から流す: x座標を進行方向、z座標をレーン位置に
            float x = -NotesTime[i] * NotesSpeed; // 左から右へ流す場合
            float z = inputJson.notes[i].block - 1.5f; // レーン位置
            Quaternion rot = Quaternion.Euler(0f, -90f, 0f); // X:90°, Y:-90°, Z:0°
            NotesObj.Add(Instantiate(noteObj, new Vector3(x, 0.55f, z), rot));
        }
    }
}
