using AutoMapper;
using EmployeeWebApp.Entities;
using EmployeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EmployeeWebApp.Mappings
{
    public class EmployeeMap : Profile
    {
        public EmployeeMap()
        {
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(s=>s.IsInStaff, opt => opt.MapFrom(d => d.StaffNo != null))
                .ForMember(s=>s.BirthDayString, opt=>opt.MapFrom(d=>d.BirthDay.ToString("dd.MM.yyyy")));

            CreateMap<EmployeeViewModel, Employee>()
                .ForMember(s => s.BirthDay, opt => opt.MapFrom(d => DateTime.ParseExact(d.BirthDayString,"dd.MM.yyyy",CultureInfo.InvariantCulture))); 
        }
    }
}