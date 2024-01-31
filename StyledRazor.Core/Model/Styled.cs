using Microsoft.AspNetCore.Components;
using StyledRazor.Core.Component;
using StyledRazor.Core.Style.Css;
using System;
using static StyledRazor.Core.Style.Css.CssHelper;

namespace StyledRazor.Core.Model;

public class Styled
{
  private CssRuleset Css { get; set; }

  public string Id { get; private set; }

  public string Element { get; private set; }

  public string CssString { get; private set; }
  
  public Type Type { get; private set; }

  public StyledBase Component { get; }
  
  internal Styled(IComponent component, string element, string baseCss)
  {
    Type = component.GetType();
    Id = IdFrom(Type.Name);
    Element = element;
    Component = new StyledBase(this);
    Css = CssFactory.Create(baseCss, ScopeFrom(Id, Element));
    UpdateCss();
  }

  public void Update(Styled styled)
  {
    Type = styled.Type;
    Id = styled.Id;
    Element = styled.Element;
    Css = styled.Css;
    UpdateCss();
  }

  public CssStyleDeclaration Get(string selector)
  {
    var scopedSelector = $"{ScopeFrom(Id, Element)}{selector}";
    var declaration = Css.Get(scopedSelector);
    declaration.OnChange += UpdateCss;
    return declaration;
  }

  private void UpdateCss() => CssString = Css?.ToString();
}