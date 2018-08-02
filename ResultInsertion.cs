using Newtonsoft.Json;
using SQLcsvInsert.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace SQLcsvInsert
{
    class ResultInsertion
    {
        string BrandId = string.Empty;

        public void Results()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();

            var filepath = currentDirectory + "\\Results";


            //var filepath = "..\\..\\DataFiles\\Results\\";
            GetCSVFormattedData(filepath);
        }

        private void GetCSVFormattedData(string filepath)
        {
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.csv"))
            {
                string brand = CommonMethods.SelectBrandName(file.Name.Split('_')[0].ToUpper());
                BrandId = CommonMethods.SelectBrandDetails(file.Name.Split('_')[0].ToUpper());
                DateTime UtcTime = DateTime.UtcNow;
                string username = "InitLoad";

                DataTable tblcsv = new DataTable();
                //creating columns  
                tblcsv.Columns.Add("POLL_ID");
                tblcsv.Columns.Add("OPTION_ID");
                tblcsv.Columns.Add("TOTAL");

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


                DataRow dr = tbl.NewRow();
                dr["Name"] = tblcsv.Rows[0]["POLL_ID"].ToString().Replace("\"", "");
                dr["Type"] = ConfigurationManager.AppSettings["type"]; 
                dr["Number"] = "Number";
                dr["Brand"] = brand;
                dr["Site"] = BrandId;
                dr["Messages"] = GetMessageFormat(tblcsv);
                dr["LastAccessDate"] = UtcTime;
                dr["CreateDate"] = UtcTime;
                dr["CreateUser"] = username;
                tbl.Rows.Add(dr);

                CommonMethods.InsertCSVRecords(tbl);
                Directory.Move(file.FullName, filepath + "\\Completed\\" + file.Name);
            }
        }


        private string GetMessageFormat(DataTable tbl)
        {
            Messages objModel = new Messages();
            List<Answer> objListAns = new List<Answer>();

            tbl.DefaultView.Sort = "OPTION_ID" + " " + "asc";
            tbl = tbl.DefaultView.ToTable();


            foreach (DataRow item in tbl.Rows)
            {
                objListAns.Add(new Answer
                {
                    optionId = Convert.ToInt32(item["OPTION_ID"]),
                    count = Convert.ToInt32(item["TOTAL"])
                });
            }
            objModel.pollId = tbl.Rows[0]["POLL_ID"].ToString();
            objModel.answers = objListAns;

            return JsonConvert.SerializeObject(objModel);
        }

    }
}
