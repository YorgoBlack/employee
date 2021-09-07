using EmployeeWebApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeWebApp.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public int? StaffNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string GenderStr { get=> Gender == Gender.Male ? "муж." : "жен."; }
        public string StaffStr { get => StaffNo == null ? "внештатн." : StaffNo.Value.ToString(); }
        public bool IsInStaff { get; set; }
        public string BirthDayString { get; set; }
    }
}