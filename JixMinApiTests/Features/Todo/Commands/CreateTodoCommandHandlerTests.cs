using JixMinApiTests.Features.Todo;
using Xunit;
using Assert = Xunit.Assert;

namespace JixMinApi.Features.Todo.Commands.Tests;

public class CreateTodoCommandHandlerTests
{
    private CreateTodoCommandHandler sut;

    internal void Setup()
    {
        var (db, logger) = TestSetup.CreateTestMocks<CreateTodoCommandHandler>();
        sut = new CreateTodoCommandHandler(db, logger);
    }

    [Fact()]
    public void CreateTodoCommandHandlerTest()
    {
        Setup();
        Assert.NotNull(sut);
    }

    [Fact()]
    public async void HandleTest()
    {
        Setup();
        var input = new TodoCreateDto("Test", true);
        var result = await sut.Handle(new CreateTodoCommand(input), default);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(input.Name, result.Value.Name);
        Assert.Equal(input.IsComplete, result.Value.IsComplete);
    }
}