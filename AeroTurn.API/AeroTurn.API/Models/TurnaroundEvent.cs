namespace AeroTurn.API.Models

{
    public class TurnaroundEvent
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string EventName { get; set; } = string.Empty; // e.g., "Refueling", "Catering"
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } // Nullable kyunke jab start hoga tab end time nahi hoga
        public string DetectedBy { get; set; } = "AI_Camera_1"; // Security ke liye
    }
}