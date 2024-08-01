using System.Diagnostics;
using System.Text.Json;
using System.Xml.Serialization;

namespace PerformanceComparison
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define data sizes in terms of number of people
            var dataSizes = new[] { 1, 10, 100, 1000, 10000 };

            foreach (var size in dataSizes)
            {
                Console.WriteLine($"Testing with {size} people:");

                var people = GeneratePeople(size);

                // Measure XML serialization performance
                var xmlSerializationTime = MeasureTime(() => SerializeToXml(people));
                Console.WriteLine($"XML Serialization Time: {xmlSerializationTime} ms");

                // Measure XML deserialization performance
                var xmlData = SerializeToXml(people);
                var xmlDeserializationTime = MeasureTime(() => DeserializeFromXml<Person[]>(xmlData));
                Console.WriteLine($"XML Deserialization Time: {xmlDeserializationTime} ms");

                // Measure JSON serialization performance
                var jsonSerializationTime = MeasureTime(() => JsonSerializer.Serialize(people));
                Console.WriteLine($"JSON Serialization Time: {jsonSerializationTime} ms");

                // Measure JSON deserialization performance
                var jsonData = JsonSerializer.Serialize(people);
                var jsonDeserializationTime = MeasureTime(() => JsonSerializer.Deserialize<Person[]>(jsonData));
                Console.WriteLine($"JSON Deserialization Time: {jsonDeserializationTime} ms");

                Console.WriteLine();
            }
        }

        static Person[] GeneratePeople(int count)
        {
            var people = new Person[count];
            for (int i = 0; i < count; i++)
            {
                people[i] = new Person
                {
                    Name = $"Person {i + 1}",
                    Age = 20 + (i % 30),
                    Address = new Address
                    {
                        Street = $"Street {i + 1}",
                        City = $"City {i + 1}"
                    }
                };
            }
            return people;
        }

        static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        static T DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringReader = new StringReader(xml);
            return (T)serializer.Deserialize(stringReader);
        }

        static long MeasureTime(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
    }

    [Serializable]
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }
}