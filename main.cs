using TraGo.Controller;
using TraGo.Models;
using TraGo.Views;

namespace TraGo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var authController = new AuthController();
            var redeemController = new RedeemController();
            var authView = new AuthView();

            while (true)
            {
                string? status = authController.LastStatusMessage;
                authController.LastStatusMessage = null;

                if (authController.CurrentUser == null)
                {
                    string mainChoice = authView.ShowGuestMenu(status);
                    if (mainChoice == "1") authController.Login();
                    else if (mainChoice == "2") authController.RegisterCustomer();
                    else if (mainChoice == "0") break;
                    else authController.LastStatusMessage = "[INFO] Pilihan tidak valid.";
                }
                else
                {
                    var user = authController.CurrentUser;
                    string choice = authView.ShowRoleMenu(user, status);

                    if (choice == "0")
                    {
                        authController.Logout();
                        continue;
                    }

                    switch (user)
                    {
                        case Admin:
                            if (choice == "1") authController.ManageUsersByAdmin();
                            else if (choice == "2") authController.LastStatusMessage = "[INFO] Hubungkan ke TrainController";
                            else authController.LastStatusMessage = "[INFO] Pilihan tidak valid.";
                            break;

                        case Customer customer:
                            if (choice == "1") authController.ManageSelfAccount();
                            else if (choice == "2") authController.LastStatusMessage = "[INFO] Hubungkan ke BookingController.";
                            else if (choice == "3") redeemController.ShowMyTickets(customer);
                            else authController.LastStatusMessage = "[INFO] Pilihan tidak valid.";
                            break;

                        case GateOfficer officer:
                            if (choice == "1") redeemController.ValidateBoarding(officer);
                            else if (choice == "2") authController.ManageSelfAccount();
                            else authController.LastStatusMessage = "[INFO] Pilihan tidak valid.";
                            break;
                    }
                }
            }
        }
    }
}