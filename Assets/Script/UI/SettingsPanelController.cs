using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI")]
    public Slider sliderMaster;
    public Slider sliderBGM;
    public Slider sliderSFX;
    public Toggle toggleMute;
    public TextMeshProUGUI txtMaster; // optional
    public TextMeshProUGUI txtBGM;    // optional
    public TextMeshProUGUI txtSFX;    // optional

    const string K_MASTER = "vol_master";
    const string K_BGM    = "vol_bgm";
    const string K_SFX    = "vol_sfx";
    const string K_MUTE   = "vol_mute";

    float master = 1f, bgm = 1f, sfx = 1f;
    bool muted = false;

    void OnEnable()
    {
        // load saved values
        master = PlayerPrefs.GetFloat(K_MASTER, 1f);
        bgm    = PlayerPrefs.GetFloat(K_BGM, 1f);
        sfx    = PlayerPrefs.GetFloat(K_SFX, 1f);
        muted  = PlayerPrefs.GetInt(K_MUTE, 0) == 1;

        // push to UI
        sliderMaster.SetValueWithoutNotify(master);
        sliderBGM.SetValueWithoutNotify(bgm);
        sliderSFX.SetValueWithoutNotify(sfx);
        toggleMute.SetIsOnWithoutNotify(muted);

        ApplyAll(); // reflect to audio now
        RefreshTexts();
        HookEvents(true);
    }

    void OnDisable() => HookEvents(false);

    void HookEvents(bool on)
    {
        if (on)
        {
            sliderMaster.onValueChanged.AddListener(OnMaster);
            sliderBGM.onValueChanged.AddListener(OnBGM);
            sliderSFX.onValueChanged.AddListener(OnSFX);
            toggleMute.onValueChanged.AddListener(OnMute);
        }
        else
        {
            sliderMaster.onValueChanged.RemoveListener(OnMaster);
            sliderBGM.onValueChanged.RemoveListener(OnBGM);
            sliderSFX.onValueChanged.RemoveListener(OnSFX);
            toggleMute.onValueChanged.RemoveListener(OnMute);
        }
    }

    void OnMaster(float v) { master = v; Save(); ApplyAll(); RefreshTexts(); }
    void OnBGM(float v)    { bgm = v;    Save(); ApplyAll(); RefreshTexts(); }
    void OnSFX(float v)    { sfx = v;    Save(); ApplyAll(); RefreshTexts(); }
    void OnMute(bool v)    { muted = v;  Save(); ApplyAll(); }

    void ApplyAll()
    {
        float m = muted ? 0f : master;
        // music
        if (MusicManager.Instance) MusicManager.Instance.SetVolume(m * bgm);
        // sfx
        if (SFXManager.Instance)   SFXManager.Instance.SetVolume(m * sfx);
        // (필요하면 GameManager 등 다른 소스에도 확장 가능)
    }

    void Save()
    {
        PlayerPrefs.SetFloat(K_MASTER, master);
        PlayerPrefs.SetFloat(K_BGM, bgm);
        PlayerPrefs.SetFloat(K_SFX, sfx);
        PlayerPrefs.SetInt(K_MUTE, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    void RefreshTexts()
    {
        if (txtMaster) txtMaster.text = Mathf.RoundToInt(master * 100) + "%";
        if (txtBGM)    txtBGM.text    = Mathf.RoundToInt(bgm * 100) + "%";
        if (txtSFX)    txtSFX.text    = Mathf.RoundToInt(sfx * 100) + "%";
    }

    // called by Back button
    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // just in case
    }
}