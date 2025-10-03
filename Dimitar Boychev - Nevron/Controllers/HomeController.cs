using Microsoft.AspNetCore.Mvc;

public class HomeController: Controller
{
    private const string SessionNumbersKey = "Numbers";
    private const string SessionSumKey = "Sum";

    public ActionResult Index()
    {
        this.ViewData[SessionNumbersKey] = this.GetNumbersFromSession();
        this.ViewData[SessionSumKey] = this.GetSumFromSession();

        return View();
    }

    [HttpPost]
    public JsonResult AddNumber()
    {
        var list = GetNumbersFromSession();
        var rand = new Random();
        
        int number = rand.Next(1, 101); // Random int number between 1 and 100
        
        list.Add(number);
        
        this.HttpContext.Session.SetObject(SessionNumbersKey, list);

        return Json(new { numbers = list, count = list.Count });
    }

    [HttpPost]
    public JsonResult ClearNumbers()
    {
        this.HttpContext.Session.SetObject(SessionNumbersKey, new List<int>());
        
        return Json(new { numbers = new List<int>(), count = 0 });
    }

    [HttpGet]
    public JsonResult SumNumbers()
    {
        var list = GetNumbersFromSession();

        int sum = list.Sum();

        this.HttpContext.Session.SetObject(SessionSumKey, sum);

        return Json(new { sum });
    }

    private List<int> GetNumbersFromSession()
    {
        return this.HttpContext.Session.GetObject<List<int>>(SessionNumbersKey) ?? new List<int>();
    }

    private int GetSumFromSession()
    {
        return this.HttpContext.Session.GetObject<int>(SessionSumKey);
    }
}

public static class SessionExtensions
{
    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
    }
}
