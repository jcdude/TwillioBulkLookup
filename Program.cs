using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Lookups.V1;

namespace TwillioBulkLookup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Twilio Bulk LookUp!");

            TwilioClient.Init("", "");

            var type = new List<string> {
            "carrier","caller-name"
            }   ;

            Console.WriteLine("Enter Phone Number");
            string phoneNumber = Console.ReadLine();

            var phoneNumberLookUp = PhoneNumberResource.Fetch(
                type: type,
                pathPhoneNumber: new Twilio.Types.PhoneNumber("+1"+phoneNumber)
            );

            Console.WriteLine("Response!");
            Console.WriteLine(phoneNumberLookUp.Carrier);
            Console.WriteLine(phoneNumberLookUp.CallerName);
        }
    }
}
