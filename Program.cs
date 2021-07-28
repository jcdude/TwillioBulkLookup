using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
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
            string outputPathJson = Path.Combine(appPath, "Output", "outPut" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".json");

            List<string> phoneNumbers = new List<string>();
            XLWorkbook wb = new XLWorkbook();
            AppSettings appSettingsDto = new AppSettings();
            DataTable phoneDetails = new DataTable();
            StringBuilder sb = new StringBuilder();

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

            Console.WriteLine("Type 1/2/3/4 and press enter to include addons and hit enter");
            var addOnSelection = Console.ReadLine();
            if (addOnSelection == "1")
            {
                addOns = new List<string>();
            }
            else if(addOnSelection == "2")
            {
                addOns = new List<string> {
                 "ekata_reverse_phone"
                };
            }
            else if (addOnSelection == "3")
            {
                addOns = new List<string> {
                 "ekata_phone_valid"
                };
            }
            else if (addOnSelection == "4")
            {
                addOns = new List<string> {
                 "ekata_reverse_phone","ekata_phone_valid"
                };
            }
            phoneDetails.Columns.Add("phone_number");
            phoneDetails.Columns.Add("carrier_name");
            phoneDetails.Columns.Add("carrier_type");
            phoneDetails.Columns.Add("mobile_country_code");
            phoneDetails.Columns.Add("mobile_network_code");
            phoneDetails.Columns.Add("caller_name");

            if (addOnSelection == "2" || addOnSelection == "4")
            {
                phoneDetails.Columns.Add("caller_fullname");
                phoneDetails.Columns.Add("is_commercial");
                phoneDetails.Columns.Add("gender");
                phoneDetails.Columns.Add("line_type");
            }
            else if (addOnSelection == "3" || addOnSelection == "4")
            {
                
            }
            else if (addOnSelection == "4")
            {
                phoneDetails.Columns.Add("caller_fullname");
                phoneDetails.Columns.Add("is_commercial");
                phoneDetails.Columns.Add("gender");
                phoneDetails.Columns.Add("line_type");
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
                                
                sb.Append(phoneNumberLookUp.Carrier.ToString());
                sb.Append(phoneNumberLookUp.CallerName.ToString());

                Carrier carrier = JsonConvert.DeserializeObject<Carrier>(phoneNumberLookUp.Carrier.ToString());

                CallerName callerName = JsonConvert.DeserializeObject<CallerName>(phoneNumberLookUp.CallerName.ToString());

                if (phoneNumberLookUp.AddOns != null)
                {
                    AddOn addOn = JsonConvert.DeserializeObject<AddOn>(phoneNumberLookUp.AddOns.ToString());

                    if (addOn.results.ekata_reverse_phone.status == "successful")
                    {
                        if (addOn.results.ekata_reverse_phone.result.current_addresses.Length > 0)
                        {
                            int addressCount = 0;
                            foreach (var address in addOn.results.ekata_reverse_phone.result.current_addresses)
                            {
                                phoneDetails.Columns.Add("caller_address_"+addressCount);
                                addressCount++;
                            }

                        }

                        if (addOn.results.ekata_reverse_phone.result.alternate_phones.Length > 0)
                        {
                            int phoneCount = 0;
                            foreach (var alternatePhone in addOn.results.ekata_reverse_phone.result.alternate_phones)
                            {
                                phoneDetails.Columns.Add("alternate_phones_" + phoneCount);
                                phoneCount++;
                            }

                        }
                    }
                }

                DataRow dr = phoneDetails.NewRow();

                dr["phone_number"] = phoneNumber;
                dr["carrier_name"] = carrier.name;
                dr["carrier_type"] = carrier.type;
                dr["mobile_country_code"] = carrier.mobile_country_code ?? "";
                dr["mobile_network_code"] = carrier.mobile_network_code ?? "";
                dr["caller_name"] = callerName.caller_name;

                if (phoneNumberLookUp.AddOns != null)
                {
                    AddOn addOn = JsonConvert.DeserializeObject<AddOn>(phoneNumberLookUp.AddOns.ToString());

                    sb.Append(phoneNumberLookUp.AddOns.ToString());

                    if (addOn.results.ekata_reverse_phone.status == "successful")
                    {
                        dr["caller_fullname"] = addOn.results.ekata_reverse_phone.result.belongs_to.name ?? "";
                        dr["is_commercial"] = addOn.results.ekata_reverse_phone.result.is_commercial.ToString();
                        dr["gender"] = addOn.results.ekata_reverse_phone.result.belongs_to.gender ?? "";
                        dr["line_type"] = addOn.results.ekata_reverse_phone.result.line_type ?? "";

                        if (addOn.results.ekata_reverse_phone.result.current_addresses.Length > 0)
                        {
                            int addressCount = 0;
                            foreach (var address in addOn.results.ekata_reverse_phone.result.current_addresses)
                            {
                                dr["caller_address_"+addressCount] = (address.street_line_1 ?? "") +
                                    (address.street_line_2 ?? "") + " " +
                                    (address.city ?? "") + " " +
                                    (address.state_code ?? "") + " " +
                                    (address.zip4 ?? "");
                                addressCount++;
                            }
                            
                        }

                        if (addOn.results.ekata_reverse_phone.result.alternate_phones.Length > 0)
                        {
                            int phoneCount = 0;
                            foreach (var alternatePhone in addOn.results.ekata_reverse_phone.result.alternate_phones)
                            {
                                dr["alternate_phone_" + phoneCount] = (alternatePhone.line_type ?? "") + ":" + (alternatePhone.phone_number ?? "");
                                phoneCount++;
                            }

                        }
                    }

                    if(addOn.results.ekata_phone_valid.status == "successful")
                    {
                        //addOn.results.ekata_phone_valid.result.
                    }
                }

                phoneDetails.Rows.Add(dr);
            }

            wb.Worksheets.Add(phoneDetails, "LookUp Results");

            wb.SaveAs(outputPath);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputPathJson))
            {
                file.WriteLine(sb.ToString()); // "sb" is the StringBuilder
            }

            Console.WriteLine("Complete");
            Console.ReadLine();
        }
    }
}
