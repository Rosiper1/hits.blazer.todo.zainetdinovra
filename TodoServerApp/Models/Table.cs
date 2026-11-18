namespace TodoServerApp.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Seats { get; set; } // Количество мест
        public string Location { get; set; } = string.Empty; // Например, "У окна" или "Зал"
        public bool IsAvailable { get; set; } = true;
    }
}
