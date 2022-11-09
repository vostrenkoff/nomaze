using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventSystem : MonoBehaviour
{
	public static CustomEventSystem current;
	public void Awake()
	{
		current = this;
	}
	public event Action onUpdateTimeTBS;
	public event Action<Vector2> onUpdateDebug;
	public void UpdateTimeTBS()
	{
		onUpdateTimeTBS?.Invoke();
	}
	public void UpdateDebug(Vector2 pos)
	{
		onUpdateDebug?.Invoke(pos);
	}
}
