﻿
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using Microsoft.Extensions.Configuration;
using MyShop.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Models
{
    public class Payment
    {

 
        ///model for payment

        private IBasketManager _basket;
        public IConfiguration Configuration { get; }

        public Payment(IConfiguration configuration, IBasketManager basket)
        {
            _basket = basket;
            Configuration = configuration;
        }
        public bool Run(Order order)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;


            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = Configuration["AuthorizeNetName"],
                ItemElementName = ItemChoiceType.transactionKey,
                Item = Configuration["AuthorizeNetTransKey"]
            };


            creditCardType creditCard = new creditCardType
            {
                cardNumber = Configuration["CreditCardNumber"],
                expirationDate = Configuration["ExpirationDate"]
            };


            customerAddressType billingAddress = GetAddress(order);


            paymentType paymentType = new paymentType
            {
                Item = creditCard
            };


            transactionRequestType transReqType = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = 9.45m,
                payment = paymentType,
                billTo = billingAddress
            };

            createTransactionRequest request = new createTransactionRequest
            {
                transactionRequest = transReqType
            };
            var controller = new createTransactionController(request);
            controller.Execute();
            var response = controller.GetApiResponse();

            if(response != null)
            {
                if(response.messages.resultCode == messageTypeEnum.Ok)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        private lineItemType[] GetLineItems(List<Product> products)
        {
            lineItemType[] items = new lineItemType[products.Count];

            int count = 0;
            foreach (var value in products)
            {
                //COME BACK TO THIS AND FIX UP
                items[count] = new lineItemType
                {
                    itemId = value.ID.ToString(),
                    name = value.Name,
                    quantity = _basket.GetAllItems().Count(),
                    unitPrice = (decimal)value.Price

                };
            }
            return items;
        }

        customerAddressType GetAddress(Order order)
        {
            customerAddressType address = new customerAddressType()
            {
                firstName = order.FirstName,
                lastName = order.LastName,
                address = order.Address,
                city = order.City,
                zip = order.Zip
            };
            return address;
        }
    }
}
