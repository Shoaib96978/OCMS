using OCMS.DTOs.Auth;
using OCMS.DTOs.User;
using OCMS.Entities;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;

namespace OCMS.Mappers.Auth
{
    public static class UserMappers
    {
        public static User MapToAddUser(this RegisterDto dto)
        {
            //var ImagePath = ImageService.SaveAndReturnPath(dto.ImageFile);
            var users = new User()
            {
                UserId = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Status = UserStatus.Active,
                CreatedDate = DateTime.UtcNow,
                ImageLink = string.Empty,
            };
            return users;
        }

        public static UserCredential AssignCredentials(this User user, string password)
        {
            PasswordServices.GenerateHashAndSalt(password, out byte[] Hash, out byte[] Salt);
            var UserCredentialDomain = new UserCredential()
            {
                CredentialId = Guid.NewGuid(),
                UserId = user.UserId,
                PasswordHash = Hash,
                PasswordSalt = Salt,
                Otp = null
            };
            return UserCredentialDomain;
        }

        public static UserRole AssignRole(this User user)
        {
            return new UserRole()
            {
                UserRoleId = Guid.NewGuid(),
                UserId = user.UserId,
                Role = AppRoles.User
            };

        }
        public static void MapToUpdatePassword(UserCredential userCredential, string password)
        {
            PasswordServices.GenerateHashAndSalt(password, out byte[] Hash, out byte[] Salt);

            userCredential.PasswordHash = Hash;
            userCredential.PasswordSalt = Salt;
            userCredential.Otp = null;
        }

        public static IEnumerable<GetUserDto> MapToGetAllUserDto(this IEnumerable<User> users)
        {
            return users.Select(u => new GetUserDto(
                                      u.UserId,
                                      FullName: u.FullName,
                                      Email: u.Email,
                                      Role: u.UserRoles.FirstOrDefault()?.Role ?? AppRoles.User,
                                      Status: u.Status,
                                      ImageLink: u.ImageLink,
                                      CreatedDate: u.CreatedDate,
                                      LastLoginDate: u.LastLoginDate,
                                      TotalComplaints: u.Complaints.Count
                                 )).ToList();
        }
    }
}
