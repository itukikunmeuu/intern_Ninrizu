using UnityEngine;

public class MeshCutterController : MonoBehaviour
{
    [SerializeField] GameObject objectA;
    [SerializeField] GameObject objectB;
    [SerializeField] Material cutMaterial; // �J�b�g��̒f�ʂɎg���}�e���A��

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // B�̒��S�_�Ə�����ŃJ�b�g
            Vector3 cutPoint = objectB.transform.position;
            Vector3 cutNormal = objectB.transform.up; // ��: ������ŃJ�b�g

            // MeshCutter�̐ÓI���\�b�h���Ăяo��
        }   
    }
}
