using Hotel.Booking.Api.Extensions;
using Hotel.Booking.Api.Shrared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private readonly ICollection<string> _errors = new List<string>();

        protected IActionResult ResponseOk(object result) =>
            Response(HttpStatusCode.OK, result);

        protected IActionResult ResponseOk() =>
            Response(HttpStatusCode.OK);

        protected IActionResult ResponseCreated() =>
            Response(HttpStatusCode.Created);

        protected IActionResult ResponseCreated(object data) =>
            Response(HttpStatusCode.Created, data);

        protected IActionResult ResponseNoContent() =>
            Response(HttpStatusCode.NoContent);

        protected IActionResult ResponseNotModified() =>
            Response(HttpStatusCode.NotModified);

        protected IActionResult ResponseBadRequest(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
                AddError(error.ErrorMessage);

            return Response(HttpStatusCode.BadRequest, errorMessages: _errors.ToArray());
        }

        protected void AddError(string erro)
        {
            _errors.Add(erro);
        }

        protected IActionResult ResponseBadRequest(string errorMessage) =>
            Response(HttpStatusCode.BadRequest, errorMessage: errorMessage);

        protected IActionResult ResponseBadRequest() =>
            Response(HttpStatusCode.BadRequest, errorMessage: "The request is invalid");

        protected IActionResult ResponseNotFound(string errorMessage) =>
            Response(HttpStatusCode.NotFound, errorMessage: errorMessage);

        protected IActionResult ResponseNotFound() =>
            Response(HttpStatusCode.NotFound, errorMessage: "Resource not found");


        protected new JsonResult Response(HttpStatusCode statusCode, object data, string errorMessage)
        {
            CustomResult result;
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var success = statusCode.IsSuccess();

                if (data != null)
                    result = new CustomResult(statusCode, success, data);
                else
                    result = new CustomResult(statusCode, success);
            }
            else
            {
                var errors = new List<string>();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                    errors.Add(errorMessage);

                result = new CustomResult(statusCode, false, errors);
            }
            return new JsonResult(result) { StatusCode = (int)result.StatusCode };
        }

        protected new JsonResult ResponseErrorList(HttpStatusCode statusCode, string[] errorMessages)
        {
            CustomResult? result = null;
            if (errorMessages.Length > 0)
            {
                var errors = new List<string>();

                foreach (var errorMessage in errorMessages)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        errors.Add(errorMessage);
                }
                result = new CustomResult(statusCode, false, errors);
            }
            return new JsonResult(result) { StatusCode = (int)statusCode };
        }

        private new JsonResult Response(HttpStatusCode statusCode, string[] errorMessages) =>
             ResponseErrorList(statusCode, errorMessages);

        protected new JsonResult Response(HttpStatusCode statusCode, object result) =>
            Response(statusCode, result, null);

        protected new JsonResult Response(HttpStatusCode statusCode, string errorMessage) =>
            Response(statusCode, null, errorMessage);

        protected new JsonResult Response(HttpStatusCode statusCode) =>
            Response(statusCode, null, null);
    }
}
