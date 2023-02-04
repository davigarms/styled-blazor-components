using Microsoft.AspNetCore.Components;
using StyledRazor.Core.Collections;
using StyledRazor.Core.Components;
using StyledRazor.Core.Model;
using StyledRazor.Core.Services;
using StyledRazor.Core.Utils;
using StyledRazor.Lib.Styles;
using System.Threading.Tasks;
using System;

namespace StyledRazor.Lib.Components.Layout;

public class Grid : StyledBase, IDisposable
{
  [Parameter]
  public string Gutter { get; set; } = Tokens.SpacingS;

  [Parameter]
  public bool Grow { get; set; } = true;

  [Parameter]
  public string Height { get; set; }

  [Parameter]
  public string BaseWidth { get; set; }

  [Parameter]
  public double Ratio { get; set; }

  [Parameter]
  public int ColsXs { get; set; }

  [Parameter]
  public int ColsSm { get; set; }

  [Parameter]
  public int ColsMd { get; set; }

  [Parameter]
  public int ColsLg { get; set; }

  [Parameter]
  public int ColsXl { get; set; }

  [Parameter]
  public int ColsXxl { get; set; }

  [Parameter]
  public int Cols { get; set; } = 1;

  [Parameter]
  public ResponsiveCols ResponsiveCols { get; set; }

  [Inject]
  private BrowserService Browser { get; set; }

  protected override string Style => $@"
    --height: {CalculatedHeight};
    --width: {CalculatedWidth};
    --flex-grow: {FlexGrow};
    --gutter: {Gutter};
  ";

  protected override Styled Base => Div(@"
    {
      list-style: none;
	    display: flex;
      flex-wrap: wrap;
      padding: 0;
      margin: calc(-1 * var(--gutter)) 0 0 calc(-1 * var(--gutter));
      width: calc(100% + var(--gutter));
      position: relative;
    }

    > * {
      width: calc(var(--width) - 1 * var(--gutter)) !important;
      margin: var(--gutter) 0 0 var(--gutter) !important;
      height: var(--height) !important;
      max-height: var(--height);
      overflow: hidden;
      flex-grow: var(--flex-grow);
    }
      
    > [double] {
      flex-basis: calc(2 * (var(--width)) - 1 * var(--gutter)) !important
    }

    > [triple] {
      flex-basis: calc(3 * (var(--width)) - 1 * var(--gutter)) !important
    }

    > [half] {
      flex-basis: calc(50% - 1 * var(--gutter)) !important
    }

    > [third] {
      flex-basis: calc(33% - 1 * var(--gutter)) !important
    }

    > [full] {
      flex-basis: 100% !important
    }
  ");

  private MediaQueryService MediaQuery { get; set; }

  private string CalculatedHeight { get; set; }

  private string CalculatedWidth => string.IsNullOrEmpty(BaseWidth) ? $"{100 / Cols}%" : $"{BaseWidth}";

  private string FlexGrow => HasBaseWidth ?
                              Grow ? "1" : "0"
                              : "0";

  private bool HasBaseWidth => !string.IsNullOrEmpty(BaseWidth);

  private bool HasHeight => !string.IsNullOrEmpty(Height);

  private bool HasRatio => Ratio != 0;

  protected override Task OnInitializedAsync() => InitComponent();

  public void Dispose()
  {
    BrowserService.OnResize -= WindowSizeHasChanged;
    GC.SuppressFinalize(this);
  }

  private async Task InitComponent()
  {
    MediaQuery = new MediaQueryService(GetResponsiveColumns());
    BrowserService.OnResize += WindowSizeHasChanged;
    await Task.Delay(1);
    await WindowSizeHasChanged();
  }

  private async Task WindowSizeHasChanged()
  {
    await SetNumberOfColumns();
    await SetCalculatedHeight();
    StateHasChanged();
  }

  private async Task SetNumberOfColumns()
  {
    var windowDimension = await Browser.WindowDimension();
    Cols = MediaQuery.NumberOfColumnsFor(windowDimension.Width) ?? Cols;
  }

  private async Task SetCalculatedHeight()
  {
    var elementDimension = await Browser.DimensionFrom(ElementRef);

    CalculatedHeight = HasHeight ? Height :
      HasRatio ? HeightFrom(elementDimension.Width) :
        "initial";
  }

  private string HeightFrom(int elementWidth) => $"{((double)elementWidth / Cols - Utils.RemToInt(Gutter)) / Ratio}px";

  private ResponsiveCols GetResponsiveColumns()
  {
    if (ResponsiveCols != null) return ResponsiveCols;

    var columns = new ResponsiveCols();

    if (ColsXs > 0) columns.Add(BreakPoint.Xs, ColsXs);
    if (ColsSm > 0) columns.Add(BreakPoint.Sm, ColsSm);
    if (ColsMd > 0) columns.Add(BreakPoint.Md, ColsMd);
    if (ColsLg > 0) columns.Add(BreakPoint.Lg, ColsLg);
    if (ColsXl > 0) columns.Add(BreakPoint.Xl, ColsXl);
    if (ColsXxl > 0) columns.Add(BreakPoint.Xxl, ColsXxl);

    return columns.Count > 0 ? columns : null;
  }
}