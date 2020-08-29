using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Utility;

namespace Taste.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MenuItem> MenuItemListItems { get; set; }
        public IEnumerable<Category> CategoryListItems { get; set; }
        public IActionResult OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var shoppingCartCount = _unitOfWork.ShoppingCart.GetAll(a => a.ApplicationUserId == claim.Value).Count();
                HttpContext.Session.SetInt32(SD.ShoppingCart, shoppingCartCount);
                
            }

            MenuItemListItems = _unitOfWork.MenuItem.GetAll(null, null, "Category,FoodType");
            CategoryListItems = _unitOfWork.Category.GetAll(null, c => c.OrderBy(c => c.Order), null);
            return Page();
        }
    }
}