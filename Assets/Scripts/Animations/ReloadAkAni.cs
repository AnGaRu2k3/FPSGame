using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ReLoadAk : MonoBehaviour
{
    public TwoBoneIKConstraint leftHandIKMag;
    public float weight = 0f;
    // Hàm để chuyển đổi giữa leftHandMag và leftHandBase
    public void SwitchTarget()
    {
        
        weight = 1 - weight;
        leftHandIKMag.weight = weight;
        Debug.Log($"changeWeight to {leftHandIKMag.weight}");
    }

    
}
