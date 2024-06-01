using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Data.Entities;
using VerificationProvider.Infrastructure.Models;
using VerificationProvider.Infrastructure.Services;



namespace VerificationProvider.Tests;

public class VerificationService_Tests
{
    private readonly Mock<ILogger<VerificationService>> _loggerMock;

    private readonly DataContext _context;

    public VerificationService_Tests()
    {

        _loggerMock = new Mock<ILogger<VerificationService>>();


        var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _context = new DataContext(options);

    }


    [Fact]
    public async Task SaveVerificationRequest_ShouldSaveNewRequest_WhenRequestDoesNotExist()
    {
        // Arrange
        var verificationRequest = new VerificationRequest { Email = "test@example.com" };
        var code = "123456";

        var verificationService = new VerificationService(_context, _loggerMock.Object);
        // Act
        var result = await verificationService.SaveVerificationRequest(verificationRequest, code);

        // Assert
        Assert.True(result.IsSuccess, $"Expected IsSuccess to be true but was {result.IsSuccess}. Error: {result.Error}");
        Assert.True(result.Result, "Expected Result to be true but was false");

        var savedRequest = await _context.IncomingRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
        Assert.NotNull(savedRequest);
        Assert.Equal(code, savedRequest.Code);
    }


    [Fact]
    public async Task SaveVerificationRequest_ShouldUpdateExistingRequest_WhenRequestExists()
    {
        // Arrange
        var verificationRequest = new VerificationRequest { Email = "test@example.com" };
        var oldCode = "oldcode";
        var newCode = "123456";


        var verificationService = new VerificationService(_context, _loggerMock.Object);


        var addRequestResult = await verificationService.SaveVerificationRequest(verificationRequest, oldCode);
        Assert.True(addRequestResult.IsSuccess, $"Initial save failed. Error: {addRequestResult.Error}");

        // Act
        var updateRequestResult = await verificationService.SaveVerificationRequest(verificationRequest, newCode);

        // Assert
        Assert.True(updateRequestResult.IsSuccess, $"Expected IsSuccess to be true but was {updateRequestResult.IsSuccess}. Error: {updateRequestResult.Error}");
        Assert.True(updateRequestResult.Result, "Expected Result to be true but was false");


        //using var context = contextFactoryMock.Object.CreateDbContext();
        var updatedRequest = await _context.IncomingRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
        Assert.NotNull(updatedRequest);
        Assert.Equal(newCode, updatedRequest.Code);
        Assert.True(updatedRequest.ExpiryDate > DateTime.Now);
    }



}







