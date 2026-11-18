using Microsoft.AspNetCore.SignalR;
using TodoServerApp.Hubs;
using TodoServerApp.Models;

namespace TodoServerApp.Services
{
    public class BookingService
    {
        private List<Table> _tables = new();
        private List<Reservation> _reservations = new();
        private int _nextReservationId = 1;

        public List<Reservation> GetReservations() => _reservations;

        public async Task CancelReservation(int reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation != null)
            {
                // Находим столик и делаем его свободным
                var table = _tables.FirstOrDefault(t => t.Id == reservation.TableId);
                if (table != null) table.IsAvailable = true;

                // Меняем статус брони
                reservation.Status = "Cancelled";

                // Отправляем уведомление через SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Бронь #{reservationId} отменена. Столик #{table?.Id} снова свободен.");
            }
        }

        // 1. Добавляем поле для отправки уведомлений
        private readonly IHubContext<NotificationHub> _hubContext;

        // 2. Добавляем его в конструктор
        public BookingService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;

            // Инициализация столиками
            _tables.AddRange(new List<Table>
            {
                new Table { Id = 1, Seats = 2, Location = "У окна" },
                new Table { Id = 2, Seats = 4, Location = "В центре" },
                new Table { Id = 3, Seats = 6, Location = "VIP-зал" }
            });
        }

        public List<Table> GetTables() => _tables;

        // 3. Делаем метод асинхронным (добавляем async Task)
        public async Task BookTable(int tableId, Client client, DateTime time)
        {
            var table = _tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null && table.IsAvailable)
            {
                table.IsAvailable = false;
                _reservations.Add(new Reservation
                {
                    Id = _nextReservationId++,
                    TableId = tableId,
                    Client = client,
                    ClientId = client.Id,
                    ReservationTime = time,
                    Status = "Confirmed"
                });

                // 4. Отправляем уведомление всем подключенным клиентам
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Столик #{tableId} успешно забронирован!");


            }
        }
    }
}