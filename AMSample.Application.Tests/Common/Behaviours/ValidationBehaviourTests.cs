namespace AMSample.Application.Tests.Common.Behaviours;

public class ValidationBehaviourTests
{
    private readonly Mock<IValidator<TestRequest>> _mockValidator;
    private readonly List<IValidator<TestRequest>> _validators;
    private readonly ValidationBehaviour<TestRequest, TestResponse> _behaviour;

    public record TestRequest(string Name, int Age) : IRequest<TestResponse>;

    public record TestResponse(string Result);

    public ValidationBehaviourTests()
    {
        _mockValidator = new Mock<IValidator<TestRequest>>();
        _validators = new List<IValidator<TestRequest>> {_mockValidator.Object};
        _behaviour = new ValidationBehaviour<TestRequest, TestResponse>(_validators);
    }

    [Fact]
    public async Task Handle_WithNoValidators_ProceedsToNext()
    {
        // Arrange
        var emptyValidators = Enumerable.Empty<IValidator<TestRequest>>();
        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(emptyValidators);
        var request = new TestRequest("Test", 25);
        var expectedResponse = new TestResponse("Success");

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(expectedResponse);

        // Act
        var result = await behaviour.Handle(request, Next, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _mockValidator.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ProceedsToNext()
    {
        // Arrange
        var request = new TestRequest("Valid Name", 25);
        var expectedResponse = new TestResponse("Success");

        // Mock validator returns no errors
        _mockValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(expectedResponse);

        // Act
        var result = await _behaviour.Handle(request, Next, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _mockValidator.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new TestRequest("", -5); // Invalid data
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Age", "Age must be positive")
        };

        _mockValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(new TestResponse("Success"));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _behaviour.Handle(request, Next, CancellationToken.None));

        _mockValidator.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_AllValidatorsAreCalled()
    {
        // Arrange
        var secondValidator = new Mock<IValidator<TestRequest>>();
        var validators = new List<IValidator<TestRequest>>
        {
            _mockValidator.Object,
            secondValidator.Object
        };

        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest("Test", 25);
        var expectedResponse = new TestResponse("Success");

        _mockValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        secondValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(expectedResponse);

        // Act
        var result = await behaviour.Handle(request, Next, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _mockValidator.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        secondValidator.Verify(
            v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleValidatorsAndOneFails_ThrowsValidationExceptionWithAllErrors()
    {
        // Arrange
        var secondValidator = new Mock<IValidator<TestRequest>>();
        var validators = new List<IValidator<TestRequest>>
        {
            _mockValidator.Object,
            secondValidator.Object
        };

        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest("", -5);

        var firstValidatorErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };

        var secondValidatorErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Age", "Age must be positive"),
            new ValidationFailure("Age", "Age must be greater than 0")
        };

        _mockValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(firstValidatorErrors));
        secondValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(secondValidatorErrors));

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(new TestResponse("Success"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behaviour.Handle(request, Next, CancellationToken.None));

        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
        exception.Errors.Should().Contain(e => e.PropertyName == "Age" && e.ErrorMessage == "Age must be positive");
        exception.Errors.Should()
            .Contain(e => e.PropertyName == "Age" && e.ErrorMessage == "Age must be greater than 0");
    }

    [Fact]
    public async Task Handle_WhenValidatorThrowsException_ExceptionIsPropagated()
    {
        // Arrange
        var request = new TestRequest("Test", 25);

        _mockValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Validator failed"));

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(new TestResponse("Success"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _behaviour.Handle(request, Next, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMixedValidatorResults_CombinesErrorsFromFailedValidators()
    {
        // Arrange
        var passingValidator = new Mock<IValidator<TestRequest>>();
        var failingValidator = new Mock<IValidator<TestRequest>>();
        var validators = new List<IValidator<TestRequest>>
        {
            passingValidator.Object,
            failingValidator.Object
        };

        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest("", 25);

        passingValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // No errors

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name cannot be empty")
        };

        failingValidator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        Task<TestResponse> Next(CancellationToken ctx) => Task.FromResult(new TestResponse("Success"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behaviour.Handle(request, Next, CancellationToken.None));

        exception.Errors.Should().HaveCount(1);
        exception.Errors.First().PropertyName.Should().Be("Name");
        exception.Errors.First().ErrorMessage.Should().Be("Name cannot be empty");
    }
}