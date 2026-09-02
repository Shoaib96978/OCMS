using OCMS.DTOs.User;
using OCMS.Mappers.Auth;
using OCMS.Entities;
using OCMS.Repositories;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;

namespace OCMS.Services.Implementations;

public class UserService(
    IRepository<User> userRepo,
    IRepository<UserCredential> credRepo) : IUserService
{
    private readonly IRepository<User> _userRepo = userRepo;
    private readonly IRepository<UserCredential> _credRepo = credRepo;

    // ===================== GET PROFILE =====================
    public async Task<AppResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null)
            return AppResponse.Fail("User not found.");

        var dto = new GetProfileDto(
            FullName: user.FullName,
            Email: user.Email,
            ImageLink: user.ImageLink
        );

        return AppResponse.Ok("Profile fetched.", data: dto);
    }

    // ===================== UPDATE PROFILE =====================
    public async Task<AppResponse> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null)
            return AppResponse.Fail("User not found.");

        // Email already taken by someone else?
        var emailTaken = await _userRepo.ExistsAsync(
            u => u.Email.ToLower() == dto.Email.ToLower()
              && u.UserId != userId);

        if (emailTaken)
            return AppResponse.Fail("This email is already in use.");

        user.FullName = dto.FullName;
        user.Email = dto.Email;

        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return AppResponse.Ok("Profile updated successfully.");
    }

    // ===================== CHANGE PASSWORD =====================
    public async Task<AppResponse> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return AppResponse.Fail("New passwords do not match.");

        var credential = await _credRepo.GetFirstOrDefaultAsync(
            c => c.UserId == userId);

        if (credential == null)
            return AppResponse.Fail("User credentials not found.");

        // Verify current password
        var isValid = PasswordServices.VerifyPassword(
            dto.CurrentPassword,
            credential.PasswordHash,
            credential.PasswordSalt);

        if (!isValid)
            return AppResponse.Fail("Current password is incorrect.");

        // Update password
        UserMappers.MapToUpdatePassword(credential, dto.NewPassword);
        await _credRepo.UpdateAsync(credential);
        await _userRepo.SaveChangesAsync();
        return AppResponse.Ok("Password changed successfully.");
    }

    // ===================== UPLOAD IMAGE =====================
    public async Task<AppResponse> UploadImageAsync(Guid userId, IFormFile imageFile)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null)
            return AppResponse.Fail("User not found.");

        // Delete old image
        ImageService.Delete(user.ImageLink);

        // Save new image
        var imagePath = await ImageService.SaveAsync(imageFile, folder: "users");

        user.ImageLink = imagePath;
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return AppResponse.Ok("Profile image updated.", data: new { imagePath });
    }

    // ===================== GET ALL USERS =====================
    public async Task<AppResponse> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllWithIncludeAsync(
            u => u.UserRoles,
            u => u.Complaints
        );

        return AppResponse.Ok("Users fetched.", data: users.MapToGetAllUserDto());
    }

    // ===================== TOGGLE STATUS =====================
    public async Task<AppResponse> ToggleStatusAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null)
            return AppResponse.Fail("User not found.");

        user.Status = user.Status == UserStatus.Active
            ? UserStatus.Inactive
            : UserStatus.Active;

        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return AppResponse.Ok(
            $"User {user.Status.ToString().ToLower()} successfully.",
            data: new { status = user.Status }
        );
    }

    // ===================== DELETE USER =====================
    public async Task<AppResponse> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null)
            return AppResponse.Fail("User not found.");

        // Profile image delete karo
        ImageService.Delete(user.ImageLink);

        // Credentials + Roles bhi delete honge (cascade)
        await _userRepo.DeleteByEntityAsync(user);
        await _userRepo.SaveChangesAsync();
        return AppResponse.Ok("User deleted successfully.");
    }
}