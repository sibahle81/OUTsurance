using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestCsvFileTransportWrite()
        {
            var a = new DataIntegrationService();
            var planets = new List<Planet>()
            {
                new Planet() {Name = "Mercury", DistanceFromSun = 57.91, OrderFromSun = 1},
                new Planet() {Name = "Venus", DistanceFromSun = 108.2, OrderFromSun = 2},
                new Planet() {Name = "Earth", DistanceFromSun = 149.6, OrderFromSun = 3},
                new Planet() {Name = "Mars", DistanceFromSun = 227.9, OrderFromSun = 4},
                new Planet() {Name = "Jupiter", DistanceFromSun = 778.5, OrderFromSun = 5},
                new Planet() {Name = "Saturn", DistanceFromSun = 1429, OrderFromSun = 6},
                new Planet() {Name = "Uranus", DistanceFromSun = 2877, OrderFromSun = 7},
                new Planet() {Name = "Neptune", DistanceFromSun = 4498, OrderFromSun = 8},
            };
            var csvSerializer = new CsvSerializer
            {
                Delimiter = "|",
                HasHeaderRecord = true
            };
            var transport = new LocalFileTransport {FilePath = $"C:\\temp\\testplanets.csv"};
            a.SendAsyncData(planets, csvSerializer, transport);
        }

        [TestMethod]
        public void TestCsvFileTransportRead()
        {
            var a = new DataIntegrationService();

            var csvSerializer = new CsvSerializer
            {
                Delimiter = "|",
                HasHeaderRecord = true
            };
            var transport = new LocalFileTransport {FilePath = $"C:\\temp\\testplanets.csv"};
            var result = a.ReceiveAsyncData<Planet>(csvSerializer, transport);
        }

        [TestMethod]
        public void TestOUTsuranceOutput1Process()
        {
            var a = new DataIntegrationService();

            var csvSerializer = new CsvSerializer
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };
            var transport = new LocalFileTransport {FilePath = $"C:\\temp\\data.csv"};
            var input = a.ReceiveAsyncData<OUTsuranceInput>(csvSerializer, transport);
            var inputList = new List<string>();
            foreach (var record in input)
            {
                inputList.Add(record.FirstName);
                inputList.Add((record.LastName));
            }

            var grouping = inputList.GroupBy(n => n).OrderByDescending(n => n.Count()).ThenBy(n => n.Key);

            var output = grouping.Select(@group => new OUTsuranceOutput1
            {
                Name = @group.Key,
                Count = @group.Count()
            }).ToList();

            csvSerializer = new CsvSerializer
            {
                Delimiter = ",",
                HasHeaderRecord = false
            };

            transport = new LocalFileTransport {FilePath = $"C:\\temp\\OUTsuranceOutput1.csv"};
            a.SendAsyncData(output, csvSerializer, transport);

            // write text file as well
            var textOutput = new string[output.Count];
            var counter = 0;
            foreach (var outputRecord in output)
            {
                textOutput[counter] = $"{outputRecord.Name}, {outputRecord.Count}";
                counter++;
            }

            File.WriteAllLines(@"C:\\temp\\OUTsuranceOutput1.txt", textOutput);
        }


        [TestMethod]
        public void TestOUTsuranceOutput2Process()
        {
            var a = new DataIntegrationService();

            var csvSerializer = new CsvSerializer
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };
            var transport = new LocalFileTransport {FilePath = $"C:\\temp\\data.csv"};
            var input = a.ReceiveAsyncData<OUTsuranceInput>(csvSerializer, transport);
            var inputList = input.Select(record => record.Address).ToList();

            var addresses = inputList.Select(s => new
            {
                FullAddress = s.Trim(),
                Tokens = s.Trim().Split()
            })
                .Where(x => x.Tokens.Length > 1 && x.Tokens[0].All(Char.IsDigit))
                .Select(x => new Address
                {
                    FullAddress = x.FullAddress,
                    Street = string.Join(" ", x.Tokens.Skip(1)),
                    Number = int.Parse(x.Tokens[0])
                })
                .OrderBy(addr => addr.Street)
                .ToList();

            var output = addresses.Select(@address => new OUTsuranceOutput2
            {
                FullAddress = @address.FullAddress
            }).ToList();

            csvSerializer = new CsvSerializer
            {
                HasHeaderRecord = false
            };

            transport = new LocalFileTransport {FilePath = $"C:\\temp\\OUTsuranceOutput2.csv"};
            a.SendAsyncData(output, csvSerializer, transport);

            // write text file as well
            var textOutput = new string[addresses.Count];
            var counter = 0;
            foreach (var outputRecord in output)
            {
                textOutput[counter] = outputRecord.FullAddress;
                counter++;
            }

            File.WriteAllLines(@"C:\\temp\\OUTsuranceOutput2.txt", textOutput);
        }
    }

    public class Planet
    {
        public string RecordType { get; set; } = "PLANET";
        public string Name { get; set; }
        public int OrderFromSun { get; set; }
        public double DistanceFromSun { get; set; }
    }

    public class OUTsuranceInput
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OUTsuranceOutput1
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class OUTsuranceOutput2
    {
        public string FullAddress { get; set; }
    }

    public class Address
    {
        public string FullAddress { get; set; }
        public int Number { get; set; }
        public string Street { get; set; }
    }
}
