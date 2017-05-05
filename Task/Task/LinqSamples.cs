﻿// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return return all presented in market products")]
		public void Linq2()
		{
			var products =
				from p in dataSource.Products
				where p.UnitsInStock > 0
				select p;

			foreach (var p in products)
			{
				ObjectDumper.Write(p);
			}
		}

		[Category("My LINQ")]
		[Title("LINQ task 001")]
		[Description("Find all values more than set value")]
		public void Linq001()
		{
			decimal value = 10000;

			//List<decimal> values = new List<decimal>();				
			//dataSource.Customers.ForEach(p => values.Add(p.Orders.Sum(v => v.Total)));

			var customers1 =
				from c in dataSource.Customers
				where c.Orders.Sum(v => v.Total) > value
				select c;

			var customers2 = dataSource.Customers.Where(c => c.Orders.Sum(v => v.Total) > value);

			var r1 = customers1.Except(customers2).ToList();
			var r2 = customers2.Except(customers1).ToList();

			var isOk = !customers1.Any(c1 => !customers1.Except(customers2).Contains(c1));

			ObjectDumper.Write(isOk?"Ok":"Not OK");
			foreach (var p in customers1)
			{
				ObjectDumper.Write(p);
			}
		}

		[Category("My LINQ")]
		[Title("LINQ task 002")]
		[Description("Group by country and city")]
		public void Linq002()
		{
		    foreach (Customer customer in dataSource.Customers)
		    {
		        var suppliers = dataSource.Suppliers.Where(s => s.City == customer.City && s.Country == customer.Country).ToList();
                if (suppliers.Count != 0) { Console.WriteLine("Customer id: " + customer.CustomerID); } //to avoid displaying customers without suppliers
                suppliers.ForEach(supplier => Console.WriteLine(supplier.SupplierName));
		    }

		    var customers =
		        dataSource.Customers.Select(c =>
		                new
		                {
		                    Cust = c.CompanyName,
		                    Suppl = dataSource.Suppliers.Where(s => s.City == c.City && s.Country == c.Country).ToList()
                        }).GroupBy(c => c.Cust).ToList();
		}

	    [Category("My LINQ")]
	    [Title("LINQ task 003")]
	    [Description("Orders sum more than value")]
	    public void Linq003()
	    {
	        int value = 10000;
	        var customers = dataSource.Customers.Where(c => c.Orders.Sum(v => v.Total) > value).ToList();
            customers.ForEach(customer => Console.WriteLine(@"Customer: {0}; Total orders: {1}",
                customer.CompanyName, customer.Orders.Sum(v => v.Total)));
	    }

        [Category("My LINQ")]
        [Title("LINQ task 004")]
        [Description("First order date")]
        public void Linq004()
        {
            var customers =
                dataSource.Customers.Where(o => o.Orders.Count() != 0).Select(c => new {name = c.CompanyName, date = c.Orders.Min(d => d.OrderDate)})
                    .ToList();
            customers.ForEach(customer => Console.WriteLine(@"Customer: {0}; First order: {1}",
                customer.name, customer.date));
        }

        [Category("My LINQ")]
        [Title("LINQ task 005")]
        [Description("Sorted first order date")]
        public void Linq005()
        {
            var customers =
                dataSource.Customers.Where(o => o.Orders.Count() != 0)
                .Select(c => new { name = c.CompanyName, date = c.Orders.Min(d => d.OrderDate), value = c.Orders.Sum(v => v.Total) })
                .Distinct()
                .OrderByDescending(d => d.date.Year)
                .ThenByDescending(m => m.date.Month)
                .ThenByDescending(v => v.value)
                .ThenByDescending(n => n.name)
                .ToList();
            customers.ForEach(customer => Console.WriteLine(@"Customer: {0}; First order: {1}",
                customer.name, customer.date));
        }
	}
}
