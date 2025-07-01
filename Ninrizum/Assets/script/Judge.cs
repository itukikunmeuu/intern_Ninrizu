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
    [SerializeField] float endTime = 0;           // �Q�[���I������
    [SerializeField] TextMeshProUGUI comboText;   // �R���{�\���p�iInspector�ŃZ�b�g�j

    private int combo = 0;

    void Update()
    {
        // �J�b�g����
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
                // �J�b�g����
                meshCutter.CutCarrot(nearest);
                carrotManager.NotesObj.Remove(nearest);
                combo++;
                if (comboText != null) comboText.text = combo.ToString();
            }
            else
            {
                // �J�b�g�~�X
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
            }
        }

        // �m�[�c�����胉�C����ʉ߂����̂ɃJ�b�g����Ȃ������ꍇ�i�~�X����j
        // ��: ��ԍ��̃m�[�c�����胉�C�����߂��Ă�����~�X
        if (carrotManager.NotesObj.Count > 0)
        {
            GameObject first = carrotManager.NotesObj[0];
            if (first != null && first.transform.position.x > judgeLine.position.x + judgeRange)
            {
                // �~�X�Ƃ��ăR���{���Z�b�g
                combo = 0;
                if (comboText != null) comboText.text = combo.ToString();
                // �m�[�c�����X�g����폜�i�K�v�ɉ�����Destroy���j
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
