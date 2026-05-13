using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using FitLife.PersonalTrainer.API.Models;
using FitLife.PersonalTrainer.API.Repositories;
using FitLife.PersonalTrainer.API.Services;

namespace PersonalTrainerService.Tests;

[TestClass]
public class TrainerServiceTests
{
    private Mock<ITrainerRepository> _repositoryMock;
    private Mock<ILogger<TrainerService>> _loggerMock;
    private TrainerService _service;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<ITrainerRepository>();
        _loggerMock = new Mock<ILogger<TrainerService>>();
        _service = new TrainerService(_repositoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenTrainerExists_ReturnsTrainer()
    {
        // Arrange
        var trainerId = Guid.NewGuid();
        var expectedTrainer = new Trainer
        {
            Id = trainerId,
            FirstName = "Jens",
            LastName = "Hansen",
            Email = "jens@fitlife.dk"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(trainerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrainer);

        // Act
        var result = await _service.GetByIdAsync(trainerId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(trainerId, result.Id);
        Assert.AreEqual("Jens", result.FirstName);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenTrainerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var trainerId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(trainerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trainer?)null);

        // Act
        var result = await _service.GetByIdAsync(trainerId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task CreateAsync_SetsIdAndIsActive()
    {
        // Arrange
        var trainer = new Trainer
        {
            FirstName = "Jens",
            LastName = "Hansen",
            Email = "jens@fitlife.dk"
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Trainer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(trainer);

        // Assert
        Assert.AreNotEqual(Guid.Empty, result.Id);
        Assert.IsTrue(result.IsActive);
    }

    [TestMethod]
public async Task DeleteAsync_WhenTrainerDoesNotExist_ThrowsKeyNotFoundException()
{
    // Arrange
    var trainerId = Guid.NewGuid();
    _repositoryMock.Setup(r => r.GetByIdAsync(trainerId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Trainer?)null);

    // Act & Assert
    try
    {
        await _service.DeleteAsync(trainerId);
        Assert.Fail("En KeyNotFoundException var forventet, men blev ikke kastet.");
    }
    catch (KeyNotFoundException)
    {
        // Succes - fejlen blev fanget som forventet
    }
}
    [TestMethod]
    public async Task CreateAsync_WhenCalled_CallsRepository()
    {
        // Arrange
        var trainer = new Trainer
        {
            FirstName = "Jens",
            LastName = "Hansen",
            Email = "jens@fitlife.dk"
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Trainer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(trainer);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Trainer>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}