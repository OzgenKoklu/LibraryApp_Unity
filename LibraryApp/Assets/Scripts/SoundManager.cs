using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private const string PlayerPrefsSound = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Button _soundOnOffToggleButton;
    [SerializeField] private Sprite _soundOnIcon;
    [SerializeField] private Sprite _soundOffIcon;

    private float volume = 1f;

    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PlayerPrefsSound, 1f);

        SetToggleButtonImage();

        _soundOnOffToggleButton.onClick.AddListener(ToggleApplicationSoundOnAndOff);
    }

    private void ToggleApplicationSoundOnAndOff()
    {
        if(volume == 1)
        {
            volume = 0;  
        }
        else
        {
            volume = 1;           
        }
        SetToggleButtonImage();

        //saving the option to the player prefs
        PlayerPrefs.SetFloat(PlayerPrefsSound, volume);
        PlayerPrefs.Save();
    }

    public void SetToggleButtonImage()
    {
        if (volume == 1)
        {
            _soundOnOffToggleButton.GetComponent<Image>().sprite = _soundOnIcon;
        }
        else
        {
            _soundOnOffToggleButton.GetComponent<Image>().sprite = _soundOffIcon;
        }
    }

    public void PlaySuccessSound()
    {
        PlaySound(_audioClipRefsSO.Success, _cameraTransform.transform.position);
    }
    public void PlayErrorSound()
    {
        PlaySound(_audioClipRefsSO.Error, _cameraTransform.transform.position);
    }
    public void PlayWarningSound()
    {
        PlaySound(_audioClipRefsSO.Warning, _cameraTransform.transform.position);
    }
    public void PlayMouseClick()
    {
        PlaySound(_audioClipRefsSO.MouseClick, _cameraTransform.transform.position);
    }

    public void PlayClickSound()
    {
        PlaySound(_audioClipRefsSO.MouseClick, _cameraTransform.transform.position);
    }

    private void PlaySound(AudioClip audioclip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(audioclip, position, volume);
    }
}
