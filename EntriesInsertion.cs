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
    class EntriesInsertion
    {
       
        string BrandId = string.Empty;

        public void Entries()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();

            var filepath = currentDirectory + "\\Entries";
            GetCSVFormattedData(filepath);
        }

        private void GetCSVFormattedData(string filepath)
        {
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.csv"))
            {
                string brand = CommonMethods.SelectBrandName(file.Name.Split('_')[0].ToUpper());
                string type = ConfigurationManager.AppSettings["type"]; 
                DateTime UtcTime = DateTime.UtcNow;
                string username = "InitLoad";
                string dateconversion; 
                DateTime oDate;

                //Console.WriteLine(file.Name + ".CSV Started Reading from the Entries Folder" );

                BrandId = CommonMethods.SelectBrandDetails(file.Name.Split('_')[0].ToUpper());

                DataTable tblcsv = new DataTable();
                //creating columns  
                tblcsv.Columns.Add("Number");
                tblcsv.Columns.Add("POLL_ID");
                tblcsv.Columns.Add("DATE_ENTRY");

                tblcsv = CommonMethods.GetCSVDataFromFile(file, tblcsv);

                DataTable tbl = new DataTable();
                //creating columns  
                tbl.Columns.Add("Name");
                tbl.Columns.Add("Type");
                tbl.Columns.Add("Number");
                tbl.Columns.Add("Brand");
                tbl.Columns.Add("Site");
                tbl.Columns.Add("Messages");
                tbl.Columns.Add("LastAccessDate");
                tbl.Columns.Add("CreateDate");
                tbl.Columns.Add("CreateUser");



                foreach (DataRow item in tblcsv.Rows)
                {
                    dateconversion = item["DATE_ENTRY"].ToString().Replace("\r", string.Empty);
                    oDate = DateTime.Parse(dateconversion); 
                    dateconversion = oDate.Month + "/" + oDate.Day + "/" + oDate.Year;

                    DataRow dr = tbl.NewRow();
                    dr["ObjName"] = item["POLL_ID"].ToString().Replace("\"","");
                    dr["Type"] = type;
                    dr["Number"] = item["Number"];
                    dr["Brand"] = brand;
                    dr["Site"] = BrandId;
                    dr["Messages"] = "{\"optionId\":0}";
                    dr["LastAccessDate"] = oDate;
                    dr["CreateDate"] = UtcTime;
                    dr["CreateUser"] = username;

                    tbl.Rows.Add(dr);
                }

                CommonMethods.InsertCSVRecords(tbl);

                //Console.WriteLine(tblcsv.Rows.Count + "Records are Inserted into the Table");

                Directory.Move(file.FullName, filepath + "\\Completed\\" + file.Name);
            }
        }
              
    }

}
