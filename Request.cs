using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    public class Request
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string PPSN { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZIP { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string ConfirmEmail { get; set; }

        public string ContactNumber { get; set; }

        public string CompletedPlace { get; set; }

        public string ProgrammeAwardCompleted { get; set; }

        public string YearAwarded { get; set; }

        public string AwardBody { get; set; }

        public bool DATAProtectionStatement { get; set; }

        public Dictionary<string, string> AttachedFileCollection { get; set; }

        public string CaptchaString { get; set; }

        public string Status { get; set; }

        public string From { get; set; }
    }
}