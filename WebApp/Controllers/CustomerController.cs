﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult GetAllCustomer()
        {
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
                return new StatusCodeResult(500);
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
                return new StatusCodeResult(500);
            }

            return Ok(Mapper.Map<CustomerDto>(customer));
        }
    }
}