using Sigaba.App.Services.SigabaFiles;
using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives.Crypto;

namespace Sigaba.App.TestHelpers;

public class Fixture
{
    private static readonly PublicKey dummyPublicKey = PublicKey.Any();
    public IEnumerable<ISigabaFile> AllImplementationsInstancesOfSigabaFile { get; } = GetAllImplementationsInstancesOfSigabaFile();

    private static IEnumerable<ISigabaFile> GetAllImplementationsInstancesOfSigabaFile()
    {
        ISigabaFile[] result =
        [
            SigabaFileV1.CreateDefault(dummyPublicKey),
        ];

        var allVersionTypes = InterfacesInspector.GetAllImplementationsOf<ISigabaFile>();
        var resultTypes = result.Select(i => i.GetType());
        allVersionTypes.Should().NotBeEmpty("it should have at least one implementation of ISigabaFile");
        resultTypes.Should().BeEquivalentTo(allVersionTypes, "it should yield all implementations of ISigabaFile");

        return result;
    }
}

[CollectionDefinition(nameof(Fixture))]
public class SigabaFileManagerTestCollection : ICollectionFixture<Fixture>
{
}
