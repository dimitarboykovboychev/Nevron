using Microsoft.AspNetCore.Mvc;

public class HomeController: Controller
{
    private const string SessionKey = "NumberList";

    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult AddNumber()
    {
        var list = GetNumberList();
        var rand = new Random();
        int number = rand.Next(1, 101); // Random number between 1 and 100
        list.Add(number);
        HttpContext.Session.SetObject(SessionKey, list);

        return Json(new { numbers = list, count = list.Count });
    }

    [HttpPost]
    public JsonResult ClearNumbers()
    {
        HttpContext.Session.SetObject(SessionKey, new List<int>());
        return Json(new { numbers = new List<int>(), count = 0 });
    }

    [HttpGet]
    public JsonResult SumNumbers()
    {
        var list = GetNumberList();
        int sum = list.Sum();
        return Json(new { sum });
    }

    private List<int> GetNumberList()
    {
        return HttpContext.Session.GetObject<List<int>>(SessionKey) ?? new List<int>();
    }
}

// Extension methods for session object serialization
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
