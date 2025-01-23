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
        // 入力を取得
        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");

        // カメラの向きを基準に移動方向を計算
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // 水平方向のみ考慮
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0; // 水平方向のみ考慮
        cameraRight.Normalize();

        // 入力に応じた移動ベクトルを計算
        var movement = (cameraRight * dx + cameraForward * dz);

        // 移動ベクトルの大きさを1に正規化
        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        // 後ろまたは後ろ斜めの時の速度調整
        if (Vector3.Dot(cameraForward, movement) < 0) // カメラ前方と移動ベクトルの角度で後方を判定
        {
            movement *= 0.8f; // 速度を80%に調整
        }

        // Rigidbodyに力を加える
        rb.AddForce(movement * 2.0f);
    }
}
