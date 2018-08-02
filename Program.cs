using System;
using SQLcsvInsert;

namespace SQLcsvInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommonMethods.connection();
                Console.WriteLine("Started Reading Entries.");
                EntriesInsertion obj = new EntriesInsertion();
                obj.Entries();
                Console.WriteLine("Finished Reading Entries.\n");
                Console.WriteLine("===================================");
                Console.WriteLine("Started Reading Results.");

                ResultInsertion objResults = new ResultInsertion();
                objResults.Results();
                Console.WriteLine("Finished Reading Results.\n");
                Console.WriteLine("===================================");
                Console.WriteLine("===================================");

                Console.WriteLine("Press Enter to Exis Results.");
                Console.ReadLine();

            }
            catch(Exception ex)
            {
                Console.Write("Exception: " + ex.Message.ToString());
                Console.Read();
                }
        }
    }
}
