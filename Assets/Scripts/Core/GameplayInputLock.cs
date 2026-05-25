using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class GameplayInputLock
{
    private static readonly HashSet<object> lockSources = new();

    public static bool IsLocked => lockSources.Count > 0;

    public static void SetLocked(object source, bool locked)
    {
        if (source == null)
        {
            return;
        }

        if (locked)
        {
            lockSources.Add(source);
        }
        else
        {
            lockSources.Remove(source);
        }
    }

    public static void ClearLock(object source)
    {
        if (source == null)
        {
            return;
        }

        lockSources.Remove(source);
    }

    public static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public static bool ShouldBlockWorldClick()
    {
        return IsLocked || IsPointerOverUI();
    }
}