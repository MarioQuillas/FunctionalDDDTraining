using System.Net.Http;
using System.Web.Http;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Common;
using CustomerManagement.Logic.Model;
using CustomerManagement.Logic.Utils;

namespace CustomerManagement.Api.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository customerRepository;
        private readonly IEmailGateway emailGateway;

        public CustomerController(
            UnitOfWork unitOfWork,
            IEmailGateway emailGateway)
            : base(unitOfWork)
        {
            customerRepository = new CustomerRepository(unitOfWork);
            this.emailGateway = emailGateway;
        }

        [HttpPost]
        [Route("customers")]
        public HttpResponseMessage Create(CreateCustomerModel model)
        {
            var customerName = CustomerName.Create(model.Name);
            var primaryEmail = Email.Create(model.PrimaryEmail);
            var secondaryEmail = GetSecondaryEmail(model.SecondaryEmail);
            var industry = Industry.Get(model.Industry);

            var result = Result.Combine(customerName, primaryEmail, secondaryEmail, industry);
            if (result.IsFailure)
                return Error(result.Error);

            var customer = new Customer(customerName.Value, primaryEmail.Value, secondaryEmail.Value, industry.Value);
            customerRepository.Save(customer);

            return Ok();
        }

        private Result<Maybe<Email>> GetSecondaryEmail(string secondaryEmail)
        {
            if (secondaryEmail == null)
                return Result.Ok<Maybe<Email>>(null);

            return Email.Create(secondaryEmail)
                .Map(email => (Maybe<Email>) email);
        }

        [HttpGet]
        [Route("customers/{id}")]
        public HttpResponseMessage Get(long id)
        {
            var customerOrNothing = customerRepository.GetById(id);
            if (customerOrNothing.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customer = customerOrNothing.Value;

            var model = new
            {
                customer.Id,
                Name = customer.Name.Value,
                PrimaryEmail = customer.PrimaryEmail.Value,
                SecondaryEmail = customer.SecondaryEmail.HasValue ? customer.SecondaryEmail.Value.Value : null,
                Industry = customer.EmailingSettings.Industry.Name,
                customer.EmailingSettings.EmailCampaign,
                customer.Status
            };

            return Ok(model);
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public HttpResponseMessage DisableEmailing(long id)
        {
            var customerOrNothing = customerRepository.GetById(id);
            if (customerOrNothing.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customer = customerOrNothing.Value;
            customer.DisableEmailing();

            return Ok();
        }

        [HttpPut]
        [Route("customers/{id}")]
        public HttpResponseMessage Update(UpdateCustomerModel model)
        {
            var customerResult = customerRepository.GetById(model.Id)
                .ToResult("Customer with such Id is not found: " + model.Id);
            var industryResult = Industry.Get(model.Industry);

            return Result.Combine(customerResult, industryResult)
                .OnSuccess(() => customerResult.Value.UpdateIndustry(industryResult.Value))
                .OnBoth(result => result.IsSuccess ? Ok() : Error(result.Error));
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public HttpResponseMessage Promote(long id)
        {
            return customerRepository.GetById(id)
                .ToResult("Customer with such Id is not found: " + id)
                .Ensure(customer => customer.CanBePromoted(), "The customer has the highest status possible")
                .OnSuccess(customer => customer.Promote())
                .OnSuccess(customer => emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status))
                .OnBoth(result => result.IsSuccess ? Ok() : Error(result.Error));
        }
    }
}