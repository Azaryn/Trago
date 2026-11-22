using TraGo.Models;

namespace TraGo.Views
{
    public class AuthView
    {
        public void ClearAndBanner(string? statusMessage = null)
        {
            Console.Write("\x1b[2J\x1b[3J\x1b[H");
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(statusMessage))
            {
                Console.WriteLine(statusMessage);
                Console.WriteLine(new string('=', 85));
            }
        }

        public string ShowGuestMenu(string? statusMessage = null)
        {
            ClearAndBanner(statusMessage);
            Console.WriteLine("===============================");
            Console.WriteLine("   TRAGO - CLI TRAIN TICKET    ");
            Console.WriteLine("===============================");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register (Customer)");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih menu: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public (string username, string password) ShowLoginForm()
        {
            Console.WriteLine("\n--- LOGIN TRAGO ---");
            Console.Write("Username : ");
            string username = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Password : ");
            string password = Console.ReadLine()?.Trim() ?? "";
            return (username, password);
        }

        public (string username, string password, string fullName, string nik) ShowRegisterForm()
        {
            Console.WriteLine("\n--- REGISTRASI CUSTOMER BARU ---");
            Console.Write("Username      : ");
            string username = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Password      : ");
            string password = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Nama Lengkap  : ");
            string fullName = Console.ReadLine()?.Trim() ?? "";
            Console.Write("NIK (16 digit): ");
            string nik = Console.ReadLine()?.Trim() ?? "";
            return (username, password, fullName, nik);
        }

        public string ShowRoleMenu(User user, string? statusMessage = null)
        {
            ClearAndBanner(statusMessage);
            switch (user)
            {
                case Admin admin:
                    Console.WriteLine($"=== MENU ADMIN ({admin.FullName} | Stasiun: {admin.StationCode}) ===");
                    Console.WriteLine("1. Manajemen Akun Pengguna");
                    Console.WriteLine("2. Kelola Jadwal Kereta (TrainController)");
                    Console.WriteLine("0. Logout");
                    break;

                case Customer customer:
                    Console.WriteLine($"=== MENU CUSTOMER ({customer.FullName} | NIK: {customer.Nik}) ===");
                    Console.WriteLine("1. Profil & Manajemen Akun Saya");
                    Console.WriteLine("2. Cari & Pesan Tiket (TrainController)");
                    Console.WriteLine("3. Lihat Tiket & Kode Redeem Saya");
                    Console.WriteLine("0. Logout");
                    break;

                case GateOfficer officer:
                    Console.WriteLine($"=== MENU GATE OFFICER ({officer.FullName} | Stasiun: {officer.StationCode}) ===");
                    Console.WriteLine("1. Validasi Boarding (Input Kode Redeem)");
                    Console.WriteLine("2. Profil & Manajemen Akun Saya");
                    Console.WriteLine("0. Logout");
                    break;
            }

            Console.Write("Pilih menu: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public void DisplayUserProfile(User user, string title = "DETAIL PROFIL AKUN")
        {
            Console.WriteLine($"\n--- {title} ---");
            Console.WriteLine($"ID       : {user.Id}");
            Console.WriteLine($"Username : {user.Username}");
            Console.WriteLine($"Nama     : {user.FullName}");
            Console.WriteLine($"Role     : {user.Role}");
            if (user is Customer c) Console.WriteLine($"NIK      : {c.Nik}");
            if (user is GateOfficer g) Console.WriteLine($"Stasiun  : {g.StationCode}");
            if (user is Admin a) Console.WriteLine($"Stasiun  : {a.StationCode}");
        }

        public string ShowSelfAccountMenu(User user, string? statusMessage = null)
        {
            ClearAndBanner(statusMessage);
            DisplayUserProfile(user, "PROFIL & MANAJEMEN AKUN SAYA");

            Console.WriteLine("\n1. Ubah Nama Lengkap");
            Console.WriteLine("2. Ubah Password");
            Console.WriteLine("0. Kembali");
            Console.Write("Pilih: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        // Tabel daftar akun selalu tampil live di atas menu Admin
        public string ShowAdminUserMenu(List<User> users, string? statusMessage = null)
        {
            ClearAndBanner(statusMessage);
            Console.WriteLine("--- MANAJEMEN AKUN PENGGUNA (ADMIN) ---");
            DisplayUserList(users);

            Console.WriteLine("\n1. Tambah Akun Petugas (Gate Officer)");
            Console.WriteLine("2. Ubah Nama / Password Pengguna");
            Console.WriteLine("3. Hapus Akun Pengguna");
            Console.WriteLine("0. Kembali");
            Console.Write("Pilih: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public void DisplayUserList(List<User> users)
        {
            Console.WriteLine("\nID  | USERNAME     | ROLE         | NAMA LENGKAP              | INFO (NIK/STASIUN)");
            Console.WriteLine(new string('-', 85));
            foreach (var u in users)
            {
                string info = u switch
                {
                    Customer c => c.Nik,
                    GateOfficer g => g.StationCode,
                    Admin a => a.StationCode,
                    _ => "-"
                };
                Console.WriteLine($"{u.Id,-3} | {u.Username,-12} | {u.Role,-12} | {u.FullName,-25} | {info}");
            }
        }

        public (string username, string password, string fullName, string station) ShowCreateOfficerForm()
        {
            Console.WriteLine("\n--- TAMBAH PETUGAS (GATE OFFICER) ---");
            Console.Write("Username Petugas : ");
            string uname = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Password         : ");
            string pass = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Nama Petugas     : ");
            string name = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Kode Stasiun     : ");
            string station = Console.ReadLine()?.Trim() ?? "GMR";
            return (uname, pass, name, station);
        }

        public string PromptInput(string label)
        {
            Console.Write(label);
            return Console.ReadLine()?.Trim() ?? "";
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}