using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Data.Entities;
using VerificationProvider.Infrastructure.Models;
using VerificationProvider.Infrastructure.Services;

namespace VerificationProvider.Tests
{
    public class ValidateCodeService_Tests
    {
        private readonly Mock<ILogger<ValidateCodeService>> _loggerMock;

        private readonly DataContext _context;

        public ValidateCodeService_Tests()
        {

            _loggerMock = new Mock<ILogger<ValidateCodeService>>();


            var options = new DbContextOptionsBuilder<DataContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            _context = new DataContext(options);

        }

        [Fact]
        public async Task ValidateCodeAsync_ShouldReturnFalse_WhenCodeIsInvalid()
        {
            // Arrange
            var validateRequest = new ValidateCodeRequest { Email = "test@example.com", Code = "123456" };


            var validateCodeService = new ValidateCodeService(_loggerMock.Object, _context);

            // Act
            var result = await validateCodeService.ValidateCodeAsync(validateRequest);

            // Assert
            Assert.False(result, "Expected validation to fail but it succeeded");
        }


        [Fact]
        public async Task ValidateCodeAsync_ShouldReturnTrue_WhenCodeIsValid()
        {
            // Arrange
            var validateRequest = new ValidateCodeRequest { Email = "test@example.com", Code = "123456" };


            var validateCodeService = new ValidateCodeService(_loggerMock.Object, _context);

            // Add a valid request to the in-memory database
            _context.IncomingRequests.Add(new IncomingRequestEntity { Email = validateRequest.Email, Code = validateRequest.Code });
            await _context.SaveChangesAsync();

            // Act
            var result = await validateCodeService.ValidateCodeAsync(validateRequest);

            // Assert
            Assert.True(result, "Expected validation to succeed but it failed");

            // Verify that the request was removed from the database
            var entity = await _context.IncomingRequests.FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);
            Assert.Null(entity);
        }
    }
}
