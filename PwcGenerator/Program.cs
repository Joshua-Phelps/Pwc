using System;
using System.Threading.Tasks;

namespace PwcGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2 )
            {
                Console.Write("Please provide 2 parameters, client id followed by client secret");
                Console.ReadKey();
                return;
            }

            string clientId = args[0];
            string clientSecret = args[1];

            try
            {
                using (var generator = new PetsGenerator(clientId, clientSecret))
                {
                    generator.GetAnimalsAsync().GetAwaiter().GetResult();
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Encountered an error while fetching the animals. Error: {0}", ex.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(string.Format("If successful, output can be found here: {0}", ConfigurationSettings.WriteLocation));
            Console.WriteLine("All done. Enter any key to exit");
            Console.ReadKey();
        }
    }
}
