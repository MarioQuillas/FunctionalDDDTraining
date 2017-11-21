using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Utils;

namespace CustomerManagement.Api.Controllers
{
    public class Controller : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public Controller(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage Error(string errorMessage)
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, Envelope.Error(errorMessage));
        }

        protected HttpResponseMessage Ok()
        {
            _unitOfWork.Commit();
            return Request.CreateResponse(HttpStatusCode.OK, Envelope.Ok());
        }

        protected HttpResponseMessage Ok<T>(T result)
        {
            _unitOfWork.Commit();
            return Request.CreateResponse(HttpStatusCode.OK, Envelope.Ok(result));
        }
    }
}