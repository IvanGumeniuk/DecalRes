using System;
using System.Collections;
using UnityEngine;

public class OpenAnimationUIController : MonoBehaviour
{
    public Action<OpenAnimationUIController> OnOpened;
    public Action<OpenAnimationUIController> OnClosed;

    public Action<OpenAnimationUIController> OnStartOpening;
    public Action<OpenAnimationUIController> OnStartClosing;

    [SerializeField] private bool isOpened;

    [SerializeField] private RectTransform target;

    public Vector2 openedPosition;
    public Vector2 closedPosition;

    public float speed;
    public float animationDuration;

    public bool isAnimating;
    private float defaultDistance;

    private Coroutine animationCoroutine;
    
    public bool IsOpened 
    { 
        get { return isOpened; }
		set 
        {
			isOpened = value;

			if (isOpened)
			{
                OnOpened?.Invoke(this);
			}
			else
			{
                OnClosed?.Invoke(this);
            }
        }
    }

    void Start()
    {
        defaultDistance = Vector2.Distance(target.anchoredPosition, IsOpened ? closedPosition : openedPosition);
    }

	public void Open()
	{
        if (target == null || isOpened)
            return;

        float distanceFromTargetPosition = Vector2.Distance(target.anchoredPosition, openedPosition);
        animationDuration = speed * (distanceFromTargetPosition / defaultDistance);
        
        CheckAnimationPlaying();
        animationCoroutine = StartCoroutine(Animate(target.anchoredPosition, openedPosition, animationDuration));
    }

    public void Close()
	{
        if (target == null || !isOpened)
            return;

        float distanceFromTargetPosition = Vector2.Distance(target.anchoredPosition, closedPosition);
        animationDuration = speed * (distanceFromTargetPosition / defaultDistance);

        CheckAnimationPlaying();
        animationCoroutine = StartCoroutine(Animate(target.anchoredPosition, closedPosition, animationDuration));
    }

    private IEnumerator Animate(Vector2 from, Vector2 to, float duration)
	{
		if (IsOpened)
		{
            OnStartClosing?.Invoke(this);
		}
		else
		{
            OnStartOpening?.Invoke(this);
        }

        isAnimating = true;
        bool actionFinished = false;

        float timeActionStarted = Time.time;

        while (!actionFinished)
        {
            float timeSince = Time.time - timeActionStarted;
            float percentage = timeSince / duration;

            var position = Vector2.Lerp(from, to, percentage);
            
            // Check is not NAN
            if(position.x == position.x)
                target.anchoredPosition = Vector2.Lerp(from, to, percentage);

            actionFinished = Time.time - timeActionStarted > duration;
            yield return null;
        }

        IsOpened = !IsOpened;
        animationCoroutine = null;
    }

    private void CheckAnimationPlaying()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
            IsOpened = !IsOpened;
        }
    }
}
