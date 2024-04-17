using JixMinApi.Features.Todo.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var expectedBadRequest = TypedResults.BadRequest<ValidationErrorDto>(
            new ValidationErrorDto(
                [new ValidationErrorItem("id", "id must not be an empty guid.")]
            ));

        // Act
        var result = await TodoEndpoints.GetTodoByIdAsync(emptyId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<BadRequest<ValidationErrorDto>, NotFound, Ok<TodoDto>>>(result);
        var badrequest = (BadRequest<ValidationErrorDto>)result.Result;
        Assert.NotNull(badrequest);
    }

    [Fact]
    public async Task GetTodoByIdAsync_Returns_NotFound_When_Todo_Not_Found()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var nonExistentId = Guid.NewGuid(); // Assuming this id doesn't exist
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllTodosQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<TodoDto>());

        // Act
        var result = await TodoEndpoints.GetTodoByIdAsync(nonExistentId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<BadRequest<ValidationErrorDto>, NotFound, Ok<TodoDto>>>(result);
        var notFoundResult = (NotFound)result.Result;

        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetTodoByIdAsync_Returns_Ok_When_Todo_Found()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var existingId = Guid.NewGuid(); // Assuming this id exists
        var todoDto = new TodoDto(existingId, "", false); // Assuming todo with this id exists

        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllTodosQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<TodoDto>() {
                todoDto
            });

        // Act
        var result = await TodoEndpoints.GetTodoByIdAsync(existingId, mediatorMock.Object);

        // Assert
        Assert.IsType<Results<BadRequest<ValidationErrorDto>, NotFound, Ok<TodoDto>>>(result);
        var okResult = (Ok<TodoDto>)result.Result;

        Assert.NotNull(okResult);
        Assert.Equal(todoDto, okResult.Value);
    }
}