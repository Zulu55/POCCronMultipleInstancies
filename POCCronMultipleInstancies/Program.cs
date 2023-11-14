namespace POCCronMultipleInstancies
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Begin");
            await Task.Delay(1000);
            Console.WriteLine("End");
        }
    }
}