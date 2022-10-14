using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderTests
{
    [TestFixture]
    public class Tests
    {

        /// <summary>
        /// Places a valid order using a correctly formatted request object
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PlaceValidOrder() 
        {
            string expectedResponse = "\"Order Placed\"";
            var respone = await Client.PlaceOrderAsync(Client.GetValidOrder());

            Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Returned response status code did not indicate success.");
            Assert.AreEqual(expectedResponse, respone.Content, "Unexpected response value returned.");

            //Here I would also call an api that returns the values for the order we just placed to make sure 
            //all the values match, alternatively if no api exists you cold query the database to make sure
            //the values were stored properly
        }

        /// <summary>
        /// Makes sure exception is thrown when issueing a blank request
        /// </summary>
        [Test]
        public void InvalidOrder_BlankRequest()
        {
            string expectedError = "Cannot write a null value for property";

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await Client.PlaceOrderAsync(new Client.Order()),
                "Expected an exception to be thrown for a bad request.");
            Assert.IsTrue(exception.Message.Contains(expectedError), 
                $"Returned error message - {exception.Message} - did not contain expected error - {expectedError}");
        }

        #region OrderId

        [Test]
        public void InvalidOrder_OrderIdLength()
        {
            string expectedError = "Order Id must be a minimum length of 6";
            //Starting with a valid order and then altering the property we want to check for
            var order = Client.GetValidOrder();
            order.OrderId = "1234";

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await Client.PlaceOrderAsync(order),
                "Expected an exception to be thrown for a bad request.");
            Assert.IsTrue(exception.Message.Contains(expectedError),
                $"Returned error message - {exception.Message} - did not contain expected error - {expectedError}");
        }

        [Test]
        public void InvalidOrder_OrderIdOnlyDigits()
        {
            string expectedError = "Order Id can only contains digits 0-9";
            //Starting with a valid order and then altering the property we want to check for
            var order = Client.GetValidOrder();
            order.OrderId = "123456P";

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await Client.PlaceOrderAsync(order),
                "Expected an exception to be thrown for a bad request.");
            Assert.IsTrue(exception.Message.Contains(expectedError),
                $"Returned error message - {exception.Message} - did not contain expected error - {expectedError}");
        }

        [Test]
        public void InvalidOrder_OrderEmpty()
        {
            string expectedError = "Cannot write a null value for property 'OrderId'. Property requires a value.";
            //Starting with a valid order and then altering the property we want to check for
            var order = Client.GetValidOrder();
            order.OrderId = null;

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await Client.PlaceOrderAsync(order),
                "Expected an exception to be thrown for a bad request.");
            Assert.IsTrue(exception.Message.Contains(expectedError),
                $"Returned error message - {exception.Message} - did not contain expected error - {expectedError}");
        }

        #endregion OrderId

    }
}
