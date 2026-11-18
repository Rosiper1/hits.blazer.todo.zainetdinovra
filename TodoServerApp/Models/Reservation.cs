namespace TodoServerApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public Table? Table { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime ReservationTime { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
    }
}
