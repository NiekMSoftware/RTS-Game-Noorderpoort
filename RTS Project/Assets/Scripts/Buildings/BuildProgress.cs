using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildProgress : MonoBehaviour
{
    private Image progressImage;
    private float buildTime;

    private void Awake()
    {
        progressImage = GetComponent<Image>();
        progressImage.fillAmount = 0;
    }

    public void Init(float _buildTime)
    {
        buildTime = _buildTime;
    }

    private void Start()
    {
        Sequence tween = DOTween.Sequence();
        tween.Append(progressImage.DOFillAmount(1f, buildTime));
        tween.AppendInterval(2);
        tween.Append(progressImage.DOFade(0f, 1f));
        tween.OnComplete(() => Destroy(gameObject));
        tween.Play();
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}