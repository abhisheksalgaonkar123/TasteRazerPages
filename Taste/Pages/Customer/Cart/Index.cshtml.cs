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
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public OrderDetailsCart orderDetailsCartObj { get; set; }
        public void OnGet()
        {
            orderDetailsCartObj = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader(),
                listCart = new List<ShoppingCart>()

            };
            orderDetailsCartObj.OrderHeader.OrderTotal = 0;

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var role = claimIdentity.FindFirst(ClaimTypes.Role);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(a => a.ApplicationUserId == claim.Value);
            if(cart != null)
            {
                orderDetailsCartObj.listCart = cart.ToList();
            }
            foreach (var cartList in orderDetailsCartObj.listCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                orderDetailsCartObj.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }

        }

        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(a => a.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCart(cart, 1);
            _unitOfWork.Save();
            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(a => a.Id == cartId);
            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
                ResetCart(cart);

            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCart(cart, 1);
                _unitOfWork.Save();
            }
            return RedirectToPage("/Customer/Cart/Index");
        }

        private void ResetCart(ShoppingCart cart)
        {
            var cnt = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cart.ApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.ShoppingCart, cnt);
        }

        public IActionResult OnPostRemove(int cartId)
        {

            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(a => a.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            ResetCart(cart);
            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}
