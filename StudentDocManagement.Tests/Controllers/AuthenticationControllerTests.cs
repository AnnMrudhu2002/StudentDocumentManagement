using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocumentManagement.Controllers;

namespace StudentDocManagement.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<ITokenRepository> _mockTokenRepository;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null
            );

            _mockTokenRepository = new Mock<ITokenRepository>();

            _controller = new AuthenticationController(_mockUserManager.Object, _mockTokenRepository.Object, null);
        }

        #region Register Tests

        [Fact]
        public async Task Register_NewUser_ShouldReturnOk()
        {
            var request = new RegisterDto
            {
                Email = "newuser@test.com",
                FullName = "New User",
                RegisterNo = "REG001",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.Users).Returns(new List<ApplicationUser>().AsQueryable());
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Student"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Registration successful", okResult.Value.ToString());
        }

        [Fact]
        public async Task Register_ExistingUser_PendingOrApproved_ShouldReturnBadRequest()
        {
            var existingUser = new ApplicationUser { Email = "exist@test.com", StatusId = 1 };
            var request = new RegisterDto
            {
                Email = "exist@test.com",
                FullName = "Test User",
                RegisterNo = "REG002",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

            var result = await _controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already exists", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Register_ExistingUser_Rejected_ShouldReturnOkAfterUpdate()
        {
            var rejectedUser = new ApplicationUser { Email = "reject@test.com", StatusId = 3 };
            var request = new RegisterDto
            {
                Email = "reject@test.com",
                FullName = "Updated User",
                RegisterNo = "REG003",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(rejectedUser);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(rejectedUser))
                .ReturnsAsync("reset-token");
            _mockUserManager.Setup(x => x.ResetPasswordAsync(rejectedUser, "reset-token", request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.UpdateAsync(rejectedUser)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.IsInRoleAsync(rejectedUser, "Student")).ReturnsAsync(false);
            _mockUserManager.Setup(x => x.AddToRoleAsync(rejectedUser, "Student")).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Registration successful", okResult.Value.ToString());
        }

        #endregion

        #region Admin Register Tests

        [Fact]
        public async Task AdminRegister_NewUser_ShouldReturnOk()
        {
            var request = new AdminRegisterDto
            {
                Email = "admin@test.com",
                FullName = "Admin User",
                Password = "Admin123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.AdminRegister(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Admin Registration successful", okResult.Value.ToString());
        }

        [Fact]
        public async Task AdminRegister_ExistingEmail_ShouldReturnBadRequest()
        {
            var request = new AdminRegisterDto
            {
                Email = "admin@test.com",
                FullName = "Admin User",
                Password = "Admin123!"
            };

            var existingUser = new ApplicationUser { Email = "admin@test.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

            var result = await _controller.AdminRegister(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already exists", badRequest.Value.ToString());
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_ValidUser_ShouldReturnToken()
        {
            var user = new ApplicationUser { Email = "login@test.com", StatusId = 2 };

            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Student" });

            _mockTokenRepository
                .Setup(x => x.GetToken(user, It.IsAny<IList<string>>()))
                .ReturnsAsync("fake-token");

            var request = new LoginDto { Email = "login@test.com", Password = "Password123!" };

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("fake-token", loginResponse.Token);
        }


        [Fact]
        public async Task Login_InvalidPassword_ShouldReturnUnauthorized()
        {
            var user = new ApplicationUser { Email = "login@test.com", StatusId = 2 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

            var request = new LoginDto { Email = "login@test.com", Password = "wrong" };
            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_PendingUser_ShouldReturnUnauthorized()
        {
            var user = new ApplicationUser { Email = "pending@test.com", StatusId = 1 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);

            var request = new LoginDto { Email = "pending@test.com", Password = "Password123!" };
            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_RejectedUser_ShouldReturnUnauthorized()
        {
            var user = new ApplicationUser { Email = "rejected@test.com", StatusId = 3 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);

            var request = new LoginDto { Email = "rejected@test.com", Password = "Password123!" };
            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        #endregion
    }
}
