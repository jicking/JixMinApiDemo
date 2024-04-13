using JixMinApi.Features.Todo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JixMinApiTests.Features.Todo;

public static class TestSetup
{
    public static (TodoDb, ILogger<T>) CreateTestMocks<T>()
    {
        var logger = NullLogger<T>.Instance;

        //	Set EF
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new TodoDb(options);
        db.SeedTestData();

        return (db, logger);
    }
}
