using UnityEngine;

namespace TheHuntIsOn.Modules.PauseTimerModule;

public enum PauseTimerPosition
{
    BottomLeft,
    BottomCenter,
    BottomRight,
    CenterLeft,
    CenterRight,
    TopCenter,
    TopRight,
    BelowUI,
}

public record PauseTimerSpacingParameters(
    TMPro.TMP_Compatibility.AnchorPositions anchor,
    Vector2 anchorPos, Vector2 forwardSpace, Vector2 reverseSpace)
{
    public Vector2 GetPosition(int i, int count, float spacing) => anchorPos + spacing * (forwardSpace * i + reverseSpace * (count - 1 - i));
}

public static class PauseTimerPositionExtensions
{
    private static bool IsLeft(this PauseTimerPosition self) => self == PauseTimerPosition.BelowUI || self == PauseTimerPosition.CenterLeft || self == PauseTimerPosition.BottomLeft;

    private static bool IsRight(this PauseTimerPosition self) => self == PauseTimerPosition.BottomRight || self == PauseTimerPosition.CenterRight || self == PauseTimerPosition.TopRight;

    private static bool IsTop(this PauseTimerPosition self) => self == PauseTimerPosition.BelowUI || self == PauseTimerPosition.TopCenter || self == PauseTimerPosition.TopRight;

    private static bool IsBottom(this PauseTimerPosition self) => self == PauseTimerPosition.BottomLeft || self == PauseTimerPosition.BottomCenter || self == PauseTimerPosition.BottomRight;

    private static bool IsVCenter(this PauseTimerPosition self) => self == PauseTimerPosition.CenterLeft || self == PauseTimerPosition.CenterRight;

    private static TMPro.TMP_Compatibility.AnchorPositions AnchorPositions(this PauseTimerPosition self) => self switch
    {
        PauseTimerPosition.BottomLeft => TMPro.TMP_Compatibility.AnchorPositions.BottomLeft,
        PauseTimerPosition.BottomCenter => TMPro.TMP_Compatibility.AnchorPositions.Bottom,
        PauseTimerPosition.BottomRight => TMPro.TMP_Compatibility.AnchorPositions.BottomRight,
        PauseTimerPosition.CenterLeft => TMPro.TMP_Compatibility.AnchorPositions.Left,
        PauseTimerPosition.CenterRight => TMPro.TMP_Compatibility.AnchorPositions.Right,
        PauseTimerPosition.TopCenter => TMPro.TMP_Compatibility.AnchorPositions.Top,
        PauseTimerPosition.TopRight => TMPro.TMP_Compatibility.AnchorPositions.TopRight,
        PauseTimerPosition.BelowUI => TMPro.TMP_Compatibility.AnchorPositions.TopLeft
    };

    public static PauseTimerSpacingParameters SpacingParameters(this PauseTimerPosition self)
    {
        Vector2 anchorPos;
        anchorPos.x = self.IsLeft() ? -7 : (self.IsRight() ? 7 : 0);
        anchorPos.y = self.IsTop() ? -6 : (self.IsBottom() ? 6 : 0);
        if (self == PauseTimerPosition.BelowUI) anchorPos.y = -5;
        if (self == PauseTimerPosition.BottomLeft) anchorPos.y = 5;

        Vector2 forwardSpace = new(0, self.IsTop() ? -1 : (self.IsVCenter() ? -0.5f : 0));
        Vector2 reverseSpace = new(0, self.IsBottom() ? 1 : (self.IsVCenter() ? 0.5f : 0));
        return new(self.AnchorPositions(), anchorPos, forwardSpace, reverseSpace);
    }
}
