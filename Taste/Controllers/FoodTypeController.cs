using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taste.DataAccess.Data.Repository.IRepository;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodTypeController : Controller
    {
        private readonly IUnitOfWork _unitofWork;

        public FoodTypeController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(_unitofWork.FoodType.GetAll());
           
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var foodTypeToDelete = _unitofWork.FoodType.Get(id);
            if(foodTypeToDelete is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitofWork.FoodType.Remove(foodTypeToDelete);
            _unitofWork.Save();
            return Json(new { success = true, message = "FoodType Deleted Successfully" });
        }
    }
}
