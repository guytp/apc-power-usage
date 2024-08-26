namespace apc_power_usage
{
    internal class ReadingsParser
    {
        private const decimal unitPrice = 0.245m;

        public void Parse(DateTime start, DateTime end)
        {
            if (!File.Exists("readings.csv"))
            {
                Console.WriteLine("readings.csv does not exist");
                return;
            }

            var rawLines = File.ReadAllLines("readings.csv");
            var parsedLines = new List<ReadingLine>();
            foreach (var line in rawLines)
            {
                var parsedLine = new ReadingLine(line);
                if (parsedLine.Date >= start && parsedLine.Date <= end)
                {
                    parsedLines.Add(parsedLine);
                }
            }

            if (parsedLines.Count < 1)
            {
                Console.WriteLine("No data found in date range");
                return;
            }

            var firstTime = parsedLines.OrderBy(x => x.Date).First().Date;
            var lastTime = parsedLines.OrderByDescending(x => x.Date).First().Date;
            var kw = Math.Round(parsedLines.Select(l => l.Kw).Average(), 2);
            var amps = Math.Round(parsedLines.Select(l => l.Amps).Average(), 2);

            var hoursRead = Math.Round((decimal)lastTime.Subtract(firstTime).TotalHours, 3);
            var kwhRead = (hoursRead == 0) ? 0m : Math.Round(kw * hoursRead, 2);
            var costRead = Math.Round(kwhRead * unitPrice, 2);

            var hoursTotal = Math.Round((decimal)end.Subtract(start).TotalHours, 3);
            var kwhTotal  = (hoursTotal == 0) ? 0m : Math.Round(kw * hoursTotal, 2);

            var costTotal = Math.Round(kwhTotal * unitPrice, 2);

            Console.WriteLine($"amps,kw,hoursRead,kwhRead,costRead,hoursTotal,kwhTotal,costTotal");
            Console.WriteLine($"{amps},{kw},{hoursRead},{kwhRead},£{costRead},{hoursTotal},{kwhTotal},£{costTotal}");
            Console.WriteLine($"{parsedLines.Count} results");
        }
    }
}