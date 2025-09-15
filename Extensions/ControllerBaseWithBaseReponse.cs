using Microsoft.AspNetCore.Mvc;

namespace PRN232.Lab1.CofeeStore.API.Extensions
{
    public class ControllerBaseWithBaseReponse : ControllerBase
    {
        [NonAction]
        public ActionResult Ok(string? message = "Success")
        {
            return base.Ok(new { Success = true, Message = message });
        }

        [NonAction]
        public ActionResult Ok(object data, string? message = "Success")
        {
            return base.Ok(new { Success = true, Message = message, Data = data });
        }

        [NonAction]
        public ActionResult BadRequest(string? message = "Bad Request")
        {
            return base.BadRequest(new { Success = false, Message = message });
        }

        [NonAction]
        public ActionResult BadRequest(object data, string? message = "Bad Request")
        {
            return base.BadRequest(new { Success = false, Message = message, Data = data });
        }

        [NonAction]
        public ActionResult NotFound(string? message = "Not Found")
        {
            return base.NotFound(new { Success = false, Message = message });
        }

        [NonAction]
        public ActionResult NotFound(object data, string? message = "Not Found")
        {
            return base.NotFound(new { Success = false, Message = message, Data = data });
        }
    }
}
