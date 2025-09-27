// Assets/_Core/Runtime/Debug/ATPDebugHUD.cs
using UnityEngine;
using Core.TimeSystem;
using Core.Economy;

public class ATPDebugHUD : MonoBehaviour
{
    public ATPWallet wallet;
    public BodyClockDirector clock;
    GUIStyle _s;

    void OnGUI()
    {
        if (_s == null) _s = new GUIStyle(GUI.skin.label){ fontSize = 14 };
        GUILayout.BeginArea(new Rect(12,12,280,90), GUI.skin.box);
        GUILayout.Label($"ATP: {(wallet ? wallet.Current : 0f):N1}", _s);
        if (clock != null)
        {
            GUILayout.Label($"Phase: {clock.CurrentPhase}  xATP: {clock.Multipliers.atpIncome:F2}", _s);
            GUILayout.Label($"timeScale: {Time.timeScale:F2}  planning: {clock.IsPlanningActive}", _s);
        }
        GUILayout.EndArea();
    }
}