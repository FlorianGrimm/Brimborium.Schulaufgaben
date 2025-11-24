// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public class EditingLucanetCacheTests {

    [Test]
    public async Task EditingLucanetCacheT() {
        var projectFolder=TestUtilities.GetProjectFolder();
        var cacheFolder = @"c:\temp\cacheFolder";
        EditingLucanetCache editingLucanetCache = new EditingLucanetCache(cacheFolder);
        //editingLucanetCache.Write();
        await Task.CompletedTask;
    }
    
}