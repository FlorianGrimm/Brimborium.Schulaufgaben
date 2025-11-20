// MIT - Florian Grimm

namespace Schulaufgaben;

public class StartupTests {
    [Test]
    public async Task VerifyChecksRun() => await VerifyChecks.Run();
}
