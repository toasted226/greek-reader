using I = GreekReader.Interop;

namespace GreekReader.Tests;

public class FacadeTests
{
    [Fact]
    public void GreekAnalyzer_is_usable_from_csharp_through_the_interface()
    {
        I.IGreekAnalyzer analyzer = new I.GreekAnalyzer();

        Assert.NotNull(analyzer);
    }
}
