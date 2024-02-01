using StyledRazor.Core.Component;
using StyledRazor.Core.Style.ComponentStyle;
using StyledRazor.Core.Style.DesignTokens;

namespace StyledRazor.Core.UnitTests.Style.ComponentStyle;

public class ComponentStyleProviderShould
{
  private class TestComponent : Styled
  {
    public TestComponent(ITokens tokens) : base(tokens) {}

    protected override Styled Component => CreateStyled.Div(@"{
      Property: Value;
    }");
  }

  [Test]
  public void GetComponentStyle_FromAStyledComponent()
  {
    const string expectedCssString = "div[TestComponent]{Property:Value;}";
    
    var componentStyle = new ComponentStyleProvider(new Tokens()).Get(typeof(TestComponent));

    Assert.Multiple(() =>
    {
      Assert.That(componentStyle.Type, Is.EqualTo(typeof(TestComponent)));
      Assert.That(componentStyle.CssString, Is.EqualTo(ExpectedCssStringWithScopeFrom(componentStyle, expectedCssString)));
    });
  }
  
  private static string ExpectedCssStringWithScopeFrom(Core.Style.ComponentStyle.ComponentStyle componentStyle, string expected) => 
    expected.Replace("TestComponent", componentStyle.Id);
}