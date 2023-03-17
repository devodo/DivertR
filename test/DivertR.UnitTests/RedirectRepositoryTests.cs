using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class RedirectRepositoryTests
{
    private readonly IRedirect<IFoo> _fooRedirect = new Redirect<IFoo>();
    private readonly IFoo _fooProxy;

    public RedirectRepositoryTests()
    {
        _fooProxy = _fooRedirect.Proxy();
    }
    
    [Fact]
    public void GivenExistingVia_WhenResetAndInsertVia_ThenResetsAndInsertsVia()
    {
        // ARRANGE
        _fooRedirect.To(x => x.Name).Via("Redirect");
        var beforeName = _fooProxy.Name;

        // ACT
        var replaceVia = ViaBuilder<IFoo>.To(x => x.Name).Build("Replaced");
        _fooRedirect.RedirectRepository.ResetAndInsert(replaceVia);
        
        // ASSERT
        beforeName.ShouldBe("Redirect");
        _fooProxy.Name.ShouldBe("Replaced");
    }

    [Fact]
    public void GivenExistingPersistentVia_WhenResetAndInsertVia_ThenAddsVia()
    {
        // ARRANGE
        _fooRedirect.To(x => x.Name).Via("Redirect", opt => opt.Persist());

        // ACT
        var replaceVia = ViaBuilder<IFoo>.To(x => x.Name).Build(call => call.CallNext() + " Replaced");
        _fooRedirect.RedirectRepository.ResetAndInsert(replaceVia);
        
        // ASSERT
        _fooProxy.Name.ShouldBe("Redirect Replaced");
    }
    
    [Fact]
    public void GivenExistingVia_WhenResetAndInsertWithPersistentVia_ThenResetsAndInsertsVia()
    {
        // ARRANGE
        _fooRedirect.To(x => x.Name).Via("Redirect");
        var beforeName = _fooProxy.Name;

        // ACT
        var replaceVia = ViaBuilder<IFoo>.To(x => x.Name).Build("Replaced");
        _fooRedirect.RedirectRepository.ResetAndInsert(replaceVia, new ViaOptions(isPersistent: true));
        
        // ASSERT
        beforeName.ShouldBe("Redirect");
        _fooProxy.Name.ShouldBe("Replaced");
    }
}