// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class FilePersistenceUtilityTests {
    [Test]
    public async Task WriteAndReadWorkFileAsync() {
        var fileFQN = System.IO.Path.GetTempFileName();
        SAWork given = new SAWork() {
            Id = Guid.NewGuid(),
            Name = fileFQN,
            Description = fileFQN
        };
        await FilePersistenceUtility.WriteWorkFileAsync(fileFQN, given, CancellationToken.None).ConfigureAwait(false);
        var actual = await FilePersistenceUtility.ReadWorkFileAsync(fileFQN, CancellationToken.None).ConfigureAwait(false);
        await Assert.That(actual).IsNotNull().And.IsEquatableTo(given);
    }

    [Test]
    public async Task NormalizeFolderNameTests() {
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("abc")).IsEqualTo("abc");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("abc ")).IsEqualTo("abc");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName(" abc")).IsEqualTo("abc");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName(" abc ")).IsEqualTo("abc");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab/c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab\\c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab\\\\c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab:c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab?c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab<c")).IsEqualTo("ab_c");
        await Assert.That(FilePersistenceUtility.NormalizeFolderName("ab>c")).IsEqualTo("ab_c");
    }
}
