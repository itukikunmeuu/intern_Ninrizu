using UnityEngine;
using System.Collections.Generic;
using EzySlice; 

public class MeshCutter : MonoBehaviour
{
    [SerializeField] Material cutMaterial;
    [SerializeField] float separateDistance = 0.1f;
    [SerializeField] float forcePower = 3f; // �f�Ђɗ^����͂̑傫��

    // �J�b�g�������O������Ăяo����悤��
    public void CutCarrot(GameObject carrot)
    {
        Vector3 cutPosition = carrot.transform.position;
        Vector3 cutNormal = Vector3.right; // �c�؂�iX�������j

        // EzySlice�ŃX���C�X
        GameObject[] result = carrot.SliceInstantiate(cutPosition, cutNormal, cutMaterial);

        if (result != null && result.Length == 2)
        {
            // �f�Ђ��Ƃɐi�s������ς���
            Vector3[] directions = {
                (Vector3.right + Vector3.forward - Vector3.up).normalized,   // �E�O��
                (-Vector3.right + Vector3.forward - Vector3.up).normalized   // ���O��
            };

            for (int i = 0; i < 2; i++)
            {
                var go = result[i];

                // ���̂ɂ񂶂�̈ʒu�E��]�E�X�P�[���������p��
                go.transform.position = carrot.transform.position + (i == 0 ? cutNormal : -cutNormal) * separateDistance;
                go.transform.rotation = carrot.transform.rotation;
                go.transform.localScale = carrot.transform.localScale;

                // Rigidbody�ǉ��ŕ�������
                var rb = go.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = go.AddComponent<Rigidbody>();
                }

                // Collider���Ȃ����MeshCollider��ǉ�
                if (go.GetComponent<Collider>() == null)
                {
                    var meshCol = go.AddComponent<MeshCollider>();
                    meshCol.convex = true;
                }

                // �΂ߑO�������ɗ͂�������
                rb.AddForce(directions[i] * forcePower, ForceMode.Impulse);
            }

            // ���̂ɂ񂶂���폜
            Destroy(carrot);
        }
    }
}
