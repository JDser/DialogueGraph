using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Tooltip")]
    [SerializeField] private Text m_TooltipText;

    [Header("Subtitle")]
    [SerializeField] private Text m_SubtitleText;

    [Header("Choices")]
    [SerializeField] private RectTransform m_ChoiceContainer;
    [SerializeField] private RectTransform m_ChoiceContent;
    [SerializeField] private ChoiceButton m_ButtonPrefab;

    private void Start()
    {
        m_TooltipText.text = null;
        m_SubtitleText.text = null;
        m_ChoiceContainer.gameObject.SetActive(false);
    }

    public void SetTooltip(/*InteractionTrigger trigger*/)
    {
       // string name;
       // string message;
       //
       // if (!string.IsNullOrEmpty(trigger.OverrideName))
       // {
       //     name = trigger.OverrideName;
       // }
       // else
       // {
       //     name = trigger.name;
       // }
       //
       // message = trigger.OverrideMessage;
       //
       // m_TooltipText.text = $"{message} \n{name}";
    }

    public void ClearTooltip()
    {
        m_TooltipText.text = null;
    }

    public void SetSubtitle(string text)
    {
        m_SubtitleText.text = text;
    }

    public void ClearSubtitle()
    {
        m_SubtitleText.text = null;
    }

    public void SetChoice(string text, System.Action onClick)
    {
        m_ChoiceContainer.gameObject.SetActive(true);

        ChoiceButton newChoice = Instantiate(m_ButtonPrefab);
        newChoice.Setup(text, onClick);

        newChoice.transform.SetParent(m_ChoiceContent);
    }

    public void ClearChoices()
    {
        m_ChoiceContainer.gameObject.SetActive(false);

        for (int i = 0; i < m_ChoiceContent.childCount; i++)
        {
            Destroy(m_ChoiceContent.GetChild(i).gameObject);
        }
    }
}