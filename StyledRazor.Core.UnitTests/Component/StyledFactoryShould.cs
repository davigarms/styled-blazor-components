using StyledRazor.Core.Component;
using StyledRazor.Core.Model;
using System.Text.Json;

namespace StyledRazor.Core.UnitTests.Component;

[TestFixture]
public class StyledFactoryShould
{
  private const string UnexpectedSpace = " ";
  private const string MalformedSelectorWithColon = $":{UnexpectedSpace}Name";
  
  private StyledFactory _createStyled = new(new TestComponent());

  private class TestComponent : Styled {}

  private static IEnumerable<(string, string)> CssCases()
  {
    yield return (@"{
                    Property1: Value;
                  }", "Element[ScopeId]{Property1:Value;}");

    yield return (@"{
                    Property1 : Value;
                  }", "Element[ScopeId]{Property1:Value;}");

    yield return (@"{
                    Property1: Value ;
                  }", "Element[ScopeId]{Property1:Value;}");

    yield return (@"{
                    Property1: Value;
                    Property2: Value;
                  }", "Element[ScopeId]{Property1:Value;Property2:Value;}");

    yield return (@"{
                    Property1: Value;     Property2: Value;
                  }", "Element[ScopeId]{Property1:Value;Property2:Value;}");

    yield return (@"{Property1: Value; Property2: Value;}", "Element[ScopeId]{Property1:Value;Property2:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }

                    Selector {
                      Property1: Value;
                    }", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }


                    Selector {
                      Property1: Value;
                    }

                    ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    :Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]:Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    ::Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]::Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    > :Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]> :Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    >:Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]>:Selector{Property1:Value;}");

    yield return (@"{
                      Property1: Value;
                      Property2: Value;
                    }
                    >  :Selector {
                      Property1: Value;
                    }
                  ", "Element[ScopeId]{Property1:Value;Property2:Value;}Element[ScopeId]>:Selector{Property1:Value;}");
  }

  [SetUp]
  public void SetUp() => _createStyled = new StyledFactory(new TestComponent());

  [TestCaseSource(nameof(CssCases))]
  public void CreateAStyledDiv_WithMinifiedAndScopedCss((string original, string minified) css)
  {
    var styled = _createStyled.Div(css.original).ComponentStyle;

    Assert.That(styled.CssString, Is.EqualTo(MinifiedCssWithScopeFor("div", styled.Id, css.minified)));
  }

  [TestCaseSource(nameof(CssCases))]
  public void CreateAStyledHyperLink_WithMinifiedAndScopedCss((string original, string minified) css)
  {
    var styled = _createStyled.A(css.original).ComponentStyle;

    Assert.That(styled.CssString, Is.EqualTo(MinifiedCssWithScopeFor("a", styled.Id, css.minified)));
  }

  [TestCaseSource(nameof(CssCases))]
  public void CreateAStyledH1_WithMinifiedAndScopedCss((string original, string minified) css)
  {
    var styled = _createStyled.H1(css.original).ComponentStyle;

    Assert.That(styled.CssString, Is.EqualTo(MinifiedCssWithScopeFor("h1", styled.Id, css.minified)));
  }

  [TestCaseSource(nameof(CssCases))]
  public void CreateAStyledUl_WithMinifiedAndScopedCss((string original, string minified) css)
  {
    var styled = _createStyled.Ul(css.original).ComponentStyle;

    Assert.That(styled.CssString, Is.EqualTo(MinifiedCssWithScopeFor("ul", styled.Id, css.minified)));
  }

  [TestCaseSource(nameof(CssCases))]
  public void CreateAStyledLi_WithMinifiedAndScopedCss((string original, string minified) css)
  {
    var styled = _createStyled.Li(css.original).ComponentStyle;

    Assert.That(styled.CssString, Is.EqualTo(MinifiedCssWithScopeFor("li", styled.Id, css.minified)));
  }

  [TestCaseSource(nameof(CssCases))]
  public void ReturnExistingStyled_WhenStyledAlreadyExists((string original, string minified) css)
  {
    var styled = _createStyled.Div(css.original).ComponentStyle;
    var styledId = styled.Id;
    styled = _createStyled.Div(css.original).ComponentStyle;

    Assert.That(styled.Id, Is.EqualTo(styledId));
  }

  [Test]
  public void ThrowJsonException_WhenCssIsMalformed()
  {
    const string css = @$"
      {MalformedSelectorWithColon} {{
        Property: Value
      }}";

    Assert.Throws<JsonException>(() => _createStyled.Div(css));
  }

  private static string MinifiedCssWithScopeFor(string element, string scopeId, string cssMinified) =>
    cssMinified.Replace("ScopeId", scopeId).Replace("Element", element);
}