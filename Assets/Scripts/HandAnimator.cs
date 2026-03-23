using System;
using DG.Tweening;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    [Header("Рука")]
    [SerializeField] private RectTransform handRect;

    [Header("Скорости (секунды)")]
    [SerializeField] private float pickUpDuration    = 0.4f;
    [SerializeField] private float returnDuration    = 0.4f;
    [SerializeField] private float dipMoveDuration   = 0.25f;  // движение к палитре
    [SerializeField] private float dipStrokeDuration = 0.12f;  // один мазок на палитре
    [SerializeField] private float applyMoveDuration = 0.25f; // подлёт к лицу
    [SerializeField] private float applyStrokeDuration = 0.1f;// один мазок на лице
    [SerializeField] private float lipstickCircleDuration = 0.2f; // одно круговое движение

    [Header("Смещения мазков на палитре")]
    [SerializeField] private float dipStrokeOffset = 30f; // насколько кисть водит по цвету

    [Header("Смещения мазков на лице")]
    [SerializeField] private float faceStrokeOffset  = 70f;  // влево/вправо
    [SerializeField] private Vector2 eyeshadowOffset = new Vector2(0, 60f);  // выше
    [SerializeField] private Vector2 blushOffset     = new Vector2(0, 0f);   // переносица
    [SerializeField] private float lipstickRadius    = 30f;  // радиус кружка для помады

    [Header("Позиции")]
    [SerializeField] private RectTransform defaultPosition;
    [SerializeField] private RectTransform midPosition;

    public event Action OnAnimationFinished;
    public event Action OnPickupReached;
    public event Action OnDipCompleted;

    public RectTransform HandRect => handRect;

    private Sequence _currentSequence;
    
    public void PlayPickUp(Vector2 itemPosition)
    {
        Stop();
        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(itemPosition, pickUpDuration).SetEase(Ease.InOutSine))
            .AppendCallback(() => OnPickupReached?.Invoke())
            .AppendInterval(0.08f)
            .Append(handRect.DOAnchorPos(midPosition.anchoredPosition, pickUpDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }

    /// <summary>
    /// Тени / румяна:
    /// 1. Рука летит к кнопке цвета
    /// 2. Водит кисть влево-вправо 3 раза по палитре (окунает)
    /// 3. OnDipCompleted — красим кончик
    /// 4. Рука в midPosition
    /// </summary>
    
    public void PlayPickBrushAndDip(Vector2 brushPosition, Vector2 palettePosition)
    {
        Stop();

        Vector2 dipDownPoint = palettePosition + new Vector2(0f, -18f);

        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(brushPosition, pickUpDuration).SetEase(Ease.InOutSine))
            .AppendCallback(() => OnPickupReached?.Invoke())
            .Append(handRect.DOAnchorPos(palettePosition, dipMoveDuration).SetEase(Ease.InOutSine))
            .Append(handRect.DOAnchorPos(dipDownPoint, dipStrokeDuration).SetEase(Ease.InOutSine))
            .Append(handRect.DOAnchorPos(palettePosition, dipStrokeDuration).SetEase(Ease.InOutSine))
            .AppendCallback(() => OnDipCompleted?.Invoke())
            .Append(handRect.DOAnchorPos(midPosition.anchoredPosition, pickUpDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }
    
    public void PlayApplyCream(Vector2 facePosition)
    {
        Stop();
        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(facePosition, applyMoveDuration).SetEase(Ease.InOutSine))
            .AppendInterval(0.2f)
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }
    
    public void PlayApplyEyeshadow(Vector2 facePosition)
    {
        Stop();
        Vector2 center = facePosition + eyeshadowOffset;
        _currentSequence = BuildFaceStrokeSequence(center);
    }
    
    public void PlayApplyBlush(Vector2 facePosition)
    {
        Stop();
        Vector2 center = facePosition + blushOffset;
        _currentSequence = BuildFaceStrokeSequence(center);
    }
    
    public void PlayApplyLipstick(Vector2 facePosition)
    {
        Stop();
        
        Vector2 top    = facePosition + new Vector2(0,               lipstickRadius);
        Vector2 right  = facePosition + new Vector2(lipstickRadius,  0);
        Vector2 bottom = facePosition + new Vector2(0,              -lipstickRadius);
        Vector2 left   = facePosition + new Vector2(-lipstickRadius, 0);

        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(facePosition, applyMoveDuration).SetEase(Ease.InOutSine))
            .AppendInterval(0.05f);
        
        for (int i = 0; i < 3; i++)
        {
            _currentSequence
                .Append(handRect.DOAnchorPos(top,    lipstickCircleDuration).SetEase(Ease.Linear))
                .Append(handRect.DOAnchorPos(right,  lipstickCircleDuration).SetEase(Ease.Linear))
                .Append(handRect.DOAnchorPos(bottom, lipstickCircleDuration).SetEase(Ease.Linear))
                .Append(handRect.DOAnchorPos(left,   lipstickCircleDuration).SetEase(Ease.Linear))
                .Append(handRect.DOAnchorPos(top,    lipstickCircleDuration).SetEase(Ease.Linear));
        }

        _currentSequence
            .Append(handRect.DOAnchorPos(facePosition, applyMoveDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }
    
    public void PlayReturn(Vector2 itemPosition)
    {
        Stop();
        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(itemPosition, returnDuration).SetEase(Ease.InOutSine))
            .AppendInterval(0.08f)
            .Append(handRect.DOAnchorPos(defaultPosition.anchoredPosition, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }
    
    public void PlayReturnToDefault()
    {
        Stop();
        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(defaultPosition.anchoredPosition, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => OnAnimationFinished?.Invoke());
    }
    
    public void PlayCancelToDefault(Vector2 itemPosition, Action onComplete = null)
    {
        Stop();
        _currentSequence = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(itemPosition, returnDuration).SetEase(Ease.InOutSine))
            .AppendInterval(0.05f)
            .Append(handRect.DOAnchorPos(defaultPosition.anchoredPosition, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SetPositionImmediate(Vector2 position)
    {
        handRect.anchoredPosition = position;
    }

    // =====================
    // ПРИВАТНЫЕ МЕТОДЫ
    // =====================

    /// <summary>
    /// Общая цепочка мазков по лицу для теней и румян:
    /// Летим к центру → 3 прохода влево-вправо → центр → OnAnimationFinished.
    /// </summary>
    private Sequence BuildFaceStrokeSequence(Vector2 center)
    {
        Vector2 left  = center + new Vector2(-faceStrokeOffset, 0);
        Vector2 right = center + new Vector2(faceStrokeOffset,  0);

        var seq = DOTween.Sequence()
            .Append(handRect.DOAnchorPos(center, applyMoveDuration).SetEase(Ease.InOutSine))
            .AppendInterval(0.05f);

        for (int i = 0; i < 3; i++)
        {
            seq.Append(handRect.DOAnchorPos(left,  applyStrokeDuration).SetEase(Ease.Linear))
               .Append(handRect.DOAnchorPos(right, applyStrokeDuration).SetEase(Ease.Linear));
        }

        seq.Append(handRect.DOAnchorPos(center, applyStrokeDuration).SetEase(Ease.Linear))
           .OnComplete(() => OnAnimationFinished?.Invoke());

        return seq;
    }

    private void Stop()
    {
        _currentSequence?.Kill();
        _currentSequence = null;
    }

    private void OnDestroy()
    {
        Stop();
    }
}