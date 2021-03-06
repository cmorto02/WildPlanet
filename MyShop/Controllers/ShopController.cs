﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyShop.Interfaces;
using MyShop.Models;
using MyShop.Models.Interfaces;

namespace MyShop.Controllers
{
    public class ShopController : Controller
    {
        private IInventoryManager _context;
        private IBasketManager _basket;

        /// <summary>
        /// bring in context
        /// </summary>
        /// <param name="context">database</param>
        public ShopController(IInventoryManager context, IBasketManager basket)
        {
            _context = context;
            _basket = basket;
        }
        /// <summary>
        /// the home page of shop renders all of the shop items
        /// </summary>
        /// <returns>all products in the database</returns>
        public async Task<IActionResult> Index()
        {
            var Inventory = await _context.GetALLProducts();
         
            return View(Inventory);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id < 1)
            {
                return NotFound();
            }
            var product = await _context.GetProduct(id);

            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

    }
}