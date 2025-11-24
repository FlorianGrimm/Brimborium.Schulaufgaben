// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class FilePersistenceUtilityTests {
    [Test]
    public async Task WriteAndReadWorkFileAsync() {
        var fileFQN = System.IO.Path.GetTempFileName();
        SADocument given = new SADocument() {
            Id = Guid.NewGuid(),
            Name = fileFQN,
            Description = fileFQN
        };
        await FilePersistenceUtility.WriteDocumentAsync(fileFQN, given, CancellationToken.None).ConfigureAwait(false);
        var actual = await FilePersistenceUtility.ReadDocumentAsync(fileFQN, CancellationToken.None).ConfigureAwait(false);
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
    [Test]
    public async Task GetMD5Test() {
        await Assert.That(FilePersistenceUtility.GetMD5("a")).IsEqualTo("0CC175B9C0F1B6A831C399E269772661");
    }
}
