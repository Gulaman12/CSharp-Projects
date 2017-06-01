//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;

namespace WebApplication1
{   [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfDataService1 : DataService<MyDataSource>
    {
        
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors= true;
        }
    }
    [DataServiceKey("CustomerID")]
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }

    [DataServiceKey("OrderID")]
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public string OrderDate { get; set; }
        public string ShippedDate { get; set; }
        public string Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
    }

    [DataServiceKey("EmployeeID")]
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        


    }
    public class MyDataSource
    {
        static MyDataSource()
        {
            _Customers =
                XElement.Load(@"C:\usertmp\XCustomers.xml")
                .Elements("Customer")
                .Select(x => new Customer
                {
                    CustomerID = (string)x.Element("CustomerID"),
                    CompanyName = (string)x.Element("CompanyName"),
                    ContactName = (string)x.Element("ContactName"),
                }).ToArray();

            _Employees =
                 XElement.Load(@"C:\usertmp\XEmployees.xml")
                 .Elements("Employee")
                 .Select(x => new Employee
                 {
                     EmployeeID = (int)x.Element("EmployeeID"),
                     FirstName = (string)x.Element("FirstName"),
                     LastName = (string)x.Element("LastName"),
                 }).ToArray();

            _Orders =
               XElement.Load(@"C:\usertmp\XOrders.xml")
               .Elements("Order")
               .Select(x => new Order
               {
                   OrderID = (int)x.Element("OrderID"),
                   CustomerID = (string)x.Element("CustomerID"),
                   EmployeeID = string.IsNullOrEmpty((string)x.Element("EmployeeID")) ? null : (int?)x.Element("EmployeeID"),
                   OrderDate = (string)x.Element("OrderDate"),
                   ShippedDate = (string)x.Element("ShippedDate"),
                   Freight = (string)x.Element("Freight"),
                   ShipName = (string)x.Element("ShipName"),
                   ShipCity = (string)x.Element("ShipCity"),
                   ShipCountry = (string)x.Element("ShipCountry"),
               }).ToArray();

            var _os = _Orders.ToLookup(o => o.CustomerID);
            var _cs = _Customers.ToDictionary(c => c.CustomerID);

            foreach (var o in _Orders) o.Customer = _cs[o.CustomerID];
            foreach (var c in _Customers) c.Orders = _os[c.CustomerID];
            
            var _eos = _Orders.Where(t =>t.EmployeeID != null).ToLookup(y => y.EmployeeID);
            var _ees = _Employees.ToDictionary(e => e.EmployeeID);

            foreach (var y in _Orders.Where(x => x.EmployeeID != null)) y.Employee = _ees[(int)y.EmployeeID];
            foreach (var e in _Employees) e.Orders = _eos[e.EmployeeID];

        }

        static IEnumerable<Customer> _Customers;
        static IEnumerable<Employee> _Employees;
        static IEnumerable<Order> _Orders;
        

        public IQueryable<Customer> Customers { get { return _Customers.AsQueryable(); } }
        public IQueryable<Employee> Employees { get { return _Employees.AsQueryable(); } }
        public IQueryable<Order> Orders { get { return _Orders.AsQueryable(); } }
        
    }
    
}
