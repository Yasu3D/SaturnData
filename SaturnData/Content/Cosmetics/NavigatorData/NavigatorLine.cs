namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// Describes a single dialogue line to display in a textbox.
/// </summary>
public class NavigatorLine
{
    /// <summary>
    /// The dialogue message to display in a textbox.
    /// </summary>
    public string Message { get; set; } = "";
    
    /// <summary>
    /// The local filepath to a spoken version of the message.
    /// </summary>
    public string AudioPath { get; set; } = "";
    
    /// <summary>
    /// The duration of how long to show the textbox.
    /// </summary>
    /// <remarks>
    /// This value should be greater than or equal to the duration of the audio file defined by <see cref="AudioPath"/>.
    /// </remarks>
    public float Duration { get; set; } = 5000;
    
    /// <summary>
    /// Determines if a button to skip a textbox should pop up.
    /// </summary>
    public bool ShowSkipButton { get; set; } = false;
    
    /// <summary>
    /// Determines the expression the chosen navigator will show.
    /// </summary>
    public NavigatorExpression Expression { get; set; } = NavigatorExpression.Neutral;
    
    /// <summary>
    /// A fallback message to display if no valid <see cref="NavigatorLine"/> with the given language and key was found.
    /// </summary>
    /// <param name="key">The key of the message.</param>
    /// <returns></returns>
    public static NavigatorLine NoMessageFound(string key = "-")
    {
        return new()
        {
            Message = $"No message found. Key: {key}",
            AudioPath = "",
            Duration = 10000,
            Expression = NavigatorExpression.Neutral,
            ShowSkipButton = false,
        };
    }
}