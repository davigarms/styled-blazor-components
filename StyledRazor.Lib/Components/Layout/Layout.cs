using StyledRazor.Core.Component;
using StyledRazor.Core.Model;

namespace StyledRazor.Lib.Components.Layout;

public class Layout : StyledBase
{
  protected override StyledBase Component => Create.Div();
}