using TraGo.Models;
using TraGo.Views;

namespace TraGo.Controller
{
    public class RedeemController
    {
        private readonly TicketModel _ticketModel = new();
        private readonly RedeemView _redeemView = new();

        // Menu Customer: Melihat daftar tiket & statusnya
        public void ShowMyTickets(Customer customer)
        {
            var tickets = _ticketModel.GetTicketsByUserId(customer.Id);
            _redeemView.DisplayCustomerTickets(tickets);
        }

        // Menu Gate Officer: Validasi Kode Redeem menggunakan State Pattern
        public void ValidateBoarding(GateOfficer officer)
        {
            string? feedback = null;

            while (true)
            {
                string code = _redeemView.ShowRedeemPrompt(officer.StationCode, feedback);
                feedback = null;

                if (code == "0") break;

                if (string.IsNullOrWhiteSpace(code))
                {
                    feedback = "[GAGAL] Kode Redeem tidak boleh kosong.";
                    continue;
                }

                var ticket = _ticketModel.GetTicketByRedeemCode(code);
                if (ticket == null)
                {
                    feedback = $"[DITOLAK] Kode Redeem '{code.ToUpper()}' tidak ditemukan di sistem.";
                    continue;
                }

                // Tampilkan detail tiket yang ditemukan
                _redeemView.DisplayTicketDetail(ticket);

                // Jika bukan PAID, langsung jalankan state.Redeem() untuk menolak akses (UNPAID / REDEEMED)
                if (ticket.Status != TicketStatus.PAID)
                {
                    var (_, rejectMessage) = ticket.ProcessRedeem();
                    feedback = rejectMessage;
                    continue;
                }

                // Jika PAID, minta konfirmasi petugas lalu ubah state ke REDEEMED
                string confirm = _redeemView.ConfirmBoarding();
                if (!confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    feedback = "[INFO] Validasi boarding dibatalkan.";
                    continue;
                }

                var (isSuccess, message) = ticket.ProcessRedeem();
                if (isSuccess)
                {
                    _ticketModel.UpdateTicketStatus(ticket.Id, ticket.Status);
                }
                feedback = message;
            }
        }
    }
}