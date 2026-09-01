using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OCMS.DTOs.Auth;
using OCMS.Entities;
using OCMS.Mappers.Auth;
using OCMS.Repositories;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;
using System.Security.Claims;

namespace OCMS.Services.Implementations
{
    public class AuthService(
        IRepository<User> userRepo,
        IRepository<UserCredential> credRepo,
        IRepository<UserRole> roleRepo,
        IHttpContextAccessor http) : IAuthService
    {
        private readonly IRepository<User> _userRepo = userRepo;
        private readonly IRepository<UserCredential> _credRepo = credRepo;
        private readonly IRepository<UserRole> _roleRepo = roleRepo;
        private readonly IHttpContextAccessor _http = http;

        // ===================== REGISTER =====================
        public async Task<AppResponse> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepo.GetFirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

            if (existingUser != null)
                return AppResponse.Fail("This email is already registered.");

            var user = dto.MapToAddUser();
            await _userRepo.AddAsync(user);
            await _credRepo.AddAsync(user.AssignCredentials(dto.Password));
            await _roleRepo.AddAsync(user.AssignRole());
            return await _userRepo.SaveChangesAsync() > 0 ?
             AppResponse.Ok(
                message: "Account created successfully! Please login.",
                redirectUrl: "/Auth/LoginPage"
            )
             : AppResponse.Fail("Internal Server Error");
        }

        // ===================== LOGIN =====================
        public async Task<AppResponse> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetFirstOrDefaultAsync(
                u => u.Email.ToLower() == dto.Email.ToLower());

            if (user == null)
                return AppResponse.Fail("Invalid email or password.");

            var credential = await _credRepo.GetFirstOrDefaultAsync(
                c => c.UserId == user.UserId);

            if (credential == null)
                return AppResponse.Fail("Invalid email or password.");

            if (!PasswordServices.VerifyPassword(
                    dto.Password,
                    credential.PasswordHash,
                    credential.PasswordSalt))
                return AppResponse.Fail("Invalid email or password.");

            // User inactive check
            if (user.Status == UserStatus.Inactive)
                return AppResponse.Fail("Your account has been deactivated.");

            var userRole = await _roleRepo.GetFirstOrDefaultAsync(
                                                             r => r.UserId == user.UserId);

            var roleName = userRole?.Role == AppRoles.Admin
                ? AppRoles.Admin.ToString()
                : AppRoles.User.ToString();

            //  Claims 
            var claims = new List<Claim>
            {
                new(AppClaims.UserId,   user.UserId.ToString()),
                new(AppClaims.FullName, user.FullName),
                new(AppClaims.Email,    user.Email),
                new(ClaimTypes.Role,    roleName),
                new(ClaimTypes.Name,    user.FullName),
                new("ImageLink", user.ImageLink ?? string.Empty),
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _http.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = dto.RememberMe,
                    ExpiresUtc = dto.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(8)
                });

            // Last login update
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();
            var redirectUrl = roleName == AppRoles.Admin.ToString()
                ? "/AdminDashboard/Index"
                : "/Home/Index";

            return AppResponse.Ok("Login successful! Welcome back.",
                redirectUrl: redirectUrl);
        }
    }
}
