using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Dtos;
using WebApp.Entities;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private ICustomerRepository _customerRepository;
        ILogger<CustomerController> _logger;
        public CustomerController(ICustomerRepository customerRepository, ILogger<CustomerController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            logger.LogInformation("customercontroler has started!");
        }

        [HttpGet]
        public IActionResult GetAllCustomer()
        {
            _logger.LogInformation("------> GetAllCustomers");
            var allcustomers = _customerRepository.GetAll().ToList();

            var allCustomersDto = allcustomers.Select(x => Mapper.Map<CustomerDto>(x));

            return Ok(allCustomersDto);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetSingleCustomer(Guid id)
        {
            Customer customer = _customerRepository.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<CustomerDto>(customer));
        }

        [HttpPost]
        public IActionResult AddCustomer(CustomerCreateDto customerCreateDto)
        {
            Customer customer = Mapper.Map<Customer>(customerCreateDto);

            _customerRepository.Add(customer);

            bool result = _customerRepository.Save();

            if (!result)
            {
                //return new StatusCodeResult(500);
                throw new Exception("Something went wrong while adding new customer");
            }

            return Ok(Mapper.Map<CustomerDto>(customer));
            //return CreatedAtRoute("GetSingleCustomer", new { id = customer.Id }, Mapper.Map<CustomerDto>(customer));
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateCustomer(Guid id,[FromBody] CustomerUpdateDto customerUpdateDto)
        {
            var customer = _customerRepository.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }

            Mapper.Map(customerUpdateDto, customer);
            _customerRepository.Update(customer);

            bool result = _customerRepository.Save();

            if (!result)
            {
                throw new Exception("Something went wrong while updating customer with id : {id}");
            }

            return Ok(Mapper.Map<CustomerDto>(customer));
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult PartialUpdate(Guid id,[FromBody]  JsonPatchDocument<CustomerUpdateDto> customerPatch)
        {
            if (customerPatch == null)
            {
                return BadRequest();
            }

            Customer customer = _customerRepository.GetSingle(id);

            if(customer == null)
            {
                return NotFound();
            }

            var customerToPatch = Mapper.Map<CustomerUpdateDto>(customer);
            customerPatch.ApplyTo(customerToPatch);

            Mapper.Map(customerToPatch, customer);
            _customerRepository.Update(customer);

            bool result = _customerRepository.Save();

            if (!result)
            {
                throw new Exception("Something went wrong while updating customer with id : {id}");
            }

            return Ok(Mapper.Map<CustomerDto>(customer));
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Remove(Guid id)
        {
            var customer = _customerRepository.GetSingle(id);

            if (customer == null)
            {
                return NotFound();
            }

            _customerRepository.Delete(id);

            bool result = _customerRepository.Save();

            if (!result)
            {
                throw new Exception("Something went wrong while removing customer with id : {id}");
            }

            return NoContent();
        }
    }
}
