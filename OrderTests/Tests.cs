using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static OrderTests.Client;

namespace OrderTests
{
    [TestFixture]
    public class Tests
    {
        //Response we are expected to see for successful calls to the orders api
        string expectedSuccessResponse = "\"Order Placed\"";

        /// <summary>
        /// Making a helper method to validate an expected error message since this will be re-used for many
        /// of the properties we are testing
        /// </summary>
        /// <param name="expectedError">Expected error message to be returned</param>
        /// <param name="order">Order object to pass to the request</param>
        public void TestInvalidOrder(string expectedError, Order order)
        {
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await Client.PlaceOrderAsync(order),
                "Expected an exception to be thrown for a bad request.");
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode, "Unexpected status code returned.");
            Assert.IsTrue(exception.Message.Contains(expectedError),
                $"Returned error message - {exception.Message} - did not contain expected error - {expectedError}");
        }

        /// <summary>
        /// Places a valid order using a correctly formatted request object
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PlaceValidOrder() 
        {
            var respone = await Client.PlaceOrderAsync(Client.GetValidOrder());

            Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Returned response status code did not indicate success.");
            Assert.AreEqual(expectedSuccessResponse, respone.Content, "Unexpected response value returned.");

            //Here I would also call an api that returns the values for the order we just placed to make sure 
            //all the values match, alternatively if no api exists you cold query the database to make sure
            //the values were stored properly
        }

        /// <summary>
        /// Tests each of the acceptable payment terms to make sure they are all successful
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PlaceValidOrder_PaymentTerms() 
        {
            //List of the presently acceptable payment terms
            List<string> paymentTerms = new List<string>() {"Credit", "PO", "NET30", "NET60"};

            //Making sure that each valid payment term returns a success
            foreach(var term in paymentTerms)
            {
                var order = Client.GetValidOrder();
                order.PaymentTerms = term;

                var respone = await Client.PlaceOrderAsync(Client.GetValidOrder());
                Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Returned response status code did not indicate success.");
                Assert.AreEqual(expectedSuccessResponse, respone.Content, "Unexpected response value returned.");
            }
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown for invalid payment terms
        /// </summary>
        [Test]
        public void InvalidOrder_PaymentTerms()
        {
            string expectedError = "Payment terms must be one of the following values ";
            var order = Client.GetValidOrder();
            order.PaymentTerms = "1234";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests each of the acceptable currency values to make sure they are all successful
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PlaceValidOrder_Currency()
        {
            //List of the presently acceptable currency values
            List<string> currencyValues = new List<string>() { "USD", "AUD", "GBP", "EUR" };

            //Making sure that each valid currency value returns a success
            foreach (var currencyValue in currencyValues)
            {
                var order = Client.GetValidOrder();
                order.Currency = currencyValue;

                var respone = await Client.PlaceOrderAsync(Client.GetValidOrder());
                Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Returned response status code did not indicate success.");
                Assert.AreEqual(expectedSuccessResponse, respone.Content, "Unexpected response value returned.");
            }
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown for invalid currency
        /// </summary>
        [Test]
        public void InvalidOrder_Currency()
        {
            string expectedError = "Currency must be one of the following values";
            var order = Client.GetValidOrder();
            order.Currency = "Franks";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown for a totally blank request
        /// </summary>
        [Test]
        public void InvalidOrder_BlankRequest()
        {
            string expectedError = "Cannot write a null value for property";
            TestInvalidOrder(expectedError, new Order());
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown for an invalid order id length
        /// </summary>
        [Test]
        public void InvalidOrder_OrderIdLength()
        {
            string expectedError = "Order Id must be a minimum length of 6";
            var order = Client.GetValidOrder();
            order.OrderId = "1234";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when the order id contains invalid characters
        /// </summary>
        [Test]
        public void InvalidOrder_OrderIdOnlyDigits()
        {
            string expectedError = "Order Id can only contains digits 0-9";
            var order = Client.GetValidOrder();
            order.OrderId = "123456P";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when the order id is not provided
        /// </summary>
        [Test]
        public void InvalidOrder_OrderIdEmpty()
        {
            string expectedError = "Cannot write a null value for property 'OrderId'. Property requires a value.";
            var order = Client.GetValidOrder();
            order.OrderId = null;

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when the order name exceeds the maximum length
        /// </summary>
        [Test]
        public void InvalidOrder_OrderNameLength()
        {
            string expectedError = "Order name cannot be greater than 50 characters in length";
            var order = Client.GetValidOrder();
            order.OrderName = "Long Order Name that is bound to be longer than the minium of 50 characters";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when the order name is not provided
        /// </summary>
        [Test]
        public void InvalidOrder_OrderNameEmpty()
        {
            string expectedError = "Cannot write a null value for property 'OrderName'. Property requires a value.";
            var order = Client.GetValidOrder();
            order.OrderName = null;

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when an invalid email address is provided
        /// </summary>
        [Test]
        public void InvalidOrder_BillToEmail()
        {
            string expectedError = "Invalid Bill To email address";
            var order = Client.GetValidOrder();
            order.BillToPerson.EmailAddress = "notvalid.com";

            TestInvalidOrder(expectedError, order);
        }

        /// <summary>
        /// Tests to make sure the order can be submitted without a region when the country being supplied
        /// does not require a region
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PlaceValidOrder_RegionNotRequired()
        {
            var order = Client.GetValidOrder();
            order.BillToPerson.Country = "FR";
            order.BillToPerson.Region = "";
            order.ShipToPerson.Country = "FR";
            order.ShipToPerson.Region = "";

            var respone = await Client.PlaceOrderAsync(order);
            Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Returned response status code did not indicate success.");
            Assert.AreEqual(expectedSuccessResponse, respone.Content, "Unexpected response value returned.");
        }

        /// <summary>
        /// Tests to make sure the proper error is thrown when a region is not supplied when its required by the country
        /// </summary>
        [Test]
        public void InvalidOrder_RegionRequired() 
        {
            List<String> regionCountries = new List<string>() {"US", "CA"};
            string expectedError = "Region Must be supplied if Country is US or CA";

            foreach (var country in regionCountries)
            {
                var order = Client.GetValidOrder();
                order.BillToPerson.Country = country;
                order.BillToPerson.Region = "";

                TestInvalidOrder(expectedError, order);

                order = Client.GetValidOrder();
                order.ShipToPerson.Country = country;
                order.ShipToPerson.Region = "";

                TestInvalidOrder(expectedError, order);
            }
        }
    }
}
