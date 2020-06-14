using InstamojoAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace instamojo_demo.Controllers
{
    public class PaymentController : Controller
    {
        private const string instamojoClientId = "-ClientId-";
        private const string instamojoSecretId = "-SecretId-";

        // These urls use for testing
        private const string auth_endpoint = "https://test.instamojo.com/oauth2/token/";
        private const string endpoint = "https://test.instamojo.com/v2/";

        // and these for production or live environment
        private const string pro_auth_endpoint = "https://www.instamojo.com/oauth2/token/";
        private const string pro_endpoint = "https://api.instamojo.com/v2/";

        // GET: Payment
        public ActionResult Initiate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrder(Models.PaymentInitiateModel paymentData)
        {
            // We can validate the payment data using dataAnnotation on model

            // Using random we crate the transaction id of payment
            Random randomObj = new Random();
            string transactionId = randomObj.Next(10000000, 100000000).ToString();

            // Create instamojo Clint Id and Secret
            Instamojo objClass = InstamojoImplementation.getApi(instamojoClientId,instamojoSecretId, endpoint, auth_endpoint);


             PaymentOrder objPaymentRequest = new PaymentOrder();
            //Required POST parameters
              objPaymentRequest.name = paymentData.name;
              objPaymentRequest.email = paymentData.email;
              objPaymentRequest.phone = paymentData.contactNumber;
              objPaymentRequest.amount = paymentData.amount;
              objPaymentRequest.transaction_id = transactionId; // Unique Id to be provided . This unique id we have to create

              // Redirect url is for when transaction completed than payment gateway redirected to this url
              // Let's set the redirecturl
              // Create Complete Controller function
              objPaymentRequest.redirect_url = "http://localhost:64200/payment/complete";
  
              //webhook is optional. this is used for when payment completed instamojo hit this api for giving the reponse of payment
             // objPaymentRequest.webhook_url = "https://your.server.com/webhook";
              
            
            // Validate the order data
              if (objPaymentRequest.validate())
              {
                    // Email is not valid
                    if (objPaymentRequest.emailInvalid)
                    {
                        
                    }
                    //Name is not valid
                    if (objPaymentRequest.nameInvalid)
                    {
                    }
                    //Phone is not valid
                    if (objPaymentRequest.phoneInvalid)
                    {

                    }
                    //Amount is not valid
                    if (objPaymentRequest.amountInvalid)
                    {
                    }
                    //Currency is not valid
                    if (objPaymentRequest.currencyInvalid)
                    {
                    }
                    //Transaction Id is not valid
                    if (objPaymentRequest.transactionIdInvalid)
                    {
                    }
                    //Redirect Url Id is not valid
                    if (objPaymentRequest.redirectUrlInvalid)
                    {
                    }
    //              Webhook URL is not valid
                    if (objPaymentRequest.webhookUrlInvalid)
                    {
	                }

            }
            else
            {
               try
               {
                    // Create the order and it will reurn payment url and instmojo order id
                    CreatePaymentOrderResponse objPaymentResponse = objClass.createNewPaymentRequest(objPaymentRequest);

                    // Instmojo order id you can save this wihth user data in db for verification on payment verification time
                    string orderId = objPaymentResponse.order.id;
                    string paymentUrl = objPaymentResponse.payment_options.payment_url;

                    // Return on payment url
                    return Redirect(paymentUrl);                   
               }
               catch (ArgumentNullException ex)
               {
               }
               catch (WebException ex)
               {
               }
               catch (IOException ex)
               {
               }
               catch (InvalidPaymentOrderException ex)
               {
                   if (!ex.IsWebhookValid())
					{
					}

					if (!ex.IsCurrencyValid())
					{
					}

					if (!ex.IsTransactionIDValid())
					{
					}
               }
               catch (ConnectionException ex)
               {
               }
               catch (BaseException ex)
               {
               }
               catch (Exception ex)
               {
               }
            }

            return View("");
        }

        [HttpGet]
        public ActionResult Complete()
        {
            // Payment data comes in url so we have to get it from url

            // This id is instamojo unique payment id which can be use to get the payment details from instamojo
            string paymentId = Request.Params["payment_id"];

            // This is payment status which will be Credit
            string paymentStatus = Request.Params["payment_status"];

            // This is orderId
            string id = Request.Params["id"];

            // This id is your unique transaction id which created on create order
            string transactionId = Request.Params["transaction_id"];

            // Now you can verify the order existance in your db using transactin id


            // Let's get the transaction data from payment gatway
            // Thaer are two ways to get transaction data through orderId or transactionid

            InstamojoAPI.Instamojo objClass = InstamojoAPI.InstamojoImplementation.getApi(instamojoClientId, instamojoSecretId, endpoint, auth_endpoint);

            // Get transaciton data using transactionId
            PaymentOrderDetailsResponse response = objClass.getPaymentOrderDetailsByTransactionId(transactionId);

            // get transaciton data using orderId
            PaymentOrderDetailsResponse response1 = objClass.getPaymentOrderDetails(id);


            // Check payment made successfully

            if (response.status.ToLower().Equals("completed") == true)
            {
                // Create these action method
                return RedirectToAction("Success");
            }
            else {
                return RedirectToAction("Failed");
            }
        }

        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Failed()
        {
            return View();
        }
    }
}