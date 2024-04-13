using JixMinApiTests.Features.Todo;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
        Assert.IsNotNull(sut);
    }

    [Fact()]
    public async void HandleTest()
    {
        Setup();
        var input = new TodoCreateDto("Test", true);
        var result = await sut.Handle(new CreateTodoCommand(input), default);

        Xunit.Assert.NotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(input.Name, result.Value.Name);
        Assert.AreEqual(input.IsComplete, result.Value.IsComplete);
    }
}