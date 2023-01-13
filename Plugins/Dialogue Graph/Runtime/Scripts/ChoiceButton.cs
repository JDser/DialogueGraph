using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Text m_ChoiceText;

    private Button m_Button;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    public void Setup(string line, System.Action onClick)
    {
        m_Button.onClick.RemoveAllListeners();

        m_ChoiceText.text = line;
        m_Button.onClick.AddListener(() => ActionWrapper(onClick));
    }

    private void ActionWrapper(System.Action action)
    {
        action?.Invoke();
    }
}