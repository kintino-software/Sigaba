using Kintino.CipherConf.App.Services.PrivateKeys;

namespace Kintino.CipherConf.App.Services.PrivateKeys;

public class PrivateKeyFileRepositoryTest : BaseTest
{
    private IPrivateKeyRepository CreateService()
    {
        return new PrivateKeyFileRepository(Fs);
    }

    [Fact]
    public async Task LoadAsync_StateUnderTest_ExpectedBehavior()
    {
        Assert.True(false);
    }

    [Fact]
    public async Task SaveAsync_StateUnderTest_ExpectedBehavior()
    {
        Assert.True(false);
    }
}

