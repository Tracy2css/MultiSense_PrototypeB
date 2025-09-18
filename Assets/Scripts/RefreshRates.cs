using UnityEngine;

public class SetFrameRate : MonoBehaviour
{
    void Start()
    {
        // 设置目标帧率
        Application.targetFrameRate = 90; 
        // 确保垂直同步关闭（仅用于性能测试）
        QualitySettings.vSyncCount = 0;
    }
}
