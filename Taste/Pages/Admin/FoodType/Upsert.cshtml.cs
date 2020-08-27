using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Utility;

namespace Taste.Pages.Admin.FoodType
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Models.FoodType FoodType { get; set; }
        public IActionResult OnGet(int? id)
        {
            FoodType = new Models.FoodType();
            if (id != null)
            {
                FoodType = _unitOfWork.FoodType.GetFirstOrDefault(u => u.Id == id);
                if (FoodType == null)
                {
                    return NotFound();
                }

            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (FoodType.Id == 0)
            {
                _unitOfWork.FoodType.Add(FoodType);
            }
            else
            {
                _unitOfWork.FoodType.Update(FoodType);
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}