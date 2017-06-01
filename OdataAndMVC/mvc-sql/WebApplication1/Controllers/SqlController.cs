using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using WebApplication1.Models;
using System.Data.Common;
using System.Linq.Expressions;

namespace WebApplication1.Controllers
{
    public class SqlController : Controller
    {
        public SqlController()
        {
            Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-NZ");
        }
        // GET: Sql

        public ActionResult WebGrid(int page = 1, int rowsPerPage = 10, string sortCol = "OrderID", string sortDir = "ASC")
        {
            List<MyModel> res;
            int count;
            string sql;
            if (sortCol == "CompanyName") sortCol = "Customer.CompanyName";
            else if (sortCol == "ContactName") sortCol = "Customer.ContactName";
            else if (sortCol == "EmpFirstName") sortCol = "Employee.FirstName";
            else if (sortCol == "EmpLastName") sortCol = "Employee.LastName";
            
            using (var nwd = new NorthwindEntities())
            
            {
                var _res = nwd.Orders
                    .OrderBy(sortCol + " " + sortDir +","+"OrderID"+" " +sortDir)
                    .Skip((page - 1) * rowsPerPage)
                    .Take(rowsPerPage)

                    .Select(o => new MyModel
                    {
                        OrderID = o.OrderID,
                        OrderDate = o.OrderDate,
                        Freight = o.Freight,
                        ShipCity = o.ShipCity,
                        ShipCountry = o.ShipCountry,
                        CompanyName = o.Customer.CompanyName,
                        ContactName = o.Customer.ContactName,
                        EmpFirstName = o.Employee.FirstName,
                        EmpLastName = o.Employee.LastName
                    });
                res = _res.ToList();
                count = nwd.Orders.Count();
                sql =_res.ToString();


            }
            ViewBag.sql = sql;
            ViewBag.sortCol = sortCol;
            ViewBag.rowsPerPage = rowsPerPage;
            ViewBag.count = count;
            ViewBag.model = res;
            return View(res);
        }
    }

      
    public class MyModel
        {
            [Key]
            public int OrderID { get; set; }
            public Nullable<System.DateTime> OrderDate { get; set; }
            public Nullable<decimal> Freight { get; set; }
            public string ShipCity { get; set; }
            public string ShipCountry { get; set; }
            public string CompanyName { get; set; }
            public string ContactName { get; set; }
            public string EmpFirstName { get; set; }
            public string EmpLastName { get; set; }
    }
}