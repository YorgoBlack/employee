using EmployeeWebApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeWebApp.Models
{
    public class EmployeeImportModel
    {
        public int? StaffNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set;}
        public string BirthDay { get; set; }
    }
}