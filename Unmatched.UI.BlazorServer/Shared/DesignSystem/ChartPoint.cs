namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>One sample on a line chart or sparkline: the value plus the label shown on the x axis.
/// Reason/IsAward are optional context the rating chart uses for its hover tooltip and permanently
/// highlighted award markers (see LineChartSvg) - every other caller leaves them unset.</summary>
public record ChartPoint(string Label, double Value, string? Reason = null, bool IsAward = false);
