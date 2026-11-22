namespace TraGo.Models
{
    public enum TicketStatus
    {
        UNPAID,
        PAID,
        REDEEMED
    }

    // Interface State Pattern
    public interface ITicketState
    {
        TicketStatus Status { get; }
        (bool isSuccess, string message) Redeem(Ticket ticket);
    }

    public class UnpaidState : ITicketState
    {
        public TicketStatus Status => TicketStatus.UNPAID;

        public (bool isSuccess, string message) Redeem(Ticket ticket)
        {
            return (false, "[DITOLAK] Tiket berstatus UNPAID. Penumpang belum menyelesaikan pembayaran.");
        }
    }

    public class PaidState : ITicketState
    {
        public TicketStatus Status => TicketStatus.PAID;

        public (bool isSuccess, string message) Redeem(Ticket ticket)
        {
            // Transisi state dari PAID -> REDEEMED
            ticket.SetState(new RedeemedState());
            return (true, $"[VALID] Boarding diizinkan untuk {ticket.PassengerName} (Kursi {ticket.SeatNumber}). Tiket kini berstatus REDEEMED.");
        }
    }

    public class RedeemedState : ITicketState
    {
        public TicketStatus Status => TicketStatus.REDEEMED;

        public (bool isSuccess, string message) Redeem(Ticket ticket)
        {
            return (false, "[DITOLAK] Tiket sudah berstatus REDEEMED (hangus / sudah digunakan sebelumnya).");
        }
    }

    // Context Class pada State Pattern
    public class Ticket
    {
        public int Id { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string? RedeemCode { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerNik { get; set; } = string.Empty;
        public string TrainName { get; set; } = string.Empty;
        public string TrainClass { get; set; } = string.Empty;
        public string OriginStation { get; set; } = string.Empty;
        public string OriginCode { get; set; } = string.Empty;
        public string DestinationStation { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string SeatNumber { get; set; } = string.Empty;

        public ITicketState CurrentState { get; private set; } = new UnpaidState();
        public TicketStatus Status => CurrentState.Status;

        public void SetState(ITicketState newState)
        {
            CurrentState = newState;
        }

        public void InitializeState(TicketStatus status)
        {
            CurrentState = status switch
            {
                TicketStatus.UNPAID => new UnpaidState(),
                TicketStatus.PAID => new PaidState(),
                TicketStatus.REDEEMED => new RedeemedState(),
                _ => new UnpaidState()
            };
        }

        public (bool isSuccess, string message) ProcessRedeem()
        {
            return CurrentState.Redeem(this);
        }
    }
}