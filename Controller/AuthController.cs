using System.Text.RegularExpressions;
using TraGo.Models;
using TraGo.Views;

namespace TraGo.Controller
{
    public class AuthController
    {
        private readonly UserModel _userModel = new();
        private readonly AuthView _authView = new();

        public User? CurrentUser { get; private set; }
        public string? LastStatusMessage { get; set; }

        private static bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) &&
                   username.Length >= 3 &&
                   Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
        }

        private static bool IsValidFullName(string fullName)
        {
            return !string.IsNullOrWhiteSpace(fullName) &&
                   fullName.Length >= 2 &&
                   Regex.IsMatch(fullName, @"^[a-zA-Z\s]+$");
        }

        private static bool IsValidNik(string nik)
        {
            return !string.IsNullOrWhiteSpace(nik) &&
                   nik.Length == 16 &&
                   nik.All(char.IsDigit);
        }

        public void Login()
        {
            var (username, password) = _authView.ShowLoginForm();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LastStatusMessage = "[GAGAL] Username dan password tidak boleh kosong.";
                return;
            }

            var user = _userModel.Authenticate(username, password);
            if (user == null)
            {
                LastStatusMessage = "[GAGAL] Username atau password salah.";
                return;
            }

            CurrentUser = user;
            LastStatusMessage = $"[BERHASIL] Selamat datang, {CurrentUser.FullName} (Role: {CurrentUser.Role})!";
        }

        public void RegisterCustomer()
        {
            var (username, password, fullName, nik) = _authView.ShowRegisterForm();

            if (!IsValidUsername(username))
            {
                LastStatusMessage = "[GAGAL] Username hanya boleh berisi huruf dan angka tanpa spasi/simbol (minimal 3 karakter).";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                LastStatusMessage = "[GAGAL] Password wajib diisi.";
                return;
            }

            if (!IsValidFullName(fullName))
            {
                LastStatusMessage = "[GAGAL] Nama Lengkap hanya boleh berisi huruf dan spasi (minimal 2 karakter).";
                return;
            }

            if (!IsValidNik(nik))
            {
                LastStatusMessage = "[GAGAL] NIK wajib terdiri dari tepat 16 digit angka.";
                return;
            }

            var newCustomer = (Customer)UserFactory.CreateUser(UserRole.Customer, username, password, fullName, nik);
            bool success = _userModel.RegisterCustomer(newCustomer);

            LastStatusMessage = success
                ? "[BERHASIL] Akun Customer berhasil didaftarkan! Silakan login."
                : $"[GAGAL] Username '{username}' sudah digunakan.";
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                _authView.ClearAndBanner();
                _authView.ShowMessage($"[LOGOUT] Sampai jumpa, {CurrentUser.FullName}.");
                Thread.Sleep(5000); // Tahan layar selama 5 detik sebelum kembali ke menu utama
                CurrentUser = null;
                LastStatusMessage = null;
            }
        }

        public void ManageSelfAccount()
        {
            if (CurrentUser == null) return;
            string? feedback = null;

            while (true)
            {
                string choice = _authView.ShowSelfAccountMenu(CurrentUser, feedback);
                feedback = null;

                if (choice == "0") break;

                if (choice == "1")
                {
                    string newName = _authView.PromptInput("Nama Lengkap Baru: ");
                    if (!IsValidFullName(newName))
                    {
                        feedback = "[GAGAL] Nama Lengkap hanya boleh berisi huruf dan spasi (minimal 2 karakter).";
                        continue;
                    }

                    if (_userModel.UpdateFullName(CurrentUser.Id, newName))
                    {
                        CurrentUser.FullName = newName;
                        feedback = "[SUKSES] Nama lengkap berhasil diperbarui.";
                    }
                }
                else if (choice == "2")
                {
                    string newPass = _authView.PromptInput("Password Baru: ");
                    if (string.IsNullOrWhiteSpace(newPass))
                    {
                        feedback = "[GAGAL] Password tidak boleh kosong.";
                        continue;
                    }

                    if (_userModel.UpdatePassword(CurrentUser.Id, newPass))
                    {
                        CurrentUser.Password = newPass;
                        feedback = "[SUKSES] Password berhasil diperbarui.";
                    }
                }
                else
                {
                    feedback = "[INFO] Pilihan tidak valid.";
                }
            }
        }

        public void ManageUsersByAdmin()
        {
            string? feedback = null;

            while (true)
            {
                // Selalu ambil data terbaru dari PostgreSQL setiap kali layar di-render
                var users = _userModel.GetAllUsers();
                string choice = _authView.ShowAdminUserMenu(users, feedback);
                feedback = null;

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        var (uname, pass, name, station) = _authView.ShowCreateOfficerForm();
                        if (!IsValidUsername(uname))
                        {
                            feedback = "[GAGAL] Username hanya boleh berisi huruf dan angka tanpa spasi/simbol (minimal 3 karakter).";
                            break;
                        }

                        if (string.IsNullOrWhiteSpace(pass))
                        {
                            feedback = "[GAGAL] Password wajib diisi.";
                            break;
                        }

                        if (!IsValidFullName(name))
                        {
                            feedback = "[GAGAL] Nama Petugas hanya boleh berisi huruf dan spasi (minimal 2 karakter).";
                            break;
                        }

                        var officer = (GateOfficer)UserFactory.CreateUser(UserRole.GateOfficer, uname, pass, name, station);
                        bool created = _userModel.CreateGateOfficer(officer);
                        feedback = created
                            ? $"[SUKSES] Akun Gate Officer '{uname}' berhasil ditambahkan."
                            : $"[GAGAL] Username '{uname}' sudah digunakan.";
                        break;

                    case "2":
                        if (!int.TryParse(_authView.PromptInput("Masukkan ID User yang ingin diubah: "), out int editId))
                        {
                            feedback = "[GAGAL] ID harus berupa angka.";
                            break;
                        }

                        var targetUser = _userModel.GetUserById(editId);
                        if (targetUser == null)
                        {
                            feedback = $"[GAGAL] User dengan ID {editId} tidak ditemukan.";
                            break;
                        }

                        // Tampilkan profil user yang dipilih sebelum input perubahan
                        _authView.DisplayUserProfile(targetUser, $"PROFIL USER TERPILIH (ID: {editId})");

                        string newName = _authView.PromptInput("\nNama Baru (kosongkan jika tetap): ");
                        string newPass = _authView.PromptInput("Password Baru (kosongkan jika tetap): ");

                        if (string.IsNullOrWhiteSpace(newName) && string.IsNullOrWhiteSpace(newPass))
                        {
                            feedback = "[INFO] Tidak ada perubahan yang disimpan karena kedua kolom dikosongkan.";
                            break;
                        }

                        if (!string.IsNullOrWhiteSpace(newName) && !IsValidFullName(newName))
                        {
                            feedback = "[GAGAL] Nama Baru hanya boleh berisi huruf dan spasi (minimal 2 karakter).";
                            break;
                        }

                        if (!string.IsNullOrWhiteSpace(newName)) _userModel.UpdateFullName(editId, newName);
                        if (!string.IsNullOrWhiteSpace(newPass)) _userModel.UpdatePassword(editId, newPass);
                        feedback = $"[SUKSES] Data pengguna '{targetUser.Username}' (ID {editId}) berhasil diperbarui.";
                        break;

                    case "3":
                        if (!int.TryParse(_authView.PromptInput("Masukkan ID User yang akan dihapus: "), out int delId))
                        {
                            feedback = "[GAGAL] ID harus berupa angka.";
                            break;
                        }

                        if (CurrentUser != null && CurrentUser.Id == delId)
                        {
                            feedback = "[GAGAL] Tidak bisa menghapus akun sendiri.";
                            break;
                        }

                        var userToDelete = _userModel.GetUserById(delId);
                        if (userToDelete == null)
                        {
                            feedback = $"[GAGAL] User dengan ID {delId} tidak ditemukan.";
                            break;
                        }

                        // Tampilkan profil user yang dipilih sebelum konfirmasi hapus
                        _authView.DisplayUserProfile(userToDelete, $"KONFIRMASI HAPUS USER (ID: {delId})");
                        string confirm = _authView.PromptInput("\nYakin ingin menghapus akun ini? (y/n): ");

                        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            _userModel.DeleteUser(delId);
                            feedback = $"[SUKSES] Akun '{userToDelete.Username}' (ID {delId}) berhasil dihapus.";
                        }
                        else
                        {
                            feedback = "[INFO] Penghapusan akun dibatalkan.";
                        }
                        break;

                    default:
                        feedback = "[INFO] Pilihan tidak valid.";
                        break;
                }
            }
        }
    }
}