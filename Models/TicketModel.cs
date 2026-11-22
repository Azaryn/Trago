using Npgsql;

namespace TraGo.Models
{
    public class TicketModel
    {
        private readonly Database _db = Database.Instance;

        public List<Ticket> GetTicketsByUserId(int userId)
        {
            var tickets = new List<Ticket>();
            using var conn = _db.GetConnection();

            string sql = @"
                SELECT 
                    t.id, b.booking_code, t.redeem_code, t.passenger_name, t.passenger_nik,
                    tr.name AS train_name, tr.class::text AS train_class,
                    orig.name AS origin_name, orig.code AS origin_code,
                    dest.name AS dest_name, dest.code AS dest_code,
                    s.departure_time, ss.seat_number, t.status::text
                FROM tickets t
                JOIN bookings b ON t.booking_id = b.id
                JOIN schedules s ON b.schedule_id = s.id
                JOIN trains tr ON s.train_id = tr.id
                JOIN stations orig ON s.origin_station_id = orig.id
                JOIN stations dest ON s.destination_station_id = dest.id
                JOIN schedule_seats ss ON t.seat_id = ss.id
                WHERE b.user_id = @userId
                ORDER BY t.id DESC;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var ticket = MapReaderToTicket(reader);
                if (ticket != null) tickets.Add(ticket);
            }
            return tickets;
        }

        public Ticket? GetTicketByRedeemCode(string redeemCode)
        {
            using var conn = _db.GetConnection();

            string sql = @"
                SELECT 
                    t.id, b.booking_code, t.redeem_code, t.passenger_name, t.passenger_nik,
                    tr.name AS train_name, tr.class::text AS train_class,
                    orig.name AS origin_name, orig.code AS origin_code,
                    dest.name AS dest_name, dest.code AS dest_code,
                    s.departure_time, ss.seat_number, t.status::text
                FROM tickets t
                JOIN bookings b ON t.booking_id = b.id
                JOIN schedules s ON b.schedule_id = s.id
                JOIN trains tr ON s.train_id = tr.id
                JOIN stations orig ON s.origin_station_id = orig.id
                JOIN stations dest ON s.destination_station_id = dest.id
                JOIN schedule_seats ss ON t.seat_id = ss.id
                WHERE UPPER(t.redeem_code) = UPPER(@code);";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("code", redeemCode);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapReaderToTicket(reader);
        }

        public bool UpdateTicketStatus(int ticketId, TicketStatus newStatus)
        {
            using var conn = _db.GetConnection();
            string sql = "UPDATE tickets SET status = @status::ticket_status WHERE id = @id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("status", newStatus.ToString());
            cmd.Parameters.AddWithValue("id", ticketId);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Ticket? MapReaderToTicket(NpgsqlDataReader reader)
        {
            string statusStr = reader.GetString(13);
            if (!Enum.TryParse<TicketStatus>(statusStr, ignoreCase: true, out var status))
                status = TicketStatus.UNPAID;

            var ticket = new Ticket
            {
                Id = reader.GetInt32(0),
                BookingCode = reader.GetString(1),
                RedeemCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                PassengerName = reader.GetString(3),
                PassengerNik = reader.GetString(4),
                TrainName = reader.GetString(5),
                TrainClass = reader.GetString(6),
                OriginStation = reader.GetString(7),
                OriginCode = reader.GetString(8),
                DestinationStation = reader.GetString(9),
                DestinationCode = reader.GetString(10),
                DepartureTime = reader.GetDateTime(11),
                SeatNumber = reader.GetString(12)
            };

            ticket.InitializeState(status);
            return ticket;
        }
    }
}