using System.Text.Json;

namespace RankCalculator.Services
{
    public class EventMessage
    {
        public string Id { get; private set; }
        public double CalculatedValue { get; private set; }

        private EventMessage() { }

        public EventMessage( string id, double calculatedValue )
        {
            Id = id;
            CalculatedValue = calculatedValue;
        }

        public static string Serialize( EventMessage message )
        {
            return JsonSerializer.Serialize(message);
        }

        public static EventMessage Deserialize( string message )
        {
            EventMessage? deserializedData = JsonSerializer.Deserialize<EventMessage>(message);

            if (deserializedData == null)
            {
                throw new Exception($"Can not deserialize string {message}");
            }

            return deserializedData;
        }
    }
}
