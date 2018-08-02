using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLcsvInsert
{
    class CommonMethods
    {
        static SqlConnection con;
        public static DataTable GetCSVDataFromFile(FileInfo file, DataTable tblcsv)
        {
            //getting full file path of Uploaded file  
            string CSVFilePath = Path.GetFullPath(file.FullName);

            //Reading All text  
            string ReadCSV = File.ReadAllText(CSVFilePath);
            //spliting row after new line  
            foreach (string csvRow in ReadCSV.Split('\n'))
            {
                if (!string.IsNullOrEmpty(csvRow))
                {
                    //Adding each row into datatable  
                    tblcsv.Rows.Add();
                    int count = 0;
                    foreach (string FileRec in csvRow.Split(','))
                    {
                        tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
                        count++;
                    }

                }
            }

            //tblcsv.Rows[0].Delete();
            tblcsv.AcceptChanges();
            return tblcsv;
        }

        public static string SelectBrandDetails(string Brand)
        {
            switch (Brand)
            {
                case "*****":
                    return "27";

                case "******":
                    return "2700";

                case "********":
                    return "2400";
                default:
                    return string.Empty;
            }
        }

        public static string SelectBrandName(string Brand)
        {
            switch (Brand)
            {
                case "******":
                    return "******";

                case "******":
                    return "******";

                case "******":
                    return "******";
                default:
                    return string.Empty;
            }
        }


        public static void connection()
        {
            try
            {
                con = new SqlConnection(ConfigurationManager.AppSettings["sqlconnection"]);
                con.Open();
                con.Close();
            }
            catch
            {
                Console.WriteLine("Not able to connect database, Please check the Connection String for the Database");
                Console.ReadLine();
                System.Environment.Exit(-1);
            }

        }

        public static void InsertCSVRecords(DataTable csvdt)
        {
            try
            {
                connection();
                //creating object of SqlBulkCopy    
                SqlBulkCopy objbulk = new SqlBulkCopy(con);

                //assigning Destination table name    
                objbulk.DestinationTableName = "[etl].[DataMigration]";
                //Mapping Table column    
                objbulk.ColumnMappings.Add("Name", "Name");
                objbulk.ColumnMappings.Add("Type", "Type");
                objbulk.ColumnMappings.Add("Number", "Number");
                objbulk.ColumnMappings.Add("Brand", "Brand");
                objbulk.ColumnMappings.Add("Site", "Site");
                objbulk.ColumnMappings.Add("Messages", "Messages");
                objbulk.ColumnMappings.Add("LastAccessDate", "LastAccessDate");
                objbulk.ColumnMappings.Add("CreateDate", "CreateDate");
                objbulk.ColumnMappings.Add("CreateUser", "CreateUser");

                objbulk.BulkCopyTimeout = 300;

                //inserting Datatable Records to DataBase    
                con.Open();
                objbulk.WriteToServer(csvdt);
                con.Close();

            }
            catch (Exception ex)
            {
                if (ex is System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("Error Occured : ");
                    Console.WriteLine("Please check the Connection String for the Database");
                    Console.WriteLine("Job Not Completed");
                    Console.WriteLine("------------------------------------------");
                    Console.ReadLine();
                    System.Environment.Exit(-1);
                }
                //return null;
            }
        }


    }
}
