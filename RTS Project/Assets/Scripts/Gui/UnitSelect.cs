using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private int maxUnitHealth = 50;

    private Unit currentUnit;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void Update()
    {
        if (!currentUnit) return;

        print("hi");
        print(currentUnit.UnitHealth);
        healthSlider.value = currentUnit.UnitHealth;
        healthText.SetText(((currentUnit.UnitHealth / maxUnitHealth) * 100) + "%");
    }

    public void Init(Unit unit)
    {
        currentUnit = unit;
    }
}
