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
    public async void Handle_OkTest()
    {
        Setup();
        const string todoName = "Test";

        var result = await sut.Handle(new CreateTodoCommand(todoName, true), default);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(todoName, result.Value.Name);
        Assert.True(result.Value.IsComplete);
    }

    [Theory()]
    [InlineData("")]
    [InlineData("TES")]
    public async void Handle_ReturnsFailureWhenNameIsNotValid(string name)
    {
        Setup();

        var result = await sut.Handle(new CreateTodoCommand(name, true), default);

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Any());
    }
}