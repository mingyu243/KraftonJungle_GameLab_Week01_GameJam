using UnityEngine;
using UnityEngine.EventSystems;

public class HelpButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject helpPanel;

    void Start()
    {
        helpPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        helpPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        helpPanel.SetActive(false);
    }
}
