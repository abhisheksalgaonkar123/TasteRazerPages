using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
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
            if (cart != null)
            {
                orderDetailsCartObj.listCart = cart.ToList();
            }
            foreach (var cartList in orderDetailsCartObj.listCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                orderDetailsCartObj.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }

            ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(a => a.Id == claim.Value);
            orderDetailsCartObj.OrderHeader.PickUpName = user.FullName;
            orderDetailsCartObj.OrderHeader.PickUpDate = DateTime.Now;
            orderDetailsCartObj.OrderHeader.PhoneNumber = user.PhoneNumber;


        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            orderDetailsCartObj.listCart = _unitOfWork.ShoppingCart.GetAll(a => a.ApplicationUserId == claim.Value).ToList();
            orderDetailsCartObj.OrderHeader.PaymentStatus = SD.PaymentPending;
            orderDetailsCartObj.OrderHeader.PickUpTime = Convert.ToDateTime(orderDetailsCartObj.OrderHeader.PickUpDate.ToShortDateString() + " " + orderDetailsCartObj.OrderHeader.PickUpTime.ToShortTimeString());
            orderDetailsCartObj.OrderHeader.UserId = claim.Value;
            orderDetailsCartObj.OrderHeader.Status = SD.StatusInProcess;
            List<OrderDetails> orderDetailList = new List<OrderDetails>();
           
            _unitOfWork.OrderHeader.Add(orderDetailsCartObj.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in orderDetailsCartObj.listCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(a => a.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderDetailsCartObj.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Name = item.MenuItem.Name,
                    Count = item.Count
                };
                orderDetailsCartObj.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                _unitOfWork.OrderDetail.Add(orderDetails);
              
            
            }

            _unitOfWork.ShoppingCart.RemoveRange(orderDetailsCartObj.listCart);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Save();

            if (!string.IsNullOrEmpty(stripeToken))
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderDetailsCartObj.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Source = stripeToken,
                    Description = "Order ID :" + orderDetailsCartObj.OrderHeader.Id,
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                orderDetailsCartObj.OrderHeader.TransactionId = charge.Id;
                if (charge.Status.ToLower() == "succeeded")
                {
                    orderDetailsCartObj.OrderHeader.PaymentStatus = SD.PaymentApproved;
                    orderDetailsCartObj.OrderHeader.PaymentStatus = SD.StatusSubmitted;
                }
                else
                {
                    orderDetailsCartObj.OrderHeader.PaymentStatus = SD.PaymentRejected;

                }
            }
            else
            {
                orderDetailsCartObj.OrderHeader.PaymentStatus = SD.PaymentRejected;
            }
            _unitOfWork.Save();
            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = orderDetailsCartObj.OrderHeader.Id });
            

        }

    }
}
