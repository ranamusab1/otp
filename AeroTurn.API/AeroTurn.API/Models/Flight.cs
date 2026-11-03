namespace AeroTurn.API.Models

{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string AssignedGate { get; set; } = string.Empty;
        public DateTime ScheduledArrivalTime { get; set; }
        public DateTime TargetOffBlockTime { get; set; } // TOBT
        public string Status { get; set; } = "Scheduled"; // Scheduled, Landed, Turnaround, Departed
    }
}