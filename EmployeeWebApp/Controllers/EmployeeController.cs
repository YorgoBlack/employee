using AutoMapper;
using EmployeeWebApp.Entities;
using EmployeeWebApp.Models;
using EmployeeWebApp.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EmployeeWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        public EmployeeController(AppDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public ActionResult Index()
        {
            return Json(_db.Employees.AsEnumerable().Select(x => _mapper.Map<EmployeeViewModel>(x)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeeViewModel model)
        {
            try
            {
                var emp = _mapper.Map<Employee>(model);
                var res = _db.Employees.Add(emp);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { errorMsg = "database error" });
            }
        }

        [HttpPost]
        public ActionResult Edit(EmployeeViewModel model)
        {
            try
            {
                var emp = _mapper.Map<Employee>(model);
                var find = _db.Employees.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                if( find != null )
                {
                    _db.Entry(find).CurrentValues.SetValues(emp);
                    _db.SaveChanges();
                    return Json(new { success = true }); 
                }
                return Json(new { errorMsg = "user not found" });
            }
            catch
            {
                return Json(new { errorMsg = "database error" });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var find = _db.Employees.FirstOrDefault(x => x.EmployeeId == id);
                if (find != null)
                {
                    _db.Employees.Remove(find);
                    _db.SaveChanges();
                    return Json(new {success = true});
                }
                return Json(new { errorMsg = "user not found" });
            }
            catch
            {
                return Json(new { errorMsg = "database error" });
            }
        }

        [HttpPost]
        public ActionResult Import()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Insert(0, $"Результаты импорта.{Environment.NewLine}{Environment.NewLine}");

                    using (var reader = new System.IO.StreamReader(Request.Files[0].InputStream, System.Text.Encoding.UTF8))
                    {
                        try
                        {
                            var data = JsonConvert.DeserializeObject<List<EmployeeImportModel>>(reader.ReadToEnd());
                            int nom = 1;
                            bool parse_error = false;
                            DateTime b_date = DateTime.MinValue;
                            var remove_lst = _db.Employees.Where(x => x.StaffNo == null);
                            foreach(var item in data)
                            {
                                if( string.IsNullOrEmpty(item.BirthDay) )
                                {
                                    sb.Append($"Не задана дата рождения, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }
                                else if (!DateTime.TryParseExact(item.BirthDay, new string[] { "dd.MM.yyyy" }, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out b_date))
                                {
                                    sb.Append($"Ошибка даты, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }

                                if (string.IsNullOrEmpty(item.FirstName))
                                {
                                    sb.Append($"Не задано имя, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }
                                if (string.IsNullOrEmpty(item.MiddleName))
                                {
                                    sb.Append($"Не задано отчество, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }
                                if (string.IsNullOrEmpty(item.LastName))
                                {
                                    sb.Append($"Не задана фамилия, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }
                                if (string.IsNullOrEmpty(item.Gender))
                                {
                                    sb.Append($"Не задан пол, строка {nom}{Environment.NewLine}");
                                    parse_error = true;
                                }
                                
                                if( !parse_error )
                                {
                                    if( item.StaffNo != null )
                                    {
                                        var find = _db.Employees.FirstOrDefault(x => x.StaffNo == item.StaffNo);
                                        if( find != null )
                                        {
                                            _db.Entry(find).CurrentValues.SetValues(
                                                new Employee { 
                                                    EmployeeId = find.EmployeeId,
                                                    BirthDay = b_date, 
                                                    FirstName = item.FirstName,
                                                    LastName = item.LastName,
                                                    MiddleName = item.MiddleName,
                                                    Gender = item.Gender == "муж." ? Gender.Male : Gender.Female, 
                                                });
                                            sb.Append($"Успешно обновлена, строка {nom}{Environment.NewLine}");
                                        }
                                        else
                                        {
                                            _db.Employees.Add(new Employee
                                            {
                                                StaffNo = item.StaffNo,
                                                BirthDay = b_date,
                                                FirstName = item.FirstName,
                                                LastName = item.LastName,
                                                MiddleName = item.MiddleName,
                                                Gender = item.Gender == "муж." ? Gender.Male : Gender.Female,
                                            });
                                            sb.Append($"Успешно добавлена, строка {nom}{Environment.NewLine}");
                                        }
                                    }
                                    else
                                    {
                                        _db.Employees.Add(new Employee
                                        {
                                            BirthDay = b_date,
                                            FirstName = item.FirstName,
                                            LastName = item.LastName,
                                            MiddleName = item.MiddleName,
                                            Gender = item.Gender == "муж." ? Gender.Male : Gender.Female,
                                        });
                                        sb.Append($"Успешно добавлена, строка {nom}{Environment.NewLine}");
                                    }
                                }
                                nom++;
                            }
                            _db.Employees.RemoveRange(remove_lst);
                            _db.SaveChanges();

                        }
                        catch(Exception e)
                        {
                            sb.Append($"{e.Message}{Environment.NewLine}");
                        }

                    };
                    
                    return File(System.Text.UTF8Encoding.UTF8.GetBytes(sb.ToString()), "text/html");
                }
                catch
                {
                    return Json(new { errorMsg = "file error" });
                }
            }
            return Json(new { errorMsg = "file error" });
        }
    }
}
