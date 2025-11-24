// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben;

public class StartupTests {
    [Test]
    public async Task VerifyChecksRun() => await VerifyChecks.Run();
}
