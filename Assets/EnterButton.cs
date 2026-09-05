using UnityEngine;

public class EnterButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.IsInputComplete = true;
    }
}
