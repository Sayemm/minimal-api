using System.Net;

namespace MagicVilla_CouponAPI.Models
{
  public class APIResponse
  {
    public APIResponse()
    {
      Errormessages = new List<string>();
    }
    public bool IsSuccess { get; set; }
    public Object Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public List<String> Errormessages { get; set; }
  }
}
