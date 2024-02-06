using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();    
    }

    private void OnEnable()
    {
        if (button != null)
        {

            button.onClick.AddListener(PlayButtonClickSound);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayButtonClickSound);
        }
    }

    private void PlayButtonClickSound()
    {
        SoundManager.Instance.PlayClickSound();    
    }
}