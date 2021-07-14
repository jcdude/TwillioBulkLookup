using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Twilio;
using Twilio.Rest.Lookups.V1;
using TwillioBulkLookup.Models;

namespace TwillioBulkLookup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Twilio Bulk LookUp!");

            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string appSettings = Path.Combine(appPath, "appsettings.json");
            string inputPath = Path.Combine(appPath, "Input");
            string outputPath = Path.Combine(appPath, "Output","outPut"+DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");

            List<string> phoneNumbers = new List<string>();
            XLWorkbook wb = new XLWorkbook();
            AppSettings appSettingsDto = new AppSettings();
            DataTable phoneDetails = new DataTable();

            using (StreamReader r = new StreamReader(appSettings))
            {
                string json = r.ReadToEnd();
                appSettingsDto = JsonConvert.DeserializeObject<AppSettings>(json);
            }

            var inputFiles = Directory.GetFiles(inputPath);
            foreach(var file in inputFiles)
            {
                var filePath = Path.Combine(inputPath, file.ToString());

                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        phoneNumbers.Add(values[0].Replace( "-", String.Empty));
                    }
                }
                File.Delete(filePath);
            }

            if(phoneNumbers.Count == 0)
            {
                Console.WriteLine("No Phone numbers to import");
                return;
            }


            TwilioClient.Init(appSettingsDto.SID, appSettingsDto.AuthToken);

            var type = new List<string> {
            "carrier","caller-name"
            }   ;

            var addOns = new List<string> {
             "ekata_reverse_phone"//,"ekata_phone_valid"
            };

            Console.WriteLine("Type 1 and press enter to include addons else just hit enter");
            if (Console.ReadLine() == "")
            {
                addOns = new List<string>();
            }
            phoneDetails.Columns.Add("phone_number");
            phoneDetails.Columns.Add("carrier_name");
            phoneDetails.Columns.Add("carrier_type");
            phoneDetails.Columns.Add("caller_name");

            if (addOns.Count > 0)
            {
                phoneDetails.Columns.Add("caller_fullname");
                phoneDetails.Columns.Add("caller_address");
            }

            foreach (string phoneNumber in phoneNumbers)
            {
                if(phoneNumber.Trim() == string.Empty)
                {
                    continue;
                }

                var phoneNumberLookUp = PhoneNumberResource.Fetch(
                    type: type,
                    addOns: addOns,
                    pathPhoneNumber: new Twilio.Types.PhoneNumber("+1" + phoneNumber)
                );

                Console.Write("=");

                Carrier carrier = JsonConvert.DeserializeObject<Carrier>(phoneNumberLookUp.Carrier.ToString());

                CallerName callerName = JsonConvert.DeserializeObject<CallerName>(phoneNumberLookUp.CallerName.ToString());

                DataRow dr = phoneDetails.NewRow();

                dr["phone_number"] = phoneNumber;
                dr["carrier_name"] = carrier.name;
                dr["carrier_type"] = carrier.type;
                dr["caller_name"] = callerName.caller_name;

                if (phoneNumberLookUp.AddOns != null)
                {
                    AddOn addOn = JsonConvert.DeserializeObject<AddOn>(phoneNumberLookUp.AddOns.ToString());

                    if (addOn.results.ekata_reverse_phone.status == "successful")
                    {
                        dr["caller_fullname"] = addOn.results.ekata_reverse_phone.result.belongs_to.name ?? "";

                        if (addOn.results.ekata_reverse_phone.result.current_addresses.Length > 0)
                        {
                            var address = addOn.results.ekata_reverse_phone.result.current_addresses[0];
                            dr["caller_address"] = address.street_line_1 ?? "" +
                                address.street_line_2 ?? "" + " " +
                                address.city ?? "" + " " +
                                address.state_code ?? "" + " " +
                                address.zip4 ?? "";
                        }
                    }
                }

                phoneDetails.Rows.Add(dr);
            }

            wb.Worksheets.Add(phoneDetails, "LookUp Results");

            wb.SaveAs(outputPath);

            Console.WriteLine("Complete");
            Console.ReadLine();
        }
    }
}
