namespace apc_power_usage
{
    internal class ReadingLine
    {
        public DateTime Date { get; }

        public decimal Amps
        {
            get;
        }

        public decimal Kw { get; }

        public ReadingLine(string line)
        {
            var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new Exception("Input line in invalid format");
            Date = DateTime.Parse(parts[0]);
            Amps = decimal.Parse(parts[1]);
            Kw = decimal.Parse(parts[2]);
        }
    }
}