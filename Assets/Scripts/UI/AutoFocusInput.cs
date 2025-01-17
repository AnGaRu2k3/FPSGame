using TMPro;
using UnityEngine;

public class AutoFocusInput : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        inputField?.Select();
        inputField?.ActivateInputField();
    }
}
