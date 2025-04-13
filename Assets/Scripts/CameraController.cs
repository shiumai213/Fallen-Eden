using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // プレイヤーなどのターゲット

    void LateUpdate()
    {
        if (target != null)
        {
            // カメラがターゲットを追従するコード
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
