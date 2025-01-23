using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMove : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ���͂��擾
        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");

        // �J�����̌�������Ɉړ��������v�Z
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // ���������̂ݍl��
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0; // ���������̂ݍl��
        cameraRight.Normalize();

        // ���͂ɉ������ړ��x�N�g�����v�Z
        var movement = (cameraRight * dx + cameraForward * dz);

        // �ړ��x�N�g���̑傫����1�ɐ��K��
        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        // ���܂��͌��΂߂̎��̑��x����
        if (Vector3.Dot(cameraForward, movement) < 0) // �J�����O���ƈړ��x�N�g���̊p�x�Ō���𔻒�
        {
            movement *= 0.8f; // ���x��80%�ɒ���
        }

        // Rigidbody�ɗ͂�������
        rb.AddForce(movement * 2.0f);
    }
}
