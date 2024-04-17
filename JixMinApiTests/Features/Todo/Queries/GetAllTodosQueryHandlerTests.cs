using JixMinApiTests.Features.Todo;
using Xunit;
using Assert = Xunit.Assert;

namespace JixMinApi.Features.Todo.Queries.Tests;

public class GetAllTodosQueryHandlerTests
{
    private GetAllTodosQueryHandler sut;

    internal void Setup()
    {
        var (db, logger) = TestSetup.CreateTestMocks<GetAllTodosQueryHandler>();
        sut = new GetAllTodosQueryHandler(db, logger);
    }

    [Fact()]
    public void GetAllTodosQueryHandlerTest()
    {
        Setup();
        Assert.NotNull(sut);
    }

    [Fact()]
    public async void HandleTest()
    {
        Setup();
        var result = await sut.Handle(new GetAllTodosQuery(), default);
        Assert.NotEmpty(result);
    }
}