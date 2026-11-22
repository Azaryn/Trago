using TraGo.Models;

namespace TraGo.Views
{
    public class RedeemView
    {
        public void DisplayCustomerTickets(List<Ticket> tickets)
        {
            Console.Clear();
            Console.WriteLine("=========================================================================================================");
            Console.WriteLine("                                      DAFTAR E-TICKET & KODE REDEEM SAYA                                 ");
            Console.WriteLine("=========================================================================================================");

            if (tickets.Count == 0)
            {
                Console.WriteLine("\nBelum ada tiket yang dipesan.");
            }
            else
            {
                Console.WriteLine("BOOKING         | KODE REDEEM | PENUMPANG        | KERETA (KELAS)            | RUTE       | KURSI  | STATUS");
                Console.WriteLine(new string('-', 105));
                foreach (var t in tickets)
                {
                    string redeem = string.IsNullOrWhiteSpace(t.RedeemCode) ? "-" : t.RedeemCode;
                    string trainInfo = $"{t.TrainName} ({t.TrainClass})";
                    string route = $"{t.OriginCode} -> {t.DestinationCode}";

                    Console.WriteLine(
                        $"{t.BookingCode,-15} | " +
                        $"{redeem,-11} | " +
                        $"{t.PassengerName,-16} | " +
                        $"{trainInfo,-25} | " +
                        $"{route,-10} | " +
                        $"{t.SeatNumber,-6} | " +
                        $"{t.Status}");
                }
            }

            Console.Write("\nTekan ENTER untuk kembali ke menu...");
            Console.ReadLine();
        }

        public string ShowRedeemPrompt(string officerStation, string? statusMessage = null)
        {
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(statusMessage))
            {
                Console.WriteLine(statusMessage);
                Console.WriteLine(new string('=', 85));
            }

            Console.WriteLine($"=== VALIDASI BOARDING GATE (STASIUN: {officerStation}) ===");
            Console.WriteLine("Ketik '0' untuk kembali ke menu utama.\n");
            Console.Write("Masukkan Kode Redeem Penumpang (contoh: RDM-8X92K1): ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public void DisplayTicketDetail(Ticket t)
        {
            Console.WriteLine("\n--- DETAIL TIKET DITEMUKAN ---");
            Console.WriteLine($"Kode Booking   : {t.BookingCode}");
            Console.WriteLine($"Kode Redeem    : {t.RedeemCode}");
            Console.WriteLine($"Nama Penumpang : {t.PassengerName} (NIK: {t.PassengerNik})");
            Console.WriteLine($"Kereta         : {t.TrainName} ({t.TrainClass})");
            Console.WriteLine($"Rute           : {t.OriginStation} ({t.OriginCode}) -> {t.DestinationStation} ({t.DestinationCode})");
            Console.WriteLine($"Keberangkatan  : {t.DepartureTime:dd MMM yyyy HH:mm}");
            Console.WriteLine($"Nomor Kursi    : {t.SeatNumber}");
            Console.WriteLine($"Status Saat Ini: {t.Status}");
        }

        public string ConfirmBoarding()
        {
            Console.Write("\nKonfirmasi validasi masuk & hanguskan tiket? (y/n): ");
            return Console.ReadLine()?.Trim() ?? "";
        }
    }
}