using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGet("api/coupon", () =>
{
  return Results.Ok(CouponStore.couponList);
})
.WithName("GetCoupons");

app.MapGet("api/coupon/{id:int}", (int id) =>
{
  var coupon = CouponStore
    .couponList
    .FirstOrDefault(c => c.Id == id);

  return Results.Ok(coupon);
})
.WithName("GetCoupon");

app.MapPost("api/coupon", ([FromBody] Coupon coupon) =>
{
  if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
  {
    return Results.BadRequest("Invalid Id or Coupon Name");
  }
  if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon.Name.ToLower()) != null)
  {
    return Results.BadRequest("Coupon Name already exists");
  }
  coupon.Id = CouponStore
    .couponList
    .OrderByDescending(c => c.Id)
    .FirstOrDefault()
    .Id + 1;
  CouponStore.couponList.Add(coupon);
  return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
  //return Results.Created($"api/coupon/{coupon.Id}", coupon);
})
 .WithName("CreateCoupon"); ;

app.MapPut("api/coupon", () =>
{

});

app.MapDelete("api/coupon/{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();
app.Run();
