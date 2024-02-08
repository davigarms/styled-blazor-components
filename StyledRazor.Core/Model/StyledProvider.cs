using StyledRazor.Core.Component;
using StyledRazor.Core.Style.DesignTokens;
using System;

namespace StyledRazor.Core.Model;

public class StyledProvider
{
  private readonly ITokens _tokens;
  
  public StyledProvider(ITokens tokens)
  {
    _tokens = tokens;
  }

  public Styled Get(Type styleType) => 
    (Activator.CreateInstance(styleType, _tokens) as StyledBase)?.Base;
}