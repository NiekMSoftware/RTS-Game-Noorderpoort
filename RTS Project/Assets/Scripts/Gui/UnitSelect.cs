using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private RawImage icon;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private Unit currentUnit;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void Setup()
    {
        if (currentUnit == null) return;

        icon.texture = currentUnit.GetRenderTexture();
    }

    private void Update()
    {
        if (!currentUnit) return;

        healthSlider.maxValue = (float)currentUnit.UnitMaxHealth;
        healthSlider.value = (float)currentUnit.UnitHealth;
        healthText.SetText((((float)currentUnit.UnitHealth / (float)currentUnit.UnitMaxHealth) * 100) + "%");
    }

    public void Init(Unit unit)
    {
        currentUnit = unit;
        print(currentUnit);
        Setup();
    }
}
