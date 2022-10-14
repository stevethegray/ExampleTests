using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SampleOrder;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Function
{

    /// <summary>
    /// Date model for the order input
    /// </summary>
    public class Order 
    {
        [JsonProperty(Required = Required.Always)]
        public string OrderId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string OrderName { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string CustomerAccount { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string PaymentTerms { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public bool? TaxExempt { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Currency { get; set; }
        public Address BillToPerson { get; set; }
        public Address ShipToPerson { get; set; }

        public class Address 
        {
            [JsonProperty(Required = Required.Always)]
            public string EmailAddress { get; set; }
            [JsonProperty(Required = Required.Always)]
            public string FirstName { get; set; }
            [JsonProperty(Required = Required.Always)]
            public string LastName { get; set; }
            [JsonProperty(Required = Required.Always)]
            public string StreetLine1 { get; set; }
            public string? StreetLine2 { get; set; }
            public string? StreetLine3 { get; set; }
            [JsonProperty(Required = Required.Always)]
            public string City { get; set; }
            [JsonProperty(Required = Required.Always)]
            public string Country { get; set; }
            public string? Region { get; set; }
        }
    }

    /// <summary>
    /// Helper method to check if an email address is valid
    /// </summary>
    /// <param name="email">email string to check</param>
    /// <returns>true if passes validation</returns>
    private bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false; // suggested by @TK-421
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Helper method for making sure the order id contains only digits
    /// </summary>
    /// <param name="str">the string to check</param>
    /// <returns>True if string contains only digits</returns>
    private bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method to validate the properties of the order that 
    /// cannot be handled with Json attributes
    /// </summary>
    /// <param name="order">order to validate</param>
    /// <returns>List containing all validation errors that were found</returns>
    private List<string> ValidateOrder(Order order)
    {
        List<string> errorMessages = new List<string>();

        //Checking ship to and bill to email address
        if (!IsValidEmail(order.BillToPerson.EmailAddress))
        {
            errorMessages.Add($"Invalid Bill To email address - {order.BillToPerson.EmailAddress}");
        }
        if (!IsValidEmail(order.ShipToPerson.EmailAddress))
        {
            errorMessages.Add($"Invalid Ship To email address - {order.ShipToPerson.EmailAddress}");
        }

        //Checking that region is supplied if Country was US or CA
        if (order.BillToPerson.Country.Equals("US") || order.BillToPerson.Country.Equals("CA"))
        {
            if (order.BillToPerson.Region.Length.Equals(0) || order.BillToPerson.Region is null)
            {
                errorMessages.Add("Region Must be supplied if Country is US or CA");
            }
        }
        if (order.ShipToPerson.Country.Equals("US") || order.ShipToPerson.Country.Equals("CA"))
        {
            if (order.ShipToPerson.Region.Length.Equals(0) || order.ShipToPerson.Region is null)
            {
                errorMessages.Add("Region Must be supplied if Country is US or CA");
            }
        }

        //Validating the order id
        if(order.OrderId.Length < 6)
        {
            errorMessages.Add("Order Id must be a minimum length of 6");
        }
        if (!IsDigitsOnly(order.OrderId)) 
        {
            errorMessages.Add("Order Id can only contains digits 0-9");
        }

        //Validating the order name
        if (order.OrderName.Length > 50)
        {
            errorMessages.Add("Order name cannot be greater than 50 characters in length");
        }

        //Validating the customer account
        if (order.CustomerAccount.Length > 50)
        {
            errorMessages.Add("Customer account name cannot be greater than 50 characters in length");
        }

        //Validating payment terms
        List<string> validPaymentTerms = new List<string>() {"Credit", "PO", "NET30", "NET60" };
        if (!validPaymentTerms.Contains(order.PaymentTerms))
        {
            errorMessages.Add($"Payment terms must be one of the following values {JsonConvert.SerializeObject(validPaymentTerms)}");
        }

        //Validating currency
        List<string> validCurrency = new List<string>() { "USD", "AUD", "GBP", "EUR" };
        if (!validCurrency.Contains(order.Currency))
        {
            errorMessages.Add($"Currency must be one of the following values {JsonConvert.SerializeObject(validCurrency)}");
        }

        return errorMessages;
    }

    /// <summary>
    /// Sample web service that will fail when certain criteria are not met on the given input
    /// </summary>
    /// <param name="order"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(Order order, ILambdaContext context)
    {
        
        //Just writing out the input the log for debugging
        Console.Out.WriteLine(JsonConvert.SerializeObject(order));

        //Validating the order
        var errors = ValidateOrder(order);

        //Throwing an exception if any errors were found in the order
        if(errors.Count > 0)
        {
            throw new Exception($"The following errors were found in the provided order - {JsonConvert.SerializeObject(errors)}");
        }

        //If this were a real web service the logic for placing the order in the apprpriate system would go here.

        //If we get this far this should return a 200
        return "Order Placed";
    }
}
