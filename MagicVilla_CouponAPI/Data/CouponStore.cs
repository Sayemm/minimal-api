using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Data
{
  public class CouponStore
  {
    public static List<Coupon> couponList = new List<Coupon>() {
      new Coupon { Id = 1, Name = "10OFF", IsActive = true },
      new Coupon { Id = 2, Name = "20OFF", IsActive = false },
    };
  }
}
