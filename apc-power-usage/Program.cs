namespace apc_power_usage
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--avg")
            {
                DateTime start;
                DateTime stop;
                if (args.Length == 3)
                {
                    if (!DateTime.TryParse(args[1], out start))
                    {
                        Console.WriteLine("Could not parse start date");
                        Usage();
                        return;
                    }

                    if (!DateTime.TryParse(args[2], out stop))
                    {
                        Console.WriteLine("Could not parse stop date");
                        Usage();
                        return;
                    }
                }
                else if (args.Length == 1)
                {
                    var now = DateTime.UtcNow;
                    stop = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                    start = stop.AddMonths(-1);
                    Console.WriteLine($"Start: {start}");
                    Console.WriteLine($"Stop:  {stop}");
                }
                else
                {
                    Console.WriteLine("Incorrect avg arguments supplied");
                    Usage();
                    return;
                }

                var readingsParser = new ReadingsParser();
                readingsParser.Parse(start, stop);
                return;
            }

            else if (args.Length > 0)
            {
                Console.WriteLine("Unknown args");
                Usage();
                return;
            }

            var reader = new PowerReader();
            bool? lastReadOk = null;
            while (true)
            {
                var res = reader.DoRead(!lastReadOk.HasValue || lastReadOk.Value);
                if (lastReadOk.HasValue && !res && lastReadOk.Value)
                {
                    Console.WriteLine("Failed to read");
                }
                else if (lastReadOk.HasValue && res && !lastReadOk.Value)
                {
                    Console.WriteLine("Read data OK again");
                }

                lastReadOk = res;
                var waitUntil = DateTime.UtcNow.AddSeconds(5);
                while (DateTime.UtcNow < waitUntil)
                    Thread.Sleep(100);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: ./apc-power-usage [--avg [start stop]]");
        }
    }
}