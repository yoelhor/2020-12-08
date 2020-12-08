#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

    // Check request body
    if (String.IsNullOrEmpty(requestBody))
    {
        return (ActionResult)new BadRequestObjectResult(new ResponseError(){userMessage = "Request content is empty."});
    }

    // Print out the request body
    log.LogInformation("Request body: " + requestBody);

    // Convert the request body into dynamic JSON data object
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    //Log the request data - REMOVE FOLLOWING LIN IN PRODUCTION
    log.LogInformation(requestBody);

    // Check whether the username element is presented
    if (data.username == null ||  data.username.ToString() == "")
    {
        return (ActionResult)new BadRequestObjectResult(new ResponseError(){userMessage = "Missing required `username` element."});
    }

    // Check whether the userId element is presented
    if (data.userId == null ||  data.userId.ToString() == "")
    {
        return (ActionResult)new BadRequestObjectResult(new ResponseError(){userMessage = "Missing required `userId` element."});
    }

    // Check whether the username element is presented
    if (data.password == null ||  data.password.ToString() == "")
    {
        return (ActionResult)new BadRequestObjectResult(new ResponseError(){userMessage = "Missing required `password` element."});
    }

    // Check whether the language element is presented
    if (data.language == null ||  data.language.ToString() == "")
    {
        // Set the default value
        data.language = "en-Us";
    }

    if (data.username.ToString().ToLower() == "1@test.com" && data.userId.ToString() == "abc" && data.password.ToString() == "123")
    {
        // The account has been found with the correct credentials. Return migration is required "true"
        return (ActionResult)new OkObjectResult(new ResponseContent(){migrationRequired = true, password = data.password.ToString() });
    } 
    else
    {
        // The account has been found with but with a wrong password. Raise an error message.
        return (ActionResult)new BadRequestObjectResult(new ResponseError(){userMessage = "Bad username and password (REST API)"});
    }
}

public class ResponseContent
{
    public bool migrationRequired { get; set; }
    public string password { get; set; }
}

public class ResponseError
{
   public ResponseError()
   {
      version = "1.0.1";
      status = 409;
   }

    public string version { get; set; }
    public int status { get; set; }
    public string userMessage { get; set; }
}

