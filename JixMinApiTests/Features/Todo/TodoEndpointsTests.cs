using JixMinApi.Features.Todo.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace JixMinApi.Features.Todo.Tests;

public class TodoEndpointsTests
{
    [Fact]
    public async Task GetTodoByIdAsync_Returns_BadRequest_When_Id_Is_Empty()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var emptyId = Guid.Empty;
        var expectedErrors = new Dictionary<string, string[]>
        {
            ["id"] = ["id parameter must not be an empty guid."],
        };
        mediatorMock.Setup(m => m.Send(It.IsAny<GetTodoByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(new Result<TodoDto>("id", "id parameter must not be an empty guid."));

        // Act
        var response = await TodoEndpoints.GetTodoByIdAsync(emptyId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<ValidationProblem, NotFound, Ok<TodoDto>>>(response);

        var result = (ValidationProblem)response.Result;
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

        var value = Assert.IsType<HttpValidationProblemDetails>(result.ProblemDetails);
        Assert.True(value.Errors.Any());
        Assert.Equal(expectedErrors.FirstOrDefault(), value.Errors.FirstOrDefault());
    }

    [Fact]
    public async Task GetTodoByIdAsync_Returns_NotFound_When_Todo_Not_Found()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var nonExistentId = Guid.NewGuid(); // Assuming this id doesn't exist
        mediatorMock.Setup(m => m.Send(It.IsAny<GetTodoByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<TodoDto>.NotFound<TodoDto>());

        // Act
        var response = await TodoEndpoints.GetTodoByIdAsync(nonExistentId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<ValidationProblem, NotFound, Ok<TodoDto>>>(response);

        var result = (NotFound)response.Result;
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetTodoByIdAsync_Returns_Ok_When_Todo_Found()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var existingId = Guid.NewGuid(); // Assuming this id exists
        var todoDto = new TodoDto(existingId, "Test name", false); // Assuming todo with this id exists

        mediatorMock.Setup(m => m.Send(It.IsAny<GetTodoByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(new Result<TodoDto>(todoDto));

        // Act
        var response = await TodoEndpoints.GetTodoByIdAsync(existingId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<ValidationProblem, NotFound, Ok<TodoDto>>>(response);

        var result = (Ok<TodoDto>)response.Result;
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(todoDto, result.Value);
    }
}