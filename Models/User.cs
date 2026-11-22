namespace TraGo.Models
{
    public enum UserRole
    {
        Admin,
        Customer,
        GateOfficer
    }

    public abstract class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public abstract UserRole Role { get; }
    }

    public class Admin : User
    {
        public string StationCode { get; set; } = "GMR";
        public override UserRole Role => UserRole.Admin;
    }

    public class Customer : User
    {
        public string Nik { get; set; } = string.Empty;
        public override UserRole Role => UserRole.Customer;
    }

    public class GateOfficer : User
    {
        public string StationCode { get; set; } = "GMR";
        public override UserRole Role => UserRole.GateOfficer;
    }

    // Factory Method Pattern
    public static class UserFactory
    {
        public static User CreateUser(
            UserRole role,
            string username,
            string password,
            string fullName,
            string extraInfo = "",
            int id = 0)
        {
            return role switch
            {
                UserRole.Admin => new Admin
                {
                    Id = id,
                    Username = username,
                    Password = password,
                    FullName = fullName,
                    StationCode = string.IsNullOrWhiteSpace(extraInfo) ? "GMR" : extraInfo.ToUpper()
                },
                UserRole.Customer => new Customer
                {
                    Id = id,
                    Username = username,
                    Password = password,
                    FullName = fullName,
                    Nik = extraInfo
                },
                UserRole.GateOfficer => new GateOfficer
                {
                    Id = id,
                    Username = username,
                    Password = password,
                    FullName = fullName,
                    StationCode = string.IsNullOrWhiteSpace(extraInfo) ? "GMR" : extraInfo.ToUpper()
                },
                _ => throw new ArgumentOutOfRangeException(nameof(role), $"Role '{role}' tidak dikenali.")
            };
        }
    }
}