namespace apc_power_usage
{
    internal class PowerReader
    {
        private const decimal powerFactor = 1;
        private const decimal volts = 240;

        public bool DoRead(bool showErrors)
        {
            try
            {
                var client = new HttpClient();
                var result = client.GetStringAsync("http://10.0.0.53").Result;
                if (string.IsNullOrEmpty(result))
                    throw new Exception("No content downloaded");

                var lines = result.Split(new [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                string ampsString = null;
                foreach (var line in lines)
                {
                    if (line.EndsWith("&nbsp;Amps</div>"))
                    {
                        ampsString = line.Split("&nbsp;Amps</div>")[0];
                        break;
                    }
                }

                if (ampsString == null)
                {
                    Console.WriteLine(result);
                    throw new Exception("Could not located amperage");
                }

                decimal amps;
                if (!decimal.TryParse(ampsString, out amps))
                    throw new Exception("Could not parse amps: " + ampsString);
                var kw = powerFactor * amps * volts / 1000m;

                var output = $"{DateTime.UtcNow.ToString("u")},{amps},{kw}";
                Console.WriteLine(output);
                File.AppendAllLines("readings.csv", new [] { output });
            }
            catch (Exception ex)
            {
                if (showErrors)
                {
                    Console.WriteLine(ex.ToString());
                }

                return false;
            }

            return true;
        }
    }
}
