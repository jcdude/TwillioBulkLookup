using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace TwillioBulkLookup.Models
{
    public class Carrier
    {
        public string mobile_country_code { get; set; }
        public string mobile_network_code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string error_code { get; set; }
    }


    public class CallerName
    {
        public string caller_name { get; set; }
        public string caller_type { get; set; }
        public string error_code { get; set; }
    }


    public class AddOn
    {
        public string status { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public Results results { get; set; }

        public void ReadPropertiesRecursive(Type type, ref DataRow dr)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {                
                if (property.PropertyType.IsClass)
                {
                    ReadPropertiesRecursive(property.PropertyType,ref dr);
                }
                else
                {
                    if (dr["ekata_" + type.Name + "_" + property.Name].ToString() == "")
                        dr["ekata_" + type.Name + "_" + property.Name] = property.GetValue(this, null);
                }
            }
        }

        public void ReadPropertiesRecursiveColumns(Type type, ref DataTable dt)
        {
            DataColumnCollection columns = dt.Columns;
            foreach (PropertyInfo property in type.GetProperties())
            {
                if(property.PropertyType == typeof(string[]) || property.PropertyType == typeof(string) || property.PropertyType == typeof(bool))
                {
                    if (!columns.Contains("ekata_"+ type.Name + "_" + property.Name))
                        dt.Columns.Add("ekata_" + type.Name + "_" + property.Name, property.PropertyType);
                }
                else if (property.PropertyType.IsClass)
                {
                    ReadPropertiesRecursiveColumns(property.PropertyType, ref dt);
                }
                
            }
        }
    }

    public class Results
    {
        public Ekata_Reverse_Phone ekata_reverse_phone { get; set; }
        public Ekata_Phone_Valid ekata_phone_valid { get; set; }
    }

    public class Ekata_Reverse_Phone
    {
        public string status { get; set; }
        public string request_sid { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public string phone_number { get; set; }
        public string[] warnings { get; set; }
        public string[] historical_addresses { get; set; }
        public Alternate_Phones[] alternate_phones { get; set; }
        public string error { get; set; }
        public bool is_commercial { get; set; }
        public Associated_People[] associated_people { get; set; }
        public string country_calling_code { get; set; }
        public Belongs_To belongs_to { get; set; }
        public bool is_valid { get; set; }
        public string line_type { get; set; }
        public string carrier { get; set; }
        public Current_Addresses[] current_addresses { get; set; }
        public string id { get; set; }
        public string is_prepaid { get; set; }
    }

    public class Associated_People
    {
        public string name { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string relation { get; set; }
        public string id { get; set; }
    }

    public class Alternate_Phones
    {
        public string phone_number { get; set; }
        public object line_type { get; set; }
        public string id { get; set; }
    }

    public class Belongs_To
    {
        public string name { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string[] industry { get; set; }
        public string[] alternate_names { get; set; }
        public string gender { get; set; }
        public string link_to_phone_start_date { get; set; }
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Current_Addresses
    {
        public string city { get; set; }
        public Lat_Long lat_long { get; set; }
        public string is_active { get; set; }
        public string location_type { get; set; }
        public string street_line_2 { get; set; }
        public string link_to_person_start_date { get; set; }
        public string street_line_1 { get; set; }
        public string postal_code { get; set; }
        public string delivery_point { get; set; }
        public string country_code { get; set; }
        public string state_code { get; set; }
        public string id { get; set; }
        public string zip4 { get; set; }
    }

    public class Lat_Long
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string accuracy { get; set; }
    }

    public class Ekata_Phone_Valid
    {
        public string status { get; set; }
        public string request_sid { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public Result1 result { get; set; }
    }

    public class Result1
    {
        public string phone_number { get; set; }
        public string[] warnings { get; set; }
        public string error { get; set; }
        public string country_calling_code { get; set; }
        public bool is_valid { get; set; }
        public string line_type { get; set; }
        public string country_code { get; set; }
        public string carrier { get; set; }
        public string country_name { get; set; }
        public string id { get; set; }
        public string is_prepaid { get; set; }
    }


}
