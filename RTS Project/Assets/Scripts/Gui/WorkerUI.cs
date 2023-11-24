using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public TMP_Text title;
    public RawImage icon;
    public Button fireButton;

    private Worker worker;
    private BuildingSelect buildingSelect;

    private void Awake()
    {
        fireButton.onClick.RemoveAllListeners();
        fireButton.onClick.AddListener(FireWorker);
    }

    public void Setup(string title, Worker worker, BuildingSelect buildingSelect)
    {
        this.title.SetText(title);
        this.worker = worker;
        this.buildingSelect = buildingSelect;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (worker == null) return;

        icon.texture = worker.GetRenderTexture();
    }

    private void FireWorker()
    {
        worker.UnAssignWorker();
        buildingSelect.Setup();
    }
}
