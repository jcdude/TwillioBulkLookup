using System;
using System.Collections.Generic;
using System.Text;

namespace TwillioBulkLookup.Models
{
    class test
    {

        public class Rootobject
        {
            public string status { get; set; }
            public object message { get; set; }
            public object code { get; set; }
            public Results results { get; set; }
        }

        public class Results
        {
            public Ekata_Reverse_Phone ekata_reverse_phone { get; set; }
        }

        public class Ekata_Reverse_Phone
        {
            public string status { get; set; }
            public string request_sid { get; set; }
            public object message { get; set; }
            public object code { get; set; }
            public Result result { get; set; }
        }

        public class Result
        {
            public string phone_number { get; set; }
            public object[] warnings { get; set; }
            public object[] historical_addresses { get; set; }
            public object[] alternate_phones { get; set; }
            public object error { get; set; }
            public bool is_commercial { get; set; }
            public Associated_People[] associated_people { get; set; }
            public string country_calling_code { get; set; }
            public Belongs_To belongs_to { get; set; }
            public bool is_valid { get; set; }
            public string line_type { get; set; }
            public string carrier { get; set; }
            public Current_Addresses[] current_addresses { get; set; }
            public string id { get; set; }
            public bool is_prepaid { get; set; }
        }

        public class Belongs_To
        {
            public object age_range { get; set; }
            public string name { get; set; }
            public object firstname { get; set; }
            public object middlename { get; set; }
            public object lastname { get; set; }
            public string[] industry { get; set; }
            public object[] alternate_names { get; set; }
            public object gender { get; set; }
            public string link_to_phone_start_date { get; set; }
            public string type { get; set; }
            public string id { get; set; }
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

        public class Current_Addresses
        {
            public string city { get; set; }
            public Lat_Long lat_long { get; set; }
            public bool is_active { get; set; }
            public string location_type { get; set; }
            public object street_line_2 { get; set; }
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



    }
}
